using System;

namespace ImGuiColorTextEditNet.Syntax;

/// <summary>
/// A dedicated syntax highlighter for C# code.
/// Handles C#-specific constructs like verbatim strings (@""), interpolated strings ($""),
/// and C# number suffixes (m, d, f, ul, etc.) with hardcoded tokenization for accuracy.
/// </summary>
public class CSharpHighlighter : ISyntaxHighlighter
{
    static readonly object DefaultState = new();
    static readonly object MultiLineCommentState = new();
    static readonly object VerbatimStringState = new();
    readonly SimpleTrie<Identifier> _identifiers;

    record Identifier(PaletteIndex Color)
    {
        public string Declaration = "";
    }

    /// <summary>
    /// Creates a new instance of the CSharpHighlighter.
    /// </summary>
    public CSharpHighlighter()
    {
        var language = LanguageDefinition.CSharp();

        _identifiers = new();
        if (language.Keywords != null)
            foreach (var keyword in language.Keywords)
                if (!string.IsNullOrEmpty(keyword))
                    _identifiers.Add(keyword, new(PaletteIndex.Keyword));

        if (language.Identifiers != null)
        {
            foreach (var name in language.Identifiers)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var identifier = new Identifier(PaletteIndex.KnownIdentifier)
                    {
                        Declaration = "Built-in type/function",
                    };
                    _identifiers.Add(name, identifier);
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool AutoIndentation => true;

    /// <inheritdoc/>
    public int MaxLinesPerFrame => 1000;

    /// <inheritdoc/>
    public string? GetTooltip(string id)
    {
        var info = _identifiers.Get(id.AsSpan());
        return info?.Declaration;
    }

    /// <inheritdoc/>
    public object Colorize(Span<Glyph> line, object? state)
    {
        for (int i = 0; i < line.Length;)
        {
            int result = Tokenize(line[i..], ref state);
            Util.Assert(result != 0);

            if (result == -1)
            {
                line[i] = new(line[i].Char, PaletteIndex.Default);
                i++;
            }
            else
                i += result;
        }

        return state ?? DefaultState;
    }

    int Tokenize(Span<Glyph> span, ref object? state)
    {
        int i = 0;

        // Skip leading whitespace
        while (i < span.Length && span[i].Char is ' ' or '\t')
            i++;

        if (i > 0)
            return i;

        int result;

        // Handle verbatim string continuation from previous line
        if ((result = TokenizeVerbatimStringContinuation(span, ref state)) != -1)
            return result;

        if ((result = TokenizeMultiLineComment(span, ref state)) != -1)
            return result;

        if ((result = TokenizeSingleLineComment(span)) != -1)
            return result;

        if ((result = TokenizePreprocessorDirective(span)) != -1)
            return result;

        if ((result = TokenizeVerbatimString(span, ref state)) != -1)
            return result;

        if ((result = TokenizeInterpolatedString(span)) != -1)
            return result;

        if ((result = TokenizeString(span)) != -1)
            return result;

        if ((result = TokenizeCharLiteral(span)) != -1)
            return result;

        if ((result = TokenizeIdentifier(span)) != -1)
            return result;

        if ((result = TokenizeNumber(span)) != -1)
            return result;

        if ((result = TokenizePunctuation(span)) != -1)
            return result;

        return -1;
    }

    static int TokenizeMultiLineComment(Span<Glyph> span, ref object? state)
    {
        int i = 0;
        if (
            state != MultiLineCommentState
            && (span[i].Char != '/' || 1 >= span.Length || span[1].Char != '*')
        )
        {
            return -1;
        }

        state = MultiLineCommentState;
        for (; i < span.Length; i++)
        {
            span[i] = new(span[i].Char, PaletteIndex.MultiLineComment);
            if (span[i].Char == '*' && i + 1 < span.Length && span[i + 1].Char == '/')
            {
                i++;
                span[i] = new(span[i].Char, PaletteIndex.MultiLineComment);
                state = DefaultState;
                return i + 1;
            }
        }

        return i;
    }

    static int TokenizeSingleLineComment(Span<Glyph> span)
    {
        if (span[0].Char != '/' || 1 >= span.Length || span[1].Char != '/')
            return -1;

        for (int i = 0; i < span.Length; i++)
            span[i] = new(span[i].Char, PaletteIndex.Comment);

        return span.Length;
    }

    static int TokenizePreprocessorDirective(Span<Glyph> span)
    {
        if (span[0].Char != '#')
            return -1;

        for (int i = 0; i < span.Length; i++)
            span[i] = new(span[i].Char, PaletteIndex.Preprocessor);

        return span.Length;
    }

    /// <summary>
    /// Handles verbatim strings (@"...") and interpolated verbatim strings ($@"..." or @$"...").
    /// These can span multiple lines, where "" is the escape for a literal quote.
    /// </summary>
    static int TokenizeVerbatimString(Span<Glyph> span, ref object? state)
    {
        int i = 0;
        bool isVerbatim = false;

        // Check for @" or $@" or @$"
        if (i < span.Length && span[i].Char == '@' && i + 1 < span.Length && span[i + 1].Char == '"')
        {
            isVerbatim = true;
            i += 2;
        }
        else if (i < span.Length && span[i].Char == '$' && i + 1 < span.Length && span[i + 1].Char == '@' && i + 2 < span.Length && span[i + 2].Char == '"')
        {
            isVerbatim = true;
            i += 3;
        }
        else if (i < span.Length && span[i].Char == '@' && i + 1 < span.Length && span[i + 1].Char == '$' && i + 2 < span.Length && span[i + 2].Char == '"')
        {
            isVerbatim = true;
            i += 3;
        }

        if (!isVerbatim)
            return -1;

        // Color what we've consumed so far
        for (int j = 0; j < i; j++)
            span[j] = new(span[j].Char, PaletteIndex.String);

        // Scan for end of verbatim string
        for (; i < span.Length; i++)
        {
            span[i] = new(span[i].Char, PaletteIndex.String);
            if (span[i].Char == '"')
            {
                // Check for escaped quote ""
                if (i + 1 < span.Length && span[i + 1].Char == '"')
                {
                    i++;
                    span[i] = new(span[i].Char, PaletteIndex.String);
                }
                else
                {
                    // End of verbatim string
                    state = DefaultState;
                    return i + 1;
                }
            }
        }

        // String continues to next line
        state = VerbatimStringState;
        return i;
    }

    /// <summary>Handles continuation of a verbatim string from a previous line.</summary>
    static int TokenizeVerbatimStringContinuation(Span<Glyph> span, ref object? state)
    {
        if (state != VerbatimStringState)
            return -1;

        for (int i = 0; i < span.Length; i++)
        {
            span[i] = new(span[i].Char, PaletteIndex.String);
            if (span[i].Char == '"')
            {
                if (i + 1 < span.Length && span[i + 1].Char == '"')
                {
                    i++;
                    span[i] = new(span[i].Char, PaletteIndex.String);
                }
                else
                {
                    state = DefaultState;
                    return i + 1;
                }
            }
        }

        return span.Length;
    }

    /// <summary>Handles interpolated strings ($"...").</summary>
    static int TokenizeInterpolatedString(Span<Glyph> span)
    {
        if (span[0].Char != '$' || 1 >= span.Length || span[1].Char != '"')
            return -1;

        int i = 2;
        span[0] = new(span[0].Char, PaletteIndex.String);
        span[1] = new(span[1].Char, PaletteIndex.String);

        for (; i < span.Length; i++)
        {
            var c = span[i].Char;
            span[i] = new(c, PaletteIndex.String);

            if (c == '"')
                return i + 1;

            if (c == '\\' && i + 1 < span.Length)
            {
                i++;
                span[i] = new(span[i].Char, PaletteIndex.String);
            }
        }

        return -1;
    }

    /// <summary>Handles regular strings ("...").</summary>
    static int TokenizeString(Span<Glyph> span)
    {
        if (span[0].Char != '"')
            return -1;

        span[0] = new(span[0].Char, PaletteIndex.String);
        for (int i = 1; i < span.Length; i++)
        {
            var c = span[i].Char;
            span[i] = new(c, PaletteIndex.String);

            if (c == '"')
                return i + 1;

            if (c == '\\' && i + 1 < span.Length)
            {
                i++;
                span[i] = new(span[i].Char, PaletteIndex.String);
            }
        }

        return -1;
    }

    static int TokenizeCharLiteral(Span<Glyph> span)
    {
        int i = 0;

        if (span[i++].Char != '\'')
            return -1;

        if (i < span.Length && span[i].Char == '\\')
            i++; // handle escape characters

        i++; // Skip actual char

        // handle end of character literal
        if (i >= span.Length || span[i].Char != '\'')
            return -1;

        for (int j = 0; j <= i; j++)
            span[j] = new(span[j].Char, PaletteIndex.CharLiteral);

        return i + 1;
    }

    int TokenizeIdentifier(Span<Glyph> input)
    {
        int i = 0;

        var c = input[i].Char;
        if (c != '_' && c != '@' && !char.IsLetter(c))
            return -1;

        // Allow @ as identifier prefix (e.g., @class)
        if (c == '@')
        {
            i++;
            if (i >= input.Length || (!char.IsLetter(input[i].Char) && input[i].Char != '_'))
                return -1;
        }

        i++;

        for (; i < input.Length; i++)
        {
            c = input[i].Char;
            if (c != '_' && !char.IsLetterOrDigit(c))
                break;
        }

        var info = _identifiers.Get<Glyph>(input[..i], x => x.Char);

        for (int j = 0; j < i; j++)
            input[j] = new(input[j].Char, info?.Color ?? PaletteIndex.Identifier);

        return i;
    }

    static int TokenizeNumber(Span<Glyph> input)
    {
        int i = 0;
        char c = input[i].Char;

        bool startsWithNumber = char.IsNumber(c);

        if (c != '+' && c != '-' && !startsWithNumber)
            return -1;

        i++;

        bool hasNumber = startsWithNumber;

        // Check for hex prefix 0x
        if (hasNumber && c == '0' && i < input.Length && input[i].Char is 'x' or 'X')
        {
            i++;
            while (i < input.Length && IsHexDigit(input[i].Char))
                i++;

            // Suffix
            while (i < input.Length && input[i].Char is 'u' or 'U' or 'l' or 'L')
                i++;

            for (int j = 0; j < i; j++)
                input[j] = new(input[j].Char, PaletteIndex.Number);

            return i;
        }

        // Check for binary prefix 0b
        if (hasNumber && c == '0' && i < input.Length && input[i].Char is 'b' or 'B')
        {
            i++;
            while (i < input.Length && input[i].Char is '0' or '1' or '_')
                i++;

            while (i < input.Length && input[i].Char is 'u' or 'U' or 'l' or 'L')
                i++;

            for (int j = 0; j < i; j++)
                input[j] = new(input[j].Char, PaletteIndex.Number);

            return i;
        }

        while (i < input.Length && (char.IsNumber(input[i].Char) || input[i].Char == '_'))
        {
            hasNumber = true;
            i++;
        }

        if (!hasNumber)
            return -1;

        bool isFloat = false;

        // Decimal point
        if (i < input.Length && input[i].Char == '.')
        {
            isFloat = true;
            i++;
            while (i < input.Length && (char.IsNumber(input[i].Char) || input[i].Char == '_'))
                i++;
        }

        // Exponent
        if (i < input.Length && input[i].Char is 'e' or 'E')
        {
            isFloat = true;
            i++;

            if (i < input.Length && input[i].Char is '+' or '-')
                i++;

            bool hasDigits = false;
            while (i < input.Length && char.IsNumber(input[i].Char))
            {
                hasDigits = true;
                i++;
            }

            if (!hasDigits)
                return -1;
        }

        // C# numeric suffixes: f, d, m, u, l, ul, lu
        if (i < input.Length && input[i].Char is 'f' or 'F' or 'd' or 'D' or 'm' or 'M')
            i++;
        else if (!isFloat)
        {
            while (i < input.Length && input[i].Char is 'u' or 'U' or 'l' or 'L')
                i++;
        }

        for (int j = 0; j < i; j++)
            input[j] = new(input[j].Char, PaletteIndex.Number);

        return i;
    }

    static bool IsHexDigit(char c) =>
        c is (>= '0' and <= '9') or (>= 'a' and <= 'f') or (>= 'A' and <= 'F');

    static int TokenizePunctuation(Span<Glyph> input)
    {
        // csharpier-ignore-start
        switch (input[0].Char)
        {
            case '[': case ']': case '{': case '}': case '(': case ')': case '-': case '+': case '<': case '>': case '?': case ':':
            case ';': case '!': case '%': case '^': case '&': case '|': case '*': case '/': case '=': case '~': case ',': case '.':
            case '@':
                input[0] = new(input[0].Char, PaletteIndex.Punctuation);
                return 1;

            default:
                return -1;
        }
        // csharpier-ignore-end
    }
}

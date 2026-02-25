using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ImGuiColorTextEditNet.Syntax;

/// <summary>
/// A syntax highlighter that uses regex patterns from a <see cref="LanguageDefinition"/>
/// to tokenize and colorize text. This is a general-purpose highlighter that can work
/// with any language definition that provides TokenRegexStrings.
/// </summary>
public class RegexSyntaxHighlighter : ISyntaxHighlighter
{
    static readonly object DefaultState = new();
    static readonly object MultiLineCommentState = new();

    readonly LanguageDefinition _language;
    readonly List<(Regex Regex, PaletteIndex Color)> _regexList = [];
    readonly SimpleTrie<IdentifierInfo> _identifiers = new();

    record IdentifierInfo(PaletteIndex Color)
    {
        public string Declaration = "";
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RegexSyntaxHighlighter"/> using the
    /// provided language definition for keywords, identifiers, and token regex patterns.
    /// </summary>
    /// <param name="language">The language definition containing syntax rules.</param>
    public RegexSyntaxHighlighter(LanguageDefinition language)
    {
        _language = language ?? throw new ArgumentNullException(nameof(language));

        // Build regex list from language definition
        foreach (var (pattern, color) in language.TokenRegexStrings)
        {
            var options = RegexOptions.Compiled;
            if (!language.CaseSensitive)
                options |= RegexOptions.IgnoreCase;

            _regexList.Add((new Regex(pattern, options), color));
        }

        // Build identifier trie from keywords
        if (language.Keywords != null)
        {
            foreach (var keyword in language.Keywords)
            {
                if (!string.IsNullOrEmpty(keyword))
                    _identifiers.Add(keyword, new IdentifierInfo(PaletteIndex.Keyword));
            }
        }

        // Build identifier trie from known identifiers
        if (language.Identifiers != null)
        {
            foreach (var name in language.Identifiers)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    _identifiers.Add(name, new IdentifierInfo(PaletteIndex.KnownIdentifier)
                    {
                        Declaration = "Built-in function"
                    });
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool AutoIndentation => _language.AutoIndentation;

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
        if (line.Length == 0)
            return state ?? DefaultState;

        // Check if we are in a multi-line comment from a previous line
        bool inMultiLineComment = state == MultiLineCommentState;

        int i = 0;
        while (i < line.Length)
        {
            // Handle multi-line comment continuation or start
            if (inMultiLineComment)
            {
                int commentEnd = FindMultiLineCommentEnd(line, i);
                if (commentEnd != -1)
                {
                    // Found end of comment
                    for (int j = i; j <= commentEnd; j++)
                        line[j] = new Glyph(line[j].Char, PaletteIndex.MultiLineComment);
                    i = commentEnd + 1;
                    inMultiLineComment = false;
                    continue;
                }
                else
                {
                    // Comment continues to end of line
                    for (int j = i; j < line.Length; j++)
                        line[j] = new Glyph(line[j].Char, PaletteIndex.MultiLineComment);
                    return MultiLineCommentState;
                }
            }

            // Skip whitespace
            if (char.IsWhiteSpace(line[i].Char))
            {
                i++;
                continue;
            }

            // Check for multi-line comment start
            if (MatchesAt(line, i, _language.CommentStart))
            {
                int commentEnd = FindMultiLineCommentEnd(line, i + _language.CommentStart.Length);
                if (commentEnd != -1)
                {
                    for (int j = i; j <= commentEnd; j++)
                        line[j] = new Glyph(line[j].Char, PaletteIndex.MultiLineComment);
                    i = commentEnd + 1;
                    continue;
                }
                else
                {
                    for (int j = i; j < line.Length; j++)
                        line[j] = new Glyph(line[j].Char, PaletteIndex.MultiLineComment);
                    return MultiLineCommentState;
                }
            }

            // Check for single-line comment
            if (!string.IsNullOrEmpty(_language.SingleLineComment) && MatchesAt(line, i, _language.SingleLineComment))
            {
                for (int j = i; j < line.Length; j++)
                    line[j] = new Glyph(line[j].Char, PaletteIndex.Comment);
                return DefaultState;
            }

            // Check for preprocessor directive
            if (line[i].Char == _language.PreprocChar && IsFirstNonWhitespace(line, i))
            {
                for (int j = i; j < line.Length; j++)
                    line[j] = new Glyph(line[j].Char, PaletteIndex.Preprocessor);
                return DefaultState;
            }

            // Try regex-based tokenization
            int matched = TryRegexMatch(line, i);
            if (matched > 0)
            {
                i += matched;
                continue;
            }

            // Default: advance one character
            i++;
        }

        return inMultiLineComment ? MultiLineCommentState : DefaultState;
    }

    /// <summary>
    /// Tries to match any of the registered regex patterns at the given position.
    /// If matched, colors the glyphs and returns the match length.
    /// Also resolves identifiers against the keyword/identifier trie.
    /// </summary>
    int TryRegexMatch(Span<Glyph> line, int startIndex)
    {
        // Build substring from startIndex to end of line
        int length = line.Length - startIndex;
        Span<char> chars = length <= 256 ? stackalloc char[length] : new char[length];
        for (int j = 0; j < length; j++)
            chars[j] = line[startIndex + j].Char;

        string text = new(chars);

        foreach (var (regex, color) in _regexList)
        {
            var match = regex.Match(text);
            if (match.Success && match.Index == 0 && match.Length > 0)
            {
                PaletteIndex finalColor = color;

                // If matched as an identifier, check against keyword/identifier trie
                if (color == PaletteIndex.Identifier)
                {
                    var span = line.Slice(startIndex, match.Length);
                    var info = _identifiers.Get<Glyph>(span, x => x.Char);
                    if (info != null)
                        finalColor = info.Color;
                }

                for (int j = 0; j < match.Length; j++)
                    line[startIndex + j] = new Glyph(line[startIndex + j].Char, finalColor);

                return match.Length;
            }
        }

        return 0;
    }

    /// <summary>Checks whether a string pattern matches at the given position in a glyph span.</summary>
    static bool MatchesAt(Span<Glyph> line, int index, string pattern)
    {
        if (index + pattern.Length > line.Length)
            return false;

        for (int j = 0; j < pattern.Length; j++)
        {
            if (line[index + j].Char != pattern[j])
                return false;
        }

        return true;
    }

    /// <summary>Finds the end index of a multi-line comment (pointing to the last char of the end marker).</summary>
    int FindMultiLineCommentEnd(Span<Glyph> line, int startIndex)
    {
        string endMarker = _language.CommentEnd;
        for (int j = startIndex; j <= line.Length - endMarker.Length; j++)
        {
            if (MatchesAt(line, j, endMarker))
                return j + endMarker.Length - 1;
        }

        return -1;
    }

    /// <summary>Checks if the character at index is the first non-whitespace character on the line.</summary>
    static bool IsFirstNonWhitespace(Span<Glyph> line, int index)
    {
        for (int j = 0; j < index; j++)
        {
            if (!char.IsWhiteSpace(line[j].Char))
                return false;
        }

        return true;
    }
}

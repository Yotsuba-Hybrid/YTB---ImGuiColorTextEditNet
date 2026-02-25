---
title: Crear un Highlighter Custom
description: Implementar ISyntaxHighlighter para tokenización avanzada
---

## ¿Cuándo crear un highlighter dedicado?

Usa `RegexSyntaxHighlighter` cuando:

- Tu lenguaje tiene sintaxis simple
- Los regex son suficientes para tokenizar

Crea un highlighter dedicado cuando:

- Necesitas manejar strings con escape sequences complejas
- Tu lenguaje tiene interpolación de strings
- Necesitas manejar contextos anidados (e.g., templates)
- Necesitas máximo rendimiento

## Paso 1: Implementar ISyntaxHighlighter

```csharp
using ImGuiColorTextEditNet;
using ImGuiColorTextEditNet.Syntax;

public class MyHighlighter : ISyntaxHighlighter
{
    readonly LanguageDefinition _lang;
    readonly SimpleTrie<PaletteIndex> _identifiers;

    // Estado para multi-line constructs
    enum State { None, InMultiLineComment, InMultiLineString }

    public MyHighlighter()
    {
        _lang = new LanguageDefinition("MyLang")
        {
            Keywords = ["if", "else", "while", "for", "return"],
            Identifiers = ["print", "input", "log"],
            SingleLineComment = "//",
            CommentStart = "/*",
            CommentEnd = "*/",
            AutoIndentation = true,
        };

        // Construir trie para búsqueda O(n)
        _identifiers = new SimpleTrie<PaletteIndex>();
        foreach (var kw in _lang.Keywords)
            _identifiers.Add(kw, PaletteIndex.Keyword);
        foreach (var id in _lang.Identifiers)
            _identifiers.Add(id, PaletteIndex.KnownIdentifier);
    }

    public bool AutoIndentation => _lang.AutoIndentation;
    public int MaxLinesPerFrame => 1000;
    public string? GetTooltip(string id) => null;

    public object? Colorize(Span<Glyph> line, object? state)
    {
        // ... implementar tokenización
    }
}
```

## Paso 2: Implementar Colorize

```csharp
public object? Colorize(Span<Glyph> line, object? state)
{
    if (line.Length == 0)
        return state;

    var currentState = state as State? ?? State.None;

    int i = 0;
    while (i < line.Length)
    {
        // 1. Manejar state de línea anterior
        if (currentState == State.InMultiLineComment)
        {
            i += ColorizeMultiLineComment(line[i..], ref currentState);
            continue;
        }

        if (currentState == State.InMultiLineString)
        {
            i += ColorizeMultiLineString(line[i..], ref currentState);
            continue;
        }

        // 2. Intentar cada tipo de token
        int consumed;

        if ((consumed = TryColorizeComment(line[i..])) > 0)
        { i += consumed; continue; }

        if ((consumed = TryColorizeMultiLineCommentStart(line[i..], ref currentState)) > 0)
        { i += consumed; continue; }

        if ((consumed = TryColorizeString(line[i..])) > 0)
        { i += consumed; continue; }

        if ((consumed = TryColorizeNumber(line[i..])) > 0)
        { i += consumed; continue; }

        if ((consumed = TryColorizeIdentifier(line[i..])) > 0)
        { i += consumed; continue; }

        // 3. Default: avanzar un carácter
        line[i] = new Glyph(line[i].Char, PaletteIndex.Default);
        i++;
    }

    return currentState == State.None ? null : currentState;
}
```

## Paso 3: Implementar tokenizadores individuales

### Comentarios de una línea

```csharp
int TryColorizeComment(Span<Glyph> span)
{
    if (span.Length < 2) return 0;
    if (span[0].Char != '/' || span[1].Char != '/') return 0;

    // Colorear todo el resto de la línea
    for (int i = 0; i < span.Length; i++)
        span[i] = new Glyph(span[i].Char, PaletteIndex.Comment);

    return span.Length;
}
```

### Strings

```csharp
int TryColorizeString(Span<Glyph> span)
{
    if (span[0].Char != '"') return 0;

    for (int i = 1; i < span.Length; i++)
    {
        char c = span[i].Char;

        // Escape sequence
        if (c == '\\' && i + 1 < span.Length)
        {
            i++; // Saltar el siguiente carácter
            continue;
        }

        // Fin del string
        if (c == '"')
        {
            // Colorear desde 0 hasta i (inclusive)
            for (int j = 0; j <= i; j++)
                span[j] = new Glyph(span[j].Char, PaletteIndex.String);
            return i + 1;
        }
    }

    // String no cerrado — colorear toda la línea
    for (int j = 0; j < span.Length; j++)
        span[j] = new Glyph(span[j].Char, PaletteIndex.String);
    return span.Length;
}
```

### Identificadores con resolución via Trie

```csharp
int TryColorizeIdentifier(Span<Glyph> span)
{
    char c = span[0].Char;
    if (!char.IsLetter(c) && c != '_') return 0;

    // Encontrar el fin del identificador
    int len = 1;
    while (len < span.Length && (char.IsLetterOrDigit(span[len].Char) || span[len].Char == '_'))
        len++;

    // Extraer el texto
    Span<char> word = stackalloc char[len];
    for (int i = 0; i < len; i++)
        word[i] = span[i].Char;

    // Buscar en el trie
    PaletteIndex color = PaletteIndex.Identifier;
    if (_identifiers.Get(word, out var found))
        color = found;

    // Colorear
    for (int i = 0; i < len; i++)
        span[i] = new Glyph(span[i].Char, color);

    return len;
}
```

## Paso 4: Usar tu highlighter

```csharp
editor.SyntaxHighlighter = new MyHighlighter();
```

## Tips de rendimiento

:::tip

- Usa `Span<Glyph>` en vez de crear arrays temporales
- Usa `stackalloc` para buffers pequeños (< 256 chars)
- El `SimpleTrie` es más rápido que `HashSet<string>` para búsquedas de palabras
- Retorna `null` como state cuando no hay estado pendiente (evita comparaciones innecesarias)
  :::

## Siguiente paso

➡️ [Integración con ImGui](/advanced/imgui-integration/) — Cómo integrar el editor en tu aplicación ImGui

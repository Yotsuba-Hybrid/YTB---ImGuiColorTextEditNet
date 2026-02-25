---
title: Syntax Highlighting
description: Cómo funciona el sistema de syntax highlighting y cómo elegir un highlighter
---

## ¿Cómo funciona?

El syntax highlighting se realiza **línea por línea** de forma incremental. Cada frame, el editor procesa un número limitado de líneas (controlado por `MaxLinesPerFrame`) para evitar bloquear el rendering.

### El flujo de colorización

```
1. TextEditorColor.ColorizeIncremental()
   → Se llama cada frame desde el renderer

2. ISyntaxHighlighter.Colorize(Span<Glyph> line, object? state)
   → Recibe una línea como array de Glyphs
   → Modifica cada Glyph asignándole un PaletteIndex
   → Devuelve un "state" para la siguiente línea

3. El state permite multi-line constructs
   → Si una línea termina dentro de un /* comentario */
   → El state indica "MultiLineCommentState"
   → La siguiente línea continúa coloreando como comentario
```

### La estructura Glyph

Cada carácter en el editor es un `Glyph`:

```csharp
public readonly struct Glyph
{
    public readonly char Char;                              // El carácter
    public readonly PaletteIndex ColorIndex;                // Su color
}
```

Un highlighter transforma `Glyph('f', Default)` → `Glyph('f', Keyword)` para la palabra `for`.

## Elegir un highlighter

### Para C#

```csharp
editor.SyntaxHighlighter = new CSharpHighlighter();
```

El `CSharpHighlighter` maneja:

- Keywords de C# (incluye `async`, `await`, `record`, `init`, etc.)
- Strings regulares (`"..."`), verbatim (`@"..."`), e interpolados (`$"..."`)
- Comentarios `//` y `/* */`
- Directivas `#region`, `#if`, etc.
- Números con sufijos de C# (`0.5f`, `100m`, `0xFF`, `0b1010`)
- Identificadores con `@` (`@class`, `@event`)

### Para C/C++

```csharp
// C++
editor.SyntaxHighlighter = new CStyleHighlighter(useCpp: true);

// C
editor.SyntaxHighlighter = new CStyleHighlighter(useCpp: false);
```

### Para otros lenguajes (HLSL, GLSL, SQL, Lua)

```csharp
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Hlsl());
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Glsl());
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Sql());
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Lua());
```

### Sin highlighting

```csharp
editor.SyntaxHighlighter = NullSyntaxHighlighter.Instance;
```

## La interfaz ISyntaxHighlighter

Todos los highlighters implementan esta interfaz:

```csharp
public interface ISyntaxHighlighter
{
    bool AutoIndentation { get; }          // ¿Soporta auto-indent?
    int MaxLinesPerFrame { get; }          // Líneas por frame (performance)
    string? GetTooltip(string id);         // Tooltip al hacer hover
    object Colorize(Span<Glyph> line, object? state);  // Colorizar una línea
}
```

| Propiedad          | Descripción                                        | Valor típico          |
| ------------------ | -------------------------------------------------- | --------------------- |
| `AutoIndentation`  | Si el editor debe auto-indentar al presionar Enter | `true`                |
| `MaxLinesPerFrame` | Cuántas líneas procesar por frame                  | `1000`                |
| `GetTooltip`       | Devuelve un tooltip para un identificador          | `"Built-in function"` |
| `Colorize`         | Modifica los glyphs y devuelve el state            | Multi-line state      |

## Cambiar highlighter en runtime

Puedes cambiar el highlighter en cualquier momento:

```csharp
if (ImGui.Button("C#"))
    editor.SyntaxHighlighter = new CSharpHighlighter();

if (ImGui.Button("SQL"))
    editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Sql());

if (ImGui.Button("Plain Text"))
    editor.SyntaxHighlighter = NullSyntaxHighlighter.Instance;
```

Al cambiar el highlighter, el editor automáticamente re-coloriza todo el texto.

## Siguiente paso

➡️ [Lenguajes Disponibles](/tutorials/languages/) — Detalle de cada lenguaje incluido

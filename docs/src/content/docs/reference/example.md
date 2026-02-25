---
title: TextEditor API
description: Referencia completa de la API pública de TextEditor
---

## TextEditor

Clase principal del editor. Punto de entrada para toda la funcionalidad.

### Propiedades

| Propiedad           | Tipo                       | Descripción                        |
| ------------------- | -------------------------- | ---------------------------------- |
| `AllText`           | `string`                   | Todo el texto del editor (get/set) |
| `TextLines`         | `IList<string>`            | Líneas de texto como strings       |
| `TotalLines`        | `int`                      | Número total de líneas             |
| `CursorPosition`    | `Coordinates`              | Posición del cursor (Line, Column) |
| `SyntaxHighlighter` | `ISyntaxHighlighter`       | Highlighter activo                 |
| `Options`           | `TextEditorOptions`        | Opciones de configuración          |
| `KeyboardInput`     | `ITextEditorKeyboardInput` | Handler de teclado                 |
| `MouseInput`        | `ITextEditorMouseInput`    | Handler de mouse                   |
| `Selection`         | `TextEditorSelection`      | Estado de selección                |
| `Movement`          | `TextEditorMovement`       | Navegación del cursor              |
| `Renderer`          | `TextEditorRenderer`       | Componente de rendering            |
| `Version`           | `long`                     | Versión incremental del texto      |
| `UndoCount`         | `int`                      | Número de acciones en undo stack   |
| `UndoIndex`         | `int`                      | Posición actual en undo stack      |

### Métodos

| Método                                        | Retorno  | Descripción                                         |
| --------------------------------------------- | -------- | --------------------------------------------------- |
| `Render(string title)`                        | `bool`   | Renderiza el editor. Retorna `true` si hubo cambios |
| `Render(string title, Vector2 size)`          | `bool`   | Renderiza con tamaño específico                     |
| `AppendLine(string text)`                     | `void`   | Agrega línea al final                               |
| `AppendLine(string text, PaletteIndex color)` | `void`   | Agrega línea coloreada                              |
| `Undo()`                                      | `void`   | Deshace la última acción                            |
| `Redo()`                                      | `void`   | Rehace la acción deshecha                           |
| `SetBreakpoints(HashSet<int>)`                | `void`   | Establece breakpoints                               |
| `SetErrorMarkers(Dictionary<int, string>)`    | `void`   | Establece errores                                   |
| `SetColor(PaletteIndex, uint)`                | `void`   | Cambia un color (ABGR)                              |
| `ScrollToLine(int line)`                      | `void`   | Scroll hasta una línea                              |
| `GetCurrentLineText()`                        | `string` | Texto de la línea actual                            |
| `SerializeState()`                            | `string` | Serializa estado a JSON                             |

### Eventos

| Evento              | Args                         | Descripción                      |
| ------------------- | ---------------------------- | -------------------------------- |
| `BreakpointRemoved` | `BreakpointRemovedEventArgs` | Breakpoint eliminado por edición |

---

## TextEditorOptions

| Propiedad            | Tipo   | Default | Descripción          |
| -------------------- | ------ | ------- | -------------------- |
| `IsReadOnly`         | `bool` | `false` | Modo solo lectura    |
| `IsColorizerEnabled` | `bool` | `true`  | Activar highlighting |
| `IsOverwrite`        | `bool` | `false` | Modo sobreescritura  |
| `TabSize`            | `int`  | `4`     | Ancho de tab         |

---

## ISyntaxHighlighter

```csharp
public interface ISyntaxHighlighter
{
    bool AutoIndentation { get; }
    int MaxLinesPerFrame { get; }
    string? GetTooltip(string id);
    object? Colorize(Span<Glyph> line, object? state);
}
```

---

## Implementaciones

### CSharpHighlighter

```csharp
new CSharpHighlighter()
```

Highlighter dedicado para C#. No requiere parámetros.

### CStyleHighlighter

```csharp
new CStyleHighlighter(bool useCpp)
```

| Parámetro        | Descripción                |
| ---------------- | -------------------------- |
| `useCpp = true`  | Keywords y features de C++ |
| `useCpp = false` | Keywords y features de C   |

### RegexSyntaxHighlighter

```csharp
new RegexSyntaxHighlighter(LanguageDefinition lang)
```

Highlighter genérico basado en regex. Funciona con cualquier `LanguageDefinition`.

### NullSyntaxHighlighter

```csharp
NullSyntaxHighlighter.Instance
```

Sin highlighting. Singleton.

---

## LanguageDefinition

| Propiedad           | Tipo                           | Descripción                        |
| ------------------- | ------------------------------ | ---------------------------------- |
| `Name`              | `string`                       | Nombre del lenguaje                |
| `Keywords`          | `string[]`                     | Palabras reservadas                |
| `Identifiers`       | `string[]`                     | Identificadores conocidos          |
| `TokenRegexStrings` | `List<(string, PaletteIndex)>` | Patrones regex                     |
| `SingleLineComment` | `string`                       | Delimitador de comentario de línea |
| `CommentStart`      | `string`                       | Inicio de comentario multi-línea   |
| `CommentEnd`        | `string`                       | Fin de comentario multi-línea      |
| `PreprocChar`       | `char`                         | Carácter de preprocesador          |
| `CaseSensitive`     | `bool`                         | Sensible a mayúsculas              |
| `AutoIndentation`   | `bool`                         | Auto-indent habilitado             |

### Factory Methods

| Método                        | Lenguaje |
| ----------------------------- | -------- |
| `LanguageDefinition.CSharp()` | C#       |
| `LanguageDefinition.Hlsl()`   | HLSL     |
| `LanguageDefinition.Glsl()`   | GLSL     |
| `LanguageDefinition.Sql()`    | SQL      |
| `LanguageDefinition.Lua()`    | Lua      |

---

## Coordinates

```csharp
public readonly struct Coordinates(int line, int column)
{
    public readonly int Line;    // 0-indexed
    public readonly int Column;  // 0-indexed
}
```

---

## Glyph

```csharp
public readonly struct Glyph(char c, PaletteIndex colorIndex)
{
    public readonly char Char;
    public readonly PaletteIndex ColorIndex;
}
```

---

## PaletteIndex

```csharp
public enum PaletteIndex
{
    Default, Keyword, Number, String, CharLiteral, Punctuation,
    Preprocessor, Identifier, KnownIdentifier, PreprocIdentifier,
    Comment, MultiLineComment, Background, Cursor, Selection,
    ErrorMarker, ControlCharacter, Breakpoint, LineNumber,
    CurrentLineFill, CurrentLineFillInactive, CurrentLineEdge,
    LineEdited, LineEditedSaved, LineEditedReverted, Custom, Max
}
```

---

## Palettes

| Paleta           | Descripción           |
| ---------------- | --------------------- |
| `Palettes.Dark`  | Tema oscuro (default) |
| `Palettes.Light` | Tema claro            |
| `Palettes.Blue`  | Tema retro azul       |

Uso: `Palettes.Dark.Apply(editor);`

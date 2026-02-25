---
title: Arquitectura Interna
description: Cómo está diseñado internamente el TextEditor
---

## Diagrama de componentes

```
TextEditor (fachada principal)
├── TextEditorText          → Almacén de líneas (List<Line>)
│   └── Line                → Lista de Glyphs
│       └── Glyph           → char + PaletteIndex
├── TextEditorColor         → Colorización incremental
│   ├── ISyntaxHighlighter  → Estrategia de highlighting
│   └── SimpleTrie          → Trie de keywords/identifiers
├── TextEditorSelection     → Cursor + selección
├── TextEditorMovement      → Navegación del cursor
├── TextEditorRenderer      → Rendering vía ImGui
├── TextEditorModify        → Operaciones de edición (estático)
├── TextEditorUndoStack     → Pila de undo/redo
│   └── UndoRecord          → Operaciones agrupadas
├── TextEditorBreakpoints   → Breakpoints por línea
├── TextEditorErrorMarkers  → Errores por línea
├── TextEditorOptions       → Configuración
├── ITextEditorKeyboardInput → Input de teclado
└── ITextEditorMouseInput    → Input de mouse
```

## Flujo de un frame

Cada vez que llamas `editor.Render()`, sucede lo siguiente:

```
1. HandleKeyboardInputs()
   → Procesa keybindings (Ctrl+Z, Tab, etc.)
   → Procesa caracteres escritos

2. HandleMouseInputs()
   → Click, doble click, selección con drag

3. ColorizeIncremental()
   → Procesa N líneas (MaxLinesPerFrame)
   → Llama a ISyntaxHighlighter.Colorize() por línea
   → Almacena state entre líneas

4. RenderEditor()
   → Dibuja números de línea
   → Dibuja breakpoints y errores
   → Dibuja cada Glyph con su color
   → Dibuja cursor y selección
   → Dibuja indicadores de edición
```

## El sistema de Glyphs

Todo el texto se almacena como `List<Line>`, donde cada `Line` es una lista de `Glyph`:

```csharp
// Un Glyph = carácter + color
public readonly struct Glyph(char c, PaletteIndex colorIndex)
{
    public readonly char Char = c;
    public readonly PaletteIndex ColorIndex = colorIndex;
}
```

Cuando el highlighter procesa una línea, **modifica in-place** los `PaletteIndex` de cada Glyph:

```
Antes:  [('f', Default), ('o', Default), ('r', Default)]
Después: [('f', Keyword), ('o', Keyword), ('r', Keyword)]
```

## El sistema de estado (State)

El método `Colorize` devuelve un `object?` que representa el estado al final de la línea. Este estado se pasa como input a la siguiente línea:

```csharp
object? Colorize(Span<Glyph> line, object? previousLineState);
```

Esto permite manejar constructs multi-línea:

```
Línea 1: "/* inicio del"     → state = MultiLineComment
Línea 2: "   comentario"     → recibe state, colorea todo como Comment
Línea 3: "   fin */"         → colorea hasta */, devuelve state = null
Línea 4: "int x = 5;"        → recibe null, colorea normal
```

## El sistema de undo/redo

Cada operación de edición se registra como un `UndoRecord`:

```
UndoRecord
├── List<IEditorOperation> Operations
│   ├── AddLineOperation     → Agregar una línea
│   ├── RemoveLineOperation  → Eliminar una línea
│   └── ModifyLineOperation  → Modificar glyphs de una línea
└── CursorPosition (before/after)
```

Las operaciones se agrupan en un `MetaOperation` cuando son parte de la misma acción del usuario (por ejemplo, pegar texto multi-línea crea múltiples operaciones atómicas).

## SimpleTrie

El `SimpleTrie<T>` es una estructura de datos eficiente para buscar keywords:

```csharp
var trie = new SimpleTrie<PaletteIndex>();

// Agregar
trie.Add("for", PaletteIndex.Keyword);
trie.Add("Console", PaletteIndex.KnownIdentifier);

// Buscar
if (trie.Get(text.AsSpan(), out var result))
{
    // result = PaletteIndex.Keyword (si text == "for")
}
```

La búsqueda es O(n) donde n es la longitud de la palabra, independiente del número de keywords registradas.

## Siguiente paso

➡️ [Crear un Highlighter](/advanced/custom-highlighter/) — Implementa tu propio ISyntaxHighlighter

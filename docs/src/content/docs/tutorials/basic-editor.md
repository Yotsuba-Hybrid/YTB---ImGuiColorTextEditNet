---
title: Editor Básico
description: Configuración detallada del TextEditor y sus opciones
---

## Creación del editor

El `TextEditor` es la clase principal. Al instanciarlo se inicializan todos los subsistemas:

```csharp
var editor = new TextEditor();
```

Internamente, esto crea:

- `TextEditorText` — almacén de texto (líneas de Glyphs)
- `TextEditorSelection` — manejo de selección y cursor
- `TextEditorColor` — sistema de colorización incremental
- `TextEditorRenderer` — rendering vía ImGui
- `TextEditorUndoStack` — pila de undo/redo
- `TextEditorMovement` — navegación del cursor

## Opciones del editor

Accede a las opciones vía `editor.Options`:

```csharp
// Modo solo lectura
editor.Options.IsReadOnly = true;

// Desactivar el colorizador
editor.Options.IsColorizerEnabled = false;

// Modo sobreescritura (Insert key)
editor.Options.IsOverwrite = false;

// Tamaño de tabulación
editor.Options.TabSize = 4;
```

## Trabajar con texto

### Texto completo

```csharp
// Establecer todo el texto
editor.AllText = @"public class Foo
{
    public int Bar { get; set; }
}";

// Leer todo el texto
string code = editor.AllText;
```

### Líneas individuales

```csharp
// Obtener todas las líneas
IList<string> lines = editor.TextLines;

// Establecer líneas
editor.TextLines = new[] { "línea 1", "línea 2" };

// Agregar líneas al final
editor.AppendLine("// nueva línea");

// Línea actual del cursor
string currentLine = editor.GetCurrentLineText();
```

### Líneas con color personalizado

```csharp
// Primero definir colores custom
editor.SetColor(PaletteIndex.Custom, 0xff0000ff);      // Rojo (ABGR)
editor.SetColor(PaletteIndex.Custom + 1, 0xff00ffff);  // Amarillo

// Agregar líneas con esos colores
editor.AppendLine("ERROR: algo falló", PaletteIndex.Custom);
editor.AppendLine("WARN: revisar esto", PaletteIndex.Custom + 1);
```

:::note[Formato de color]
Los colores usan formato **ABGR** (Alpha-Blue-Green-Red), no ARGB. Es el formato nativo de ImGui. Por ejemplo:

- `0xff0000ff` = Rojo opaco
- `0xff00ff00` = Verde opaco
- `0xffff0000` = Azul opaco
  :::

## Cursor y navegación

```csharp
// Leer posición del cursor
Coordinates pos = editor.CursorPosition;
Console.WriteLine($"Línea {pos.Line}, Columna {pos.Column}");

// Mover el cursor programáticamente
editor.CursorPosition = new Coordinates(10, 0); // Línea 11, columna 1

// Scroll a una línea
editor.ScrollToLine(50);
```

## Undo / Redo

```csharp
// Deshacer última acción
editor.Undo();

// Rehacer
editor.Redo();

// Verificar estado
int undoCount = editor.UndoCount;    // Cuántas acciones hay en el stack
int undoIndex = editor.UndoIndex;    // Posición actual en el stack
```

## Detectar cambios

El método `Render` devuelve `true` si el texto fue modificado:

```csharp
if (editor.Render("Editor"))
{
    // El usuario cambió algo
    OnCodeChanged(editor.AllText);
}
```

También puedes usar la propiedad `Version`:

```csharp
long lastVersion = editor.Version;

// ... más tarde ...
if (editor.Version != lastVersion)
{
    // Hubo cambios
    lastVersion = editor.Version;
}
```

## Serialización del estado

Guarda y restaura el estado completo del editor:

```csharp
// Serializar a JSON
string json = editor.SerializeState();

// El JSON incluye:
// - Options (ReadOnly, TabSize, etc.)
// - Selection (cursor, start, end)
// - Breakpoints
// - ErrorMarkers
// - Text (todas las líneas)
```

## Siguiente paso

➡️ [Syntax Highlighting](/tutorials/syntax-highlighting/) — Cómo funciona y cómo configurarlo

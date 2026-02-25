---
title: Temas y Colores
description: Personalizar la paleta de colores del editor
---

## Paletas predefinidas

El editor incluye tres paletas de colores:

```csharp
// Dark mode (por defecto)
Palettes.Dark

// Light mode
Palettes.Light

// Retro blue (reminiscente de Turbo Pascal)
Palettes.Blue
```

### Aplicar una paleta

```csharp
// Establecer tema dark
Palettes.Dark.Apply(editor);

// Establecer tema light
Palettes.Light.Apply(editor);

// Establecer tema retro
Palettes.Blue.Apply(editor);
```

## El sistema PaletteIndex

Cada elemento visual del editor tiene un `PaletteIndex` que mapea a un color:

```csharp
public enum PaletteIndex
{
    Default,              // Texto normal
    Keyword,              // Palabras reservadas (if, for, while)
    Number,               // Literales numéricos (42, 3.14f)
    String,               // Cadenas de texto ("hello")
    CharLiteral,          // Literales de carácter ('a')
    Punctuation,          // Símbolos (+, -, {, })
    Preprocessor,         // Directivas (#include, #define)
    Identifier,           // Identificadores desconocidos
    KnownIdentifier,      // Identificadores conocidos (Console, Math)
    PreprocIdentifier,    // Identificadores de preprocesador
    Comment,              // Comentarios (// y /* */)
    MultiLineComment,     // Comentarios multi-línea
    Background,           // Fondo del editor
    Cursor,               // Color del cursor
    Selection,            // Color de selección
    ErrorMarker,          // Fondo de errores
    ControlCharacter,     // Caracteres de control
    Breakpoint,           // Fondo de breakpoints
    LineNumber,           // Números de línea
    CurrentLineFill,      // Fondo de línea actual
    CurrentLineFillInactive,  // Fondo de línea (sin foco)
    CurrentLineEdge,      // Borde de línea actual
    LineEdited,           // Indicador de línea editada
    LineEditedSaved,      // Indicador de línea guardada
    LineEditedReverted,   // Indicador de línea revertida
    Custom,               // Primer color personalizado
    // Custom + 1, Custom + 2, ... para más colores
}
```

## Cambiar colores individuales

```csharp
// Cambiar el color de los keywords a naranja brillante
editor.SetColor(PaletteIndex.Keyword, 0xff00aaff);  // ABGR

// Cambiar el fondo a gris oscuro
editor.SetColor(PaletteIndex.Background, 0xff1e1e1e);

// Cambiar el color de los strings a verde
editor.SetColor(PaletteIndex.String, 0xff33cc33);

// Cambiar el color de los comentarios a gris
editor.SetColor(PaletteIndex.Comment, 0xff888888);
editor.SetColor(PaletteIndex.MultiLineComment, 0xff888888);
```

:::caution[Formato ABGR]
ImGui usa colores en formato **ABGR**, no ARGB:

- `0xAABBGGRR` donde AA=Alpha, BB=Blue, GG=Green, RR=Red
- Ejemplo: Rojo = `0xFF0000FF`, Verde = `0xFF00FF00`, Azul = `0xFFFF0000`
  :::

## Crear una paleta personalizada

```csharp
void ApplyMonokaiTheme(TextEditor editor)
{
    editor.SetColor(PaletteIndex.Default,           0xfff8f8f2);  // Blanco cálido
    editor.SetColor(PaletteIndex.Keyword,           0xffff6188);  // Rosa
    editor.SetColor(PaletteIndex.Number,            0xffab9df2);  // Púrpura
    editor.SetColor(PaletteIndex.String,            0xffffd866);  // Amarillo
    editor.SetColor(PaletteIndex.CharLiteral,       0xffffd866);
    editor.SetColor(PaletteIndex.Punctuation,       0xfff8f8f2);
    editor.SetColor(PaletteIndex.Preprocessor,      0xffff6188);
    editor.SetColor(PaletteIndex.Identifier,        0xfff8f8f2);
    editor.SetColor(PaletteIndex.KnownIdentifier,   0xffa9dc76);  // Verde
    editor.SetColor(PaletteIndex.Comment,           0xff727072);  // Gris
    editor.SetColor(PaletteIndex.MultiLineComment,  0xff727072);
    editor.SetColor(PaletteIndex.Background,        0xff2d2a2e);  // Fondo oscuro
    editor.SetColor(PaletteIndex.Cursor,            0xfffcfcfa);
    editor.SetColor(PaletteIndex.Selection,         0x80515052);
    editor.SetColor(PaletteIndex.ErrorMarker,       0x80ff5555);
    editor.SetColor(PaletteIndex.Breakpoint,        0x40ff6188);
    editor.SetColor(PaletteIndex.LineNumber,        0xff5b595c);
    editor.SetColor(PaletteIndex.CurrentLineFill,   0x10ffffff);
    editor.SetColor(PaletteIndex.CurrentLineEdge,   0x30ffffff);
}
```

## Colores personalizados (Custom)

Usa `PaletteIndex.Custom` y valores mayores para tus propios colores:

```csharp
// Definir colores custom
editor.SetColor(PaletteIndex.Custom,     0xff00ffff);  // Cyan
editor.SetColor(PaletteIndex.Custom + 1, 0xffff8800);  // Naranja
editor.SetColor(PaletteIndex.Custom + 2, 0xffff00ff);  // Magenta

// Usar en líneas
editor.AppendLine("INFO:  Todo bien", PaletteIndex.Custom);
editor.AppendLine("WARN:  Revisar esto", PaletteIndex.Custom + 1);
editor.AppendLine("ERROR: Falló algo", PaletteIndex.Custom + 2);
```

## Ejemplo: Selector de tema en runtime

```csharp
string[] themes = { "Dark", "Light", "Blue", "Monokai" };
int currentTheme = 0;

// En tu loop de ImGui:
if (ImGui.Combo("Tema", ref currentTheme, themes, themes.Length))
{
    switch (currentTheme)
    {
        case 0: Palettes.Dark.Apply(editor); break;
        case 1: Palettes.Light.Apply(editor); break;
        case 2: Palettes.Blue.Apply(editor); break;
        case 3: ApplyMonokaiTheme(editor); break;
    }
}
```

## Siguiente paso

➡️ [Input y Keybindings](/tutorials/input-keybindings/) — Personalizar los controles del editor

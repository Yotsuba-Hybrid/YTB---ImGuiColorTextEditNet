---
title: Quick Start
description: Tu primer editor de código funcionando en minutos
---

## El editor más simple

Solo necesitas 3 líneas para tener un editor funcional:

```csharp
using ImGuiColorTextEditNet;

// 1. Crear el editor
var editor = new TextEditor();

// 2. Ponerle contenido
editor.AllText = "Console.WriteLine(\"Hello World!\");";

// 3. Renderizar en tu loop de ImGui
editor.Render("MiEditor");
```

## Ejemplo completo con ImGui

Este es un ejemplo realista dentro de un loop de ImGui:

```csharp
using System.Numerics;
using ImGuiColorTextEditNet;
using ImGuiColorTextEditNet.Syntax;
using ImGuiNET;

// Crear el editor con syntax highlighting para C#
var editor = new TextEditor
{
    AllText = @"using System;

namespace MiApp;

public class Program
{
    public static void Main(string[] args)
    {
        // Saludo al usuario
        var nombre = ""Mundo"";
        Console.WriteLine($""¡Hola, {nombre}!"");

        for (int i = 0; i < 10; i++)
            Console.Write($""{i} "");
    }
}",
    SyntaxHighlighter = new CSharpHighlighter(),
};

// En tu loop de rendering:
void OnImGuiRender()
{
    ImGui.Begin("Editor de Código");

    // Mostrar info del cursor
    var pos = editor.CursorPosition;
    ImGui.Text($"Línea: {pos.Line + 1} | Columna: {pos.Column + 1}");
    ImGui.Separator();

    // Renderizar el editor
    editor.Render("CodeEditor", new Vector2(-1, -1));

    ImGui.End();
}
```

## ¿Qué estoy viendo?

Cuando el editor se renderiza, verás:

- **Números de línea** a la izquierda
- **Syntax highlighting** con colores para keywords, strings, comentarios, etc.
- **Cursor parpadeante** con soporte para navegación con teclado
- **Selección de texto** con mouse o Shift+flechas

## Operaciones básicas del editor

```csharp
// Leer todo el texto
string code = editor.AllText;

// Reemplazar todo el texto
editor.AllText = "nuevo código aquí";

// Leer/escribir líneas individualmente
IList<string> lines = editor.TextLines;
editor.TextLines = new[] { "línea 1", "línea 2", "línea 3" };

// Posición del cursor
var cursor = editor.CursorPosition;
editor.CursorPosition = new Coordinates(5, 0); // Ir a línea 6, columna 1

// Undo / Redo
editor.Undo();
editor.Redo();

// Agregar líneas al final
editor.AppendLine("// Nueva línea");
editor.AppendLine("// Línea con color", PaletteIndex.Custom);

// Verificar si el texto cambió
bool changed = editor.Render("Editor");
if (changed)
    Console.WriteLine("El usuario modificó el texto");

// Serializar el estado completo a JSON
string json = editor.SerializeState();

// Scroll a una línea específica
editor.ScrollToLine(42);
```

## Siguiente paso

➡️ [Editor Básico](/tutorials/basic-editor/) — Configuración detallada del editor

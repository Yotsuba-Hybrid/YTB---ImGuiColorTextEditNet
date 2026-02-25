---
title: Integración con ImGui
description: Cómo integrar ImGuiColorTextEditNet en una aplicación Dear ImGui existente
---

## Prerequisitos

Antes de usar ImGuiColorTextEditNet necesitas:

1. Una aplicación con **Dear ImGui** renderizando vía **ImGui.NET**
2. Un backend de rendering (Veldrid, OpenTK, Silk.NET, DirectX, OpenGL, etc.)
3. **.NET 8.0** o superior

## Enfoque básico

El editor se integra como cualquier otro widget de ImGui:

```csharp
using ImGuiColorTextEditNet;
using ImGuiColorTextEditNet.Syntax;
using ImGuiNET;
using System.Numerics;

// Crear el editor UNA VEZ (no en el loop)
TextEditor editor = new()
{
    AllText = File.ReadAllText("Program.cs"),
    SyntaxHighlighter = new CSharpHighlighter(),
};

// En tu loop de rendering:
void OnImGuiFrame()
{
    ImGui.Begin("Code Editor");

    // size = (-1, -1) para llenar todo el espacio disponible
    bool changed = editor.Render("editor1", new Vector2(-1, -1));

    if (changed)
        Console.WriteLine("Text was modified");

    ImGui.End();
}
```

## Ejemplo con Veldrid (completo)

Este es un setup completo usando Veldrid como backend:

```csharp
using System.Numerics;
using ImGuiColorTextEditNet;
using ImGuiColorTextEditNet.Syntax;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

// Crear ventana y dispositivo gráfico
VeldridStartup.CreateWindowAndGraphicsDevice(
    new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "Code Editor"),
    new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
    out Sdl2Window window,
    out GraphicsDevice gd);

// Crear controlador de ImGui
var imguiRenderer = new ImGuiRenderer(
    gd, gd.MainSwapchain.Framebuffer.OutputDescription,
    window.Width, window.Height);

// Crear editor
var editor = new TextEditor
{
    AllText = @"using System;

public class HelloWorld
{
    public static void Main()
    {
        Console.WriteLine(""Hello, World!"");
    }
}",
    SyntaxHighlighter = new CSharpHighlighter(),
};

// Main loop
while (window.Exists)
{
    var input = window.PumpEvents();
    imguiRenderer.Update(1f / 60f, input);

    ImGui.Begin("Editor", ImGuiWindowFlags.NoCollapse);

    // Info bar
    var pos = editor.CursorPosition;
    ImGui.Text($"Ln {pos.Line + 1}, Col {pos.Column + 1} | {editor.TotalLines} lines");
    ImGui.Separator();

    // Editor widget
    editor.Render("code", new Vector2(-1, -1));

    ImGui.End();

    var cl = gd.ResourceFactory.CreateCommandList();
    cl.Begin();
    cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
    cl.ClearColorTarget(0, RgbaFloat.Black);
    imguiRenderer.Render(gd, cl);
    cl.End();
    gd.SubmitCommands(cl);
    gd.SwapBuffers(gd.MainSwapchain);
    cl.Dispose();
}
```

## Múltiples editores

Puedes tener múltiples editores en la misma ventana:

```csharp
var csharpEditor = new TextEditor
{
    AllText = csharpCode,
    SyntaxHighlighter = new CSharpHighlighter(),
};

var sqlEditor = new TextEditor
{
    AllText = sqlQuery,
    SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Sql()),
};

// En el loop:
if (ImGui.BeginTabBar("editors"))
{
    if (ImGui.BeginTabItem("C# Code"))
    {
        csharpEditor.Render("csharp_editor");
        ImGui.EndTabItem();
    }

    if (ImGui.BeginTabItem("SQL Query"))
    {
        sqlEditor.Render("sql_editor");
        ImGui.EndTabItem();
    }

    ImGui.EndTabBar();
}
```

:::caution[IDs únicos]
Cada editor necesita un **ID único** en `Render()`. Si usas el mismo ID para dos editores, ImGui confundirá sus estados internos.
:::

## Layout con paneles

Un layout tipo IDE con panel lateral:

```csharp
// Panel izquierdo - File tree
ImGui.BeginChild("file_tree", new Vector2(200, -1), ImGuiChildFlags.Border);
if (ImGui.Selectable("Program.cs", selectedFile == 0))
{
    selectedFile = 0;
    editor.AllText = File.ReadAllText("Program.cs");
    editor.SyntaxHighlighter = new CSharpHighlighter();
}
if (ImGui.Selectable("queries.sql", selectedFile == 1))
{
    selectedFile = 1;
    editor.AllText = File.ReadAllText("queries.sql");
    editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Sql());
}
ImGui.EndChild();

// Panel derecho - Editor
ImGui.SameLine();
ImGui.BeginChild("editor_panel", new Vector2(-1, -1));

// Status bar
var pos = editor.CursorPosition;
ImGui.Text($"Ln {pos.Line + 1}  Col {pos.Column + 1}");
ImGui.SameLine(ImGui.GetWindowWidth() - 100);
ImGui.Text($"{editor.TotalLines} lines");
ImGui.Separator();

// Editor
editor.Render("main_editor", new Vector2(-1, -1));
ImGui.EndChild();
```

## Fuentes monoespaciadas

Para que el editor se vea bien, usa una fuente monoespaciada:

```csharp
var io = ImGui.GetIO();
io.Fonts.AddFontFromFileTTF("SpaceMono-Regular.ttf", 16.0f);
// O usa otra fuente mono: JetBrains Mono, Fira Code, Cascadia Code, etc.
```

:::tip
El proyecto demo incluye `SpaceMono-Regular.ttf` como referencia.
:::

---
title: Instalación
description: Cómo agregar ImGuiColorTextEditNet a tu proyecto .NET
---

## Requisitos previos

- **.NET 8.0** o superior
- Un proyecto con **ImGui.NET** ya configurado (por ejemplo, usando Veldrid, OpenTK, Silk.NET, etc.)

## Instalación via NuGet

```bash
dotnet add package ImGuiColorTextEditNet
```

## Instalación desde código fuente

Si quieres modificar la librería (como en este fork):

```bash
git clone https://github.com/Yotsuba-Hybrid/YTB---ImGuiColorTextEditNet.git
```

Luego agrega una referencia al proyecto en tu `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\path\to\TextEdit\ImGuiColorTextEditNet.csproj" />
</ItemGroup>
```

## Dependencias

La librería solo depende de:

| Paquete     | Versión  | Propósito                        |
| ----------- | -------- | -------------------------------- |
| `ImGui.NET` | 1.91.6.1 | Bindings de Dear ImGui para .NET |

No tiene dependencias nativas ni de rendering — eso lo maneja tu aplicación host.

## Verificar la instalación

Agrega este código mínimo para verificar que todo funciona:

```csharp
using ImGuiColorTextEditNet;

var editor = new TextEditor();
editor.AllText = "// Hello World!";
Console.WriteLine($"Editor creado con {editor.TotalLines} línea(s)");
```

## Estructura del proyecto

```
ImGuiColorTextEditNet/
├── Editor/          → Componentes del editor (rendering, selection, movement, etc.)
├── Input/           → Manejo de teclado y mouse
├── Operations/      → Sistema de undo/redo
├── Syntax/          → Highlighters y definiciones de lenguaje
├── TextEditor.cs    → Clase principal (punto de entrada)
├── Glyph.cs         → Carácter + color
├── PaletteIndex.cs  → Enum de colores
└── Palettes.cs      → Paletas predefinidas (Dark, Light, etc.)
```

## Siguiente paso

➡️ [Quick Start](/guides/quick-start/) — Tu primer editor funcional en 3 pasos

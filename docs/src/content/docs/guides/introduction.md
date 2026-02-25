---
title: Introducción
description: ¿Qué es ImGuiColorTextEditNet y por qué usarlo?
---

## ¿Qué es ImGuiColorTextEditNet?

**ImGuiColorTextEditNet** es un componente de editor de texto multi-línea para [Dear ImGui](https://github.com/ocornut/imgui) (vía [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET)) escrito completamente en C#. Proporciona syntax highlighting, undo/redo, selección de texto, breakpoints, error markers y más.

Es un port del proyecto original [ImGuiColorTextEdit](https://github.com/BalazsJako/ImGuiColorTextEdit) de C++ a .NET, con mejoras significativas en la arquitectura y nuevos lenguajes.

## Características

- ✅ **Syntax highlighting** para C#, C, C++, HLSL, GLSL, SQL y Lua
- ✅ **Sistema extensible** — crea tus propios lenguajes con regex o tokenización dedicada
- ✅ **Undo / Redo** completo con stack de operaciones
- ✅ **Breakpoints** con soporte para datos asociados
- ✅ **Error markers** para mostrar errores inline
- ✅ **Selección de texto** — click, doble click para palabra, triple click para línea
- ✅ **Keybindings** configurables (Ctrl+C, Ctrl+V, Ctrl+Z, Tab indent, etc.)
- ✅ **Colores personalizables** — paleta completa con 20+ índices de color
- ✅ **Read-only mode** para visualización de código
- ✅ **Serialización** del estado del editor a JSON
- ✅ **100% managed C#** — sin dependencias nativas

## Arquitectura del sistema de highlighting

El editor usa un patrón de estrategia para el syntax highlighting:

```
TextEditor
  └── ISyntaxHighlighter (interfaz)
        ├── CStyleHighlighter       → C y C++ (tokenización dedicada)
        ├── CSharpHighlighter       → C# (tokenización dedicada)
        ├── RegexSyntaxHighlighter  → Cualquier lenguaje via LanguageDefinition
        └── NullSyntaxHighlighter   → Sin highlighting
```

Los highlighters dedicados (`CStyleHighlighter`, `CSharpHighlighter`) usan tokenización carácter por carácter para máxima precisión. El `RegexSyntaxHighlighter` es genérico y usa expresiones regulares definidas en `LanguageDefinition`.

## ¿Para quién es esta librería?

- **Desarrolladores de herramientas** que usan ImGui para crear editores, IDEs, o herramientas de debugging
- **Desarrolladores de game engines** que necesitan un editor de código integrado
- **Proyectos de scripting** que necesitan un editor con highlighting para su DSL

## Siguiente paso

➡️ [Instalación](/guides/installation/) — Configura tu proyecto con ImGuiColorTextEditNet

---
title: Lenguajes Disponibles
description: Todos los lenguajes soportados con detalle de keywords e identifiers
---

## Resumen

| Lenguaje | Highlighter                | Case Sensitive | Auto Indent | Comentarios    |
| -------- | -------------------------- | :------------: | :---------: | -------------- |
| C#       | `CSharpHighlighter`        |       ✅       |     ✅      | `//` `/* */`   |
| C        | `CStyleHighlighter(false)` |       ✅       |     ✅      | `//` `/* */`   |
| C++      | `CStyleHighlighter(true)`  |       ✅       |     ✅      | `//` `/* */`   |
| HLSL     | `RegexSyntaxHighlighter`   |       ✅       |     ✅      | `//` `/* */`   |
| GLSL     | `RegexSyntaxHighlighter`   |       ✅       |     ✅      | `//` `/* */`   |
| SQL      | `RegexSyntaxHighlighter`   |       ❌       |     ❌      | `//` `/* */`   |
| Lua      | `RegexSyntaxHighlighter`   |       ✅       |     ❌      | `--` `--[[ ]]` |

## C\#

```csharp
editor.SyntaxHighlighter = new CSharpHighlighter();
```

**Tokenización dedicada** con soporte completo para:

- **85+ keywords**: `abstract`, `as`, `async`, `await`, `base`, `bool`, `break`, `byte`, `case`, `catch`, `char`, `checked`, `class`, `const`, `continue`, `decimal`, `default`, `delegate`, `do`, `double`, `else`, `enum`, `event`, `explicit`, `extern`, `false`, `finally`, `fixed`, `float`, `for`, `foreach`, `goto`, `if`, `implicit`, `in`, `int`, `interface`, `internal`, `is`, `lock`, `long`, `namespace`, `new`, `null`, `object`, `operator`, `out`, `override`, `params`, `private`, `protected`, `public`, `readonly`, `ref`, `return`, `sbyte`, `sealed`, `short`, `sizeof`, `stackalloc`, `static`, `string`, `struct`, `switch`, `this`, `throw`, `true`, `try`, `typeof`, `uint`, `ulong`, `unchecked`, `unsafe`, `ushort`, `using`, `virtual`, `void`, `volatile`, `while`, `async`, `await`, `var`, `dynamic`, `yield`, `nameof`, `when`, `where`, `record`, `init`, `required`, ...

- **100+ identifiers**: `Console`, `Math`, `String`, `List`, `Dictionary`, `Task`, `Span`, `File`, `DateTime`, `Exception`, `Enumerable`, `Action`, `Func`, etc.

- **Strings especiales**: `@"verbatim"`, `$"interpolated"`, `$@"both"`, `@$"both"`

- **Números**: `0xFF`, `0b1010`, `1.5f`, `100m`, `42UL`

## C / C++

```csharp
// C++
editor.SyntaxHighlighter = new CStyleHighlighter(useCpp: true);

// C
editor.SyntaxHighlighter = new CStyleHighlighter(useCpp: false);
```

**Tokenización dedicada** con soporte para:

- Keywords de C11 o C++17 según el modo
- Strings, char literals, números (hex, binary, float)
- Directivas de preprocesador (`#include`, `#define`, etc.)
- std library identifiers (`printf`, `std::vector`, etc.)

## HLSL

```csharp
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Hlsl());
```

Incluye todos los tipos de datos HLSL (`float4`, `matrix`, `Texture2D`, etc.), funciones intrínsecas (`abs`, `dot`, `lerp`, `normalize`, etc.), y keywords de shading.

## GLSL

```csharp
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Glsl());
```

Keywords y funciones de OpenGL Shading Language.

## SQL

```csharp
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Sql());
```

:::note
SQL es **case insensitive** — tanto `SELECT` como `select` se resaltan como keyword.
:::

Incluye keywords SQL estándar (`SELECT`, `FROM`, `WHERE`, `JOIN`, etc.) y funciones Oracle/SQL Server.

## Lua

```csharp
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(LanguageDefinition.Lua());
```

:::note
Lua usa `--` para comentarios de una línea y `--[[ ]]` para multi-línea.
:::

Keywords de Lua 5.3+ y todas las librerías estándar (`coroutine`, `table`, `io`, `os`, `string`, `math`, `debug`, etc.).

## Siguiente paso

➡️ [Lenguajes Personalizados](/tutorials/custom-languages/) — Crea tu propio lenguaje

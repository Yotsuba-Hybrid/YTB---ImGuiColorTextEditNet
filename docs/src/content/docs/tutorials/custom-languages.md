---
title: Lenguajes Personalizados
description: Crea tus propios lenguajes con LanguageDefinition y RegexSyntaxHighlighter
---

## Crear un lenguaje con regex

El `RegexSyntaxHighlighter` funciona con cualquier `LanguageDefinition`. Puedes crear tu propio lenguaje personalizando keywords, identifiers y patrones regex.

### Paso 1: Definir el lenguaje

```csharp
var jsonLang = new LanguageDefinition("JSON")
{
    // JSON no tiene keywords, pero si quieres resaltar true/false/null:
    Keywords = ["true", "false", "null"],

    // No tiene identifiers conocidos
    Identifiers = [],

    // JSON no tiene comentarios
    SingleLineComment = "",
    CommentStart = "",
    CommentEnd = "",

    // No tiene preprocesador
    PreprocChar = '\0',

    // Case sensitive
    CaseSensitive = true,

    // Auto indent (por las llaves)
    AutoIndentation = true,
};
```

### Paso 2: Agregar patrones de tokens (regex)

Los patrones se evalúan en orden. El primer match gana.

```csharp
// Strings JSON (con escape sequences)
jsonLang.TokenRegexStrings.Add((@"""(?:[^""\\]|\\.)*""", PaletteIndex.String));

// Números (enteros y decimales, con signo opcional)
jsonLang.TokenRegexStrings.Add((@"-?[0-9]+\.?[0-9]*(?:[eE][+-]?[0-9]+)?", PaletteIndex.Number));

// Identifiers (keys antes de los dos puntos)
jsonLang.TokenRegexStrings.Add((@"[a-zA-Z_][a-zA-Z0-9_]*", PaletteIndex.Identifier));

// Puntuación
jsonLang.TokenRegexStrings.Add((@"[{}\[\]:,]", PaletteIndex.Punctuation));
```

### Paso 3: Usar con RegexSyntaxHighlighter

```csharp
editor.SyntaxHighlighter = new RegexSyntaxHighlighter(jsonLang);
```

## Ejemplo completo: DSL de configuración

Imaginemos un DSL para configurar un juego:

```csharp
var gameDSL = new LanguageDefinition("GameConfig")
{
    Keywords = [
        "entity", "component", "system", "world",
        "spawn", "destroy", "update",
        "if", "else", "while", "for", "return",
        "true", "false", "null",
        "int", "float", "string", "bool", "vec2", "vec3"
    ],
    Identifiers = [
        "Player", "Enemy", "Bullet", "Transform",
        "Velocity", "Health", "Sprite", "Collider",
        "Input", "Physics", "Renderer", "Audio"
    ],
    SingleLineComment = "//",
    CommentStart = "/*",
    CommentEnd = "*/",
    PreprocChar = '@',
    CaseSensitive = true,
    AutoIndentation = true,
};

// Strings
gameDSL.TokenRegexStrings.Add((@"""[^""]*""", PaletteIndex.String));

// Números con sufijos
gameDSL.TokenRegexStrings.Add((@"[0-9]+\.?[0-9]*[fF]?", PaletteIndex.Number));

// Directivas @import, @module, etc.
gameDSL.TokenRegexStrings.Add((@"@[a-zA-Z_]+", PaletteIndex.Preprocessor));

// Identifiers
gameDSL.TokenRegexStrings.Add((@"[a-zA-Z_][a-zA-Z0-9_]*", PaletteIndex.Identifier));

// Operadores
gameDSL.TokenRegexStrings.Add((@"[+\-*/=<>!&|^~%]", PaletteIndex.Punctuation));

editor.SyntaxHighlighter = new RegexSyntaxHighlighter(gameDSL);
```

## Cómo funciona el matching

El `RegexSyntaxHighlighter` procesa cada línea así:

1. **Verifica state** — ¿Estamos dentro de un comentario multi-línea?
2. **Verifica preprocesador** — ¿La línea empieza con `PreprocChar`?
3. **Verifica comentario de línea** — ¿Empieza con `SingleLineComment`?
4. **Verifica comentario multi-línea** — ¿Empieza con `CommentStart`?
5. **Intenta cada regex** — En orden, intenta matchear patrones de `TokenRegexStrings`
6. **Resuelve identifiers** — Si un match es `PaletteIndex.Identifier`, busca en el `SimpleTrie` si es keyword o identifier conocido
7. **Default** — Si nada matchea, avanza un carácter con `PaletteIndex.Default`

### La resolución de identifiers

Cuando un regex matchea `PaletteIndex.Identifier`, el highlighter no lo deja así. Busca el texto en un `SimpleTrie`:

```
"for"    → encontrado en Keywords   → PaletteIndex.Keyword
"Player" → encontrado en Identifiers → PaletteIndex.KnownIdentifier
"myVar"  → no encontrado            → PaletteIndex.Identifier
```

## Complejidad de regex

:::caution[Rendimiento]
Los regex son evaluados en **cada posición** de cada línea. Evita patrones complejos con backreferences o lookaheads excesivos. Los patrones simples y anclados rinden mejor.
:::

| Patrón              | Performance | Nota                |
| ------------------- | :---------: | ------------------- | ---------------- |
| `[0-9]+`            |  ⚡ Rápida  | Patrón simple       |
| `"[^"]*"`           |  ⚡ Rápida  | Sin backtracking    |
| `"(?:[^"\\]         |  \\.)\*"`   | 🟡 Moderada         | Escape sequences |
| `(?<=\bclass\s)\w+` |  🔴 Lenta   | Lookbehind — evitar |

## Siguiente paso

➡️ [Breakpoints y Errores](/tutorials/breakpoints-errors/) — Agregar breakpoints y error markers

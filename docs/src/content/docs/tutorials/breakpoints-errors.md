---
title: Breakpoints y Error Markers
description: Cómo usar breakpoints y mostrar errores inline en el editor
---

## Breakpoints

Los breakpoints se muestran como indicadores visuales en el margen izquierdo del editor, similar a un IDE.

### Establecer breakpoints

```csharp
// Establecer breakpoints en líneas específicas (0-indexed)
editor.SetBreakpoints(new HashSet<int> { 5, 10, 15 });
```

### Leer breakpoints

```csharp
// Obtener breakpoints asociados al editor
var breakpoints = editor.Breakpoints;
```

### Evento de breakpoint eliminado

Cuando el usuario borra texto que contiene un breakpoint, se dispara un evento:

```csharp
editor.BreakpointRemoved += (sender, args) =>
{
    Console.WriteLine($"Breakpoint eliminado en línea {args.Line}");
    // Aquí puedes actualizar tu depurador
};
```

### Ejemplo: Toggle de breakpoints

```csharp
void ToggleBreakpoint(TextEditor editor, int line)
{
    var breakpoints = new HashSet<int>(editor.Breakpoints.Select(b => b.Key));

    if (breakpoints.Contains(line))
        breakpoints.Remove(line);
    else
        breakpoints.Add(line);

    editor.SetBreakpoints(breakpoints);
}
```

## Error Markers

Los error markers muestran errores inline con subrayado rojo y tooltip al hacer hover.

### Establecer errores

```csharp
// Diccionario de línea → mensaje de error
var errors = new Dictionary<int, string>
{
    { 3, "CS1002: ; expected" },
    { 7, "CS0103: The name 'foo' does not exist" },
    { 12, "CS0246: The type 'Bar' could not be found" }
};

editor.SetErrorMarkers(errors);
```

### Limpiar errores

```csharp
// Quitar todos los errores
editor.SetErrorMarkers(new Dictionary<int, string>());
```

### Ejemplo: Validación en vivo

```csharp
void ValidateCode(TextEditor editor)
{
    var errors = new Dictionary<int, string>();
    var lines = editor.TextLines;

    for (int i = 0; i < lines.Count; i++)
    {
        string line = lines[i].TrimEnd();

        // Ejemplo: verificar que las líneas terminen en ;
        if (line.Length > 0 && !line.EndsWith(";") && !line.EndsWith("{")
            && !line.EndsWith("}") && !line.StartsWith("//")
            && !line.StartsWith("using") && !line.StartsWith("namespace"))
        {
            errors[i] = "Posible ; faltante al final de la línea";
        }
    }

    editor.SetErrorMarkers(errors);
}

// Llamar después de cada cambio
if (editor.Render("Editor"))
    ValidateCode(editor);
```

## Combinando breakpoints y errores

Los breakpoints y errores se muestran simultáneamente:

```csharp
var editor = new TextEditor
{
    AllText = sampleCode,
    SyntaxHighlighter = new CSharpHighlighter()
};

// Breakpoints en líneas 5 y 10
editor.SetBreakpoints(new HashSet<int> { 5, 10 });

// Error en línea 7
editor.SetErrorMarkers(new Dictionary<int, string>
{
    { 7, "Error de compilación aquí" }
});
```

En el rendering verás:

- 🔴 **Punto rojo** en el margen para breakpoints
- 🟥 **Resaltado rojo** de fondo para líneas con error
- **Tooltip** al hacer hover sobre una línea con error

## Siguiente paso

➡️ [Temas y Colores](/tutorials/themes-colors/) — Personalizar la apariencia del editor

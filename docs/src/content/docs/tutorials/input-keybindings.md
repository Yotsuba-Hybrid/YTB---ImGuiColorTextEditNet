---
title: Input y Keybindings
description: Personalizar controles de teclado y mouse del editor
---

## Keybindings por defecto

El editor viene con keybindings estándar:

### Navegación

| Keybinding                   | Acción                |
| ---------------------------- | --------------------- |
| `↑` `↓` `←` `→`              | Mover cursor          |
| `Ctrl + ←` / `Ctrl + →`      | Mover por palabra     |
| `Home` / `End`               | Inicio/fin de línea   |
| `Ctrl + Home` / `Ctrl + End` | Inicio/fin de archivo |
| `Page Up` / `Page Down`      | Scroll por página     |

### Selección

| Keybinding                     | Acción                                  |
| ------------------------------ | --------------------------------------- |
| `Shift + ↑↓←→`                 | Seleccionar con cursor                  |
| `Ctrl + Shift + ←→`            | Seleccionar por palabra                 |
| `Shift + Home` / `Shift + End` | Seleccionar hasta inicio/fin de línea   |
| `Ctrl + Shift + Home/End`      | Seleccionar hasta inicio/fin de archivo |
| `Ctrl + A`                     | Seleccionar todo                        |

### Edición

| Keybinding    | Acción                    |
| ------------- | ------------------------- |
| `Ctrl + C`    | Copiar                    |
| `Ctrl + V`    | Pegar                     |
| `Ctrl + X`    | Cortar                    |
| `Ctrl + Z`    | Deshacer                  |
| `Ctrl + Y`    | Rehacer                   |
| `Delete`      | Borrar adelante           |
| `Backspace`   | Borrar atrás              |
| `Enter`       | Nueva línea               |
| `Tab`         | Indentar (o insertar tab) |
| `Shift + Tab` | Desindentar               |
| `Insert`      | Toggle sobreescritura     |

## Personalizar keybindings

### Agregar un binding nuevo

```csharp
var keyboard = (StandardKeyboardInput)editor.KeyboardInput;

// Binding de solo lectura (funciona aunque el editor sea read-only)
keyboard.AddReadOnlyBinding("Ctrl + S", editor =>
{
    string code = editor.AllText;
    File.WriteAllText("code.cs", code);
    Console.WriteLine("Guardado!");
});

// Binding que modifica texto (se ignora en read-only)
keyboard.AddMutatingBinding("Ctrl + D", editor =>
{
    // Duplicar línea actual
    string line = editor.GetCurrentLineText();
    int lineIndex = editor.CursorPosition.Line;
    // ... lógica para duplicar
});
```

### Binding con contexto

```csharp
keyboard.AddBinding("F5", myDebugger, (editor, context) =>
{
    var debugger = (MyDebugger)context!;
    debugger.Run(editor.AllText);
    return true; // true = consumió el evento
});
```

### Limpiar todos los bindings

```csharp
keyboard.ClearBindings();
// Luego agrega solo los que necesites
```

### Formato de keybindings

Los strings de keybinding soportan:

```
"Ctrl + S"         → Ctrl + S
"Shift + Tab"      → Shift + Tab
"Ctrl + Shift + F" → Ctrl + Shift + F
"Delete"           → Delete key
"F5"               → F5 key
"Enter"            → Enter key
```

Los modificadores soportados son `Ctrl` y `Shift`. Se separan con `+` y espacios opcionales.

## Mouse Input

El editor también maneja input de mouse:

- **Click** — Posicionar cursor
- **Doble click** — Seleccionar palabra
- **Triple click** — Seleccionar línea
- **Click + Drag** — Seleccionar rango

El mouse input se maneja vía `StandardMouseInput` y generalmente no necesita configuración.

## Modo Colemak

El editor soporta layout Colemak — el `CapsLock` actúa como `Backspace`:

```csharp
var keyboard = (StandardKeyboardInput)editor.KeyboardInput;
keyboard.ColemakMode = true;
```

## Ejemplo: Editor tipo IDE

```csharp
var keyboard = (StandardKeyboardInput)editor.KeyboardInput;

// Guardar
keyboard.AddReadOnlyBinding("Ctrl + S", e =>
{
    File.WriteAllText("output.cs", e.AllText);
});

// Compilar
keyboard.AddReadOnlyBinding("F5", e =>
{
    Compile(e.AllText);
});

// Toggle breakpoint en cursor
keyboard.AddMutatingBinding("F9", e =>
{
    ToggleBreakpoint(e, e.CursorPosition.Line);
});

// Ir a línea
keyboard.AddMutatingBinding("Ctrl + G", e =>
{
    // Mostrar diálogo "Ir a línea"
    showGoToDialog = true;
});
```

## Siguiente paso

➡️ [Arquitectura Interna](/advanced/architecture/) — Entender el diseño del editor

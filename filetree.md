# ImGuiColorTextEditNet - File Tree

```
YTB---ImGuiColorTextEditNet/
├── .github/
├── .gitignore
├── LICENSE
├── README.md
├── TextEdit.sln
├── switcher.json
└── src/
    ├── Directory.Build.props
    ├── TextEdit/
    │   ├── ImGuiColorTextEditNet.csproj
    │   ├── BreakpointRemovedEventArgs.cs
    │   ├── Coordinates.cs
    │   ├── Glyph.cs
    │   ├── Line.cs
    │   ├── PaletteIndex.cs
    │   ├── Palettes.cs
    │   ├── SelectionMode.cs
    │   ├── SelectionState.cs
    │   ├── SimpleCache.cs
    │   ├── SimpleTrie.cs
    │   ├── TextEditor.cs
    │   ├── Util.cs
    │   ├── Editor/
    │   │   ├── TextEditorBreakpoints.cs
    │   │   ├── TextEditorColor.cs
    │   │   ├── TextEditorErrorMarkers.cs
    │   │   ├── TextEditorModify.cs
    │   │   ├── TextEditorMovement.cs
    │   │   ├── TextEditorOptions.cs
    │   │   ├── TextEditorRenderer.cs
    │   │   ├── TextEditorSelection.cs
    │   │   ├── TextEditorText.cs
    │   │   └── TextEditorUndoStack.cs
    │   ├── Input/
    │   │   ├── EditorKeybind.cs
    │   │   ├── EditorKeybindAction.cs
    │   │   ├── ITextEditorKeyboardInput.cs
    │   │   ├── ITextEditorMouseInput.cs
    │   │   ├── StandardKeyboardInput.cs
    │   │   └── StandardMouseInput.cs
    │   ├── Operations/
    │   │   ├── AddLineOperation.cs
    │   │   ├── IEditorOperation.cs
    │   │   ├── MetaOperation.cs
    │   │   ├── ModifyLineOperation.cs
    │   │   ├── RemoveLineOperation.cs
    │   │   └── UndoRecord.cs
    │   └── Syntax/
    │       ├── CSharpHighlighter.cs      ← NEW
    │       ├── CStyleHighlighter.cs
    │       ├── ISyntaxHighlighter.cs
    │       ├── LanguageDefinition.cs      ← MODIFIED (added CSharp())
    │       ├── NullSyntaxHighlighter.cs
    │       └── RegexSyntaxHighlighter.cs  ← REWRITTEN (C++ → C#)
    ├── TextEdit.Demo/
    │   ├── TextEdit.Demo.csproj
    │   ├── Program.cs
    │   ├── ImGuiController.cs
    │   ├── SpaceMono-Regular.ttf
    │   └── Shaders/
    └── TextEdit.Tests/
        ├── TextEdit.Tests.csproj
        ├── BasicTests.cs
        ├── BreakpointTests.cs
        ├── ClipboardTests.cs
        ├── DeletionTests.cs
        ├── ErrorTests.cs
        ├── InsertionTests.cs
        ├── MovementTests.cs
        ├── SelectionTests.cs
        └── UndoHelper.cs
```

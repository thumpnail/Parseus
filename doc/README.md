# Rust-ähnliches Diagnostic System für Parseus

Ein modernes, hochwertiges Error- und Warning-Report-System für den Parseus-Parser, inspiriert von Rusts einzigartiger Diagnostic-Ausgabe.

## Features

### 🎨 Visuelle Fehlerdarstellung
- **Farbige Konsolen-Ausgabe** mit automatischer TTY-Erkennung
- **Code-Snippets** mit Kontext um die Fehlerposition
- **Visuelle Marker** unter fehlerhaften Zeichen (ähnlich Rust)
- **Multi-line Fehler** mit durchgehenden Markierungen
- **Zeile/Spalte Information** in 1-basierten Indizes (wie Standard-Tools)

### 📊 Diagnostic Levels
- **Error** (🔴): Fatale Fehler, die die Compilation stoppen
- **Warning** (🟡): Potenzielle Probleme, die beachtet werden sollten
- **Note** (🔵): Zusätzliche Informationen zum Fehler
- **Help** (🟢): Vorschläge zur Behebung des Problems

### ⚡ Performance
- **LineColumnCache**: Vorberechnung von Zeile/Spalte für schnelle Lookups
- **Lazy Diagnostics**: Fehler werden gesammelt, nicht sofort ausgegeben
- **Effiziente String-Behandlung**: Minimal Memory-Overhead

### 🔧 Integration
- **Einfache API**: Nur wenige Methoden zum Lernen
- **Automatische Position-Tracking**: Token-Positionen werden automatisch erfasst
- **Flexible Verwendung**: Kann in bestehende Parser eingebaut werden

## Verwendung

### Grundlegende Fehlerbehandlung

```csharp
// Parser-Kontext erstellen
var lexResult = lexer.Lex(sourceCode);
var parserCtx = new BasicAParserContext(lexResult);
parserCtx.SetSourceCode(sourceCode);

var state = new CancellationState();
var ctx = new BaseParserContext(parserCtx, state);

// Fehler melden
BaseParser.ReportError(ctx, "expected identifier");
BaseParser.ReportWarning(ctx, "unused variable");

// Diagnostics ausgeben
BaseParser.OutputDiagnostics(ctx);
Console.WriteLine($"error: {BaseParser.GetDiagnosticSummary(ctx)}");
```

### Manuelle Diagnostic-Erstellung

```csharp
var source = @"let x = 10
let y = x +";

var diag = new Diagnostic(
    new DiagnosticMessage(DiagnosticLevel.Error, "unexpected end of expression"),
    "script.nano"
)
.WithSourceCode(source)
.WithMessage(DiagnosticLevel.Note, "expected operand after '+'")
.WithMessage(DiagnosticLevel.Help, "try adding a number like '5'");

DiagnosticRenderer.Output(diag);
```

### Mehrere Diagnostics auf einmal

```csharp
var diagnostics = state.Diagnostics;
DiagnosticRenderer.OutputAll(diagnostics);

var summary = DiagnosticRenderer.GetSummary(diagnostics);
Console.WriteLine($"error: {summary}");
```

## Ausgabe-Beispiele

### Fehler mit Context
```
error: input
  unexpected end of expression: expected operand after '+'

  2 | let y = x +
    |             ^
    |
note: expected operand after '+'
help: try adding a number like '5'
```

### Warning
```
warning: script.nano
  unused variable: 'foo'

  3 | let foo = 42;
    |     ^^^
    |
note: if this is intentional, prefix with '_'
help: consider renaming to '_foo'
```

### Multi-line Fehler
```
error: parser.nano
  missing closing parenthesis

  5 | fn calculate(
    |    ^^^^^^^^^^
  6 | x: int,
    | ^^^^^^^
  7 | y: int)
    | ^^^^^^
```

## API-Referenz

### Klassen

#### `DiagnosticLevel` (Enum)
```csharp
public enum DiagnosticLevel {
    Error,      // 0 - Stoppender Fehler
    Warning,    // 1 - Warnung
    Note,       // 2 - Information
    Help        // 3 - Hinweis
}
```

#### `TextSpan` (Record)
Repräsentiert eine Spanne von Text in der Quellcode.
```csharp
public record TextSpan(int StartIndex, int Length, TextLocation StartLocation, TextLocation? EndLocation);

// Konstruktoren
TextSpan.At(index)              // Single character
TextSpan.Range(start, end)      // From start to end (inclusive)
```

#### `TextLocation` (Record)
Zeile und Spalte eines Zeichens in der Quellcode.
```csharp
public record TextLocation(int Line, int Column, int Index);
```

#### `DiagnosticMessage` (Record)
Eine einzelne Diagnostic-Nachricht.
```csharp
public record DiagnosticMessage(DiagnosticLevel Level, string Text, TextSpan? Span);
```

#### `Diagnostic` (Klasse)
Komplette Diagnostic mit Primary-Message und Related-Messages.
```csharp
public class Diagnostic {
    public DiagnosticMessage Message { get; set; }
    public List<DiagnosticMessage> RelatedMessages { get; set; }
    public string? SourceCode { get; set; }
    public string? SourceLabel { get; set; }
    
    public Diagnostic WithMessage(DiagnosticLevel level, string text, TextSpan? span = null);
    public Diagnostic WithSourceCode(string source);
}
```

#### `CancellationState` (Erweiterungen)
```csharp
public class CancellationState {
    public List<Diagnostic> Diagnostics { get; }
    public bool HasDiagnostics { get; }
    public bool HasErrors { get; }
    public bool HasWarnings { get; }
    
    public void ReportDiagnostic(DiagnosticMessage message, string? sourceLabel = null);
    public void ReportError(string message, TextSpan? span = null, string? sourceLabel = null);
    public void ReportWarning(string message, TextSpan? span = null, string? sourceLabel = null);
    public void ReportNote(string message, TextSpan? span = null, string? sourceLabel = null);
}
```

#### `BasicAParserContext` (Erweiterungen)
```csharp
public class BasicAParserContext {
    public string? SourceCode { get; }
    public LineColumnCache? LineCache { get; }
    
    public void SetSourceCode(string source);
    public TextSpan GetSpanAt(int tokenIndex);
    public TextSpan GetCurrentSpan();
    public TextSpan GetSpanBetween(int startTokenIndex, int endTokenIndex);
}
```

#### `DiagnosticRenderer` (Static)
```csharp
public static class DiagnosticRenderer {
    public static string Render(Diagnostic diag, RenderOptions? options = null);
    public static string RenderAll(IEnumerable<Diagnostic> diagnostics, RenderOptions? options = null);
    public static void Output(Diagnostic diag, RenderOptions? options = null);
    public static void OutputAll(IEnumerable<Diagnostic> diagnostics, RenderOptions? options = null);
    public static string GetSummary(IEnumerable<Diagnostic> diagnostics);
}
```

#### `BaseParser` (Statische Helper-Methoden)
```csharp
public abstract class BaseParser {
    protected internal static void ReportError(BaseParserContext ctx, string message, string? sourceLabel = null);
    protected internal static void ReportWarning(BaseParserContext ctx, string message, string? sourceLabel = null);
    protected internal static void ReportNote(BaseParserContext ctx, string message, string? sourceLabel = null);
    protected internal static void SetSourceCode(BaseParserContext ctx, string source);
    protected internal static void OutputDiagnostics(BaseParserContext ctx, DiagnosticRenderer.RenderOptions? options = null);
    protected internal static string GetDiagnosticSummary(BaseParserContext ctx);
}
```

## Erweiterte Konfiguration

### RenderOptions
```csharp
public class RenderOptions {
    public bool? UseColors { get; set; }       // null = Auto-detect TTY
    public int ContextLines { get; set; }      // 2 (default)
    public int MaxWidth { get; set; }          // 120 (default)
}

// Verwendung:
DiagnosticRenderer.Output(diag, new RenderOptions {
    UseColors = false,      // Farben deaktivieren
    ContextLines = 3,       // Mehr Context-Zeilen
    MaxWidth = 80           // Schmalere Ausgabe
});
```

## Beispiele

Siehe `DiagnosticExample.cs` für umfassende Demos:

1. **DemoBothDiagnosticLevels()** - Zeigt alle Diagnostic-Level mit Formatierung
2. **DemoDiagnosticCollection()** - Zeigt Fehler-Sammlung während des Parsing
3. **DemoParserIntegration()** - Zeigt Integration in reale Parser

Ausführen:
```csharp
DiagnosticExample.RunAllDemos();
```

## Struktur

```
Parseus/src/Parser/Diagnostics/
├── DiagnosticLevel.cs          # Enum für Severity-Level
├── TextLocation.cs             # Zeile/Spalte Information
├── Diagnostic.cs               # Haupt-Diagnostic-Klasse + Cache
└── DiagnosticRenderer.cs       # Rendering-Engine

Parseus/src/Parser/Common/
├── CancellationState.cs        # Erweitert mit Diagnostics
├── BasicAParserContext.cs      # Erweitert mit Source-Code-Tracking
└── ParseException.cs           # Erweitert mit Diagnostic-Support

Parseus/src/Parser/Implicit/
└── Parser.cs                   # Erweitert mit Helper-Methoden

Parseus/src/Example/
└── DiagnosticExample.cs        # Umfangreiche Beispiele

Parseus/src/Tests/
└── DiagnosticTests.cs          # Unit-Tests
```

## Performance-Überlegungen

### LineColumnCache
Das System precomputes Zeile/Spalte-Offsets bei der Quellcode-Setzung:
- **Einmalig**: O(n) bei SetSourceCode()
- **Pro-Lookup**: O(log n) binary search
- **Speicher**: ~2-3 Bytes pro Zeile

Für eine 10.000-Zeilen-Datei: ~25-30 KB Cache.

### Diagnostic-Sammlung
- Fehler werden **nicht sofort ausgegeben**, sondern gesammelt
- Dies ermöglicht bessere UX mit Multiple-Error-Reports
- Ideal für IDE-Integration und Batch-Processing

## Integration mit bestehenden Parsern

Um das System zu integrieren:

1. **Quellcode setzen**:
```csharp
var ctx = new BasicAParserContext(tokens);
ctx.SetSourceCode(sourceCode);
```

2. **Parser-Fehler melden**:
```csharp
if (!ctx.Context.MatchToken("IDENTIFIER")) {
    BaseParser.ReportError(parserCtx, "expected identifier");
    return;
}
```

3. **Diagnostics ausgeben**:
```csharp
BaseParser.OutputDiagnostics(parserCtx);
if (parserCtx.State.HasErrors) {
    Console.WriteLine($"error: {BaseParser.GetDiagnosticSummary(parserCtx)}");
}
```

## Testing

Unit-Tests sind in `DiagnosticTests.cs`:

```bash
dotnet test Parseus.sln --filter "DiagnosticTests"
```

Tests decken ab:
- TextSpan-Erstellung
- LineColumnCache-Funktionalität
- Diagnostic-Erstellung und -Sammlung
- Rendering verschiedener Level
- Parser-Integration
- Multi-line Diagnostics

## Lizenz

Teil des Parseus-Projekts.


# Diagnostic System - Quick Reference

## 🚀 Schnelle Integration (3 Zeilen Code)

```csharp
var ctx = new BasicAParserContext(tokens);
ctx.SetSourceCode(source);  // ← Quellcode speichern
BaseParser.SetSourceCode(parserCtx, source);  // ← oder so
```

## 📝 Fehler Melden

```csharp
// Mit automatischer Position
BaseParser.ReportError(ctx, "message");
BaseParser.ReportWarning(ctx, "message");  
BaseParser.ReportNote(ctx, "message");

// Mit manueller Position
var span = TextSpan.At(index);
state.ReportError("message", span);

// Mit Zeilenbereich
var span = TextSpan.Range(start, end);
state.ReportError("message", span);
```

## 🎨 Ausgeben

```csharp
// Einzeln
DiagnosticRenderer.Output(diagnostic);

// Alle sammeln
DiagnosticRenderer.OutputAll(state.Diagnostics);

// Summary
Console.WriteLine($"error: {DiagnosticRenderer.GetSummary(state.Diagnostics)}");

// Mit Optionen
DiagnosticRenderer.Output(diagnostic, new() { 
    UseColors = false,
    ContextLines = 3 
});
```

## 📊 Status Prüfen

```csharp
state.HasErrors        // true wenn Fehler vorhanden
state.HasWarnings      // true wenn Warnungen vorhanden
state.HasDiagnostics   // true wenn irgendwas vorhanden
state.Diagnostics.Count  // Anzahl Diagnostics
```

## 📍 Span Ermitteln

```csharp
// Aktueller Token
TextSpan span = ctx.GetCurrentSpan();

// Bestimmter Token
TextSpan span = ctx.GetSpanAt(tokenIndex);

// Bereich
TextSpan span = ctx.GetSpanBetween(startIdx, endIdx);

// Manuell
TextSpan span = TextSpan.At(5);           // Position 5, Länge 1
TextSpan span = TextSpan.Range(5, 10);    // Von 5 bis 10 (inclusive)
```

## 🏗️ Diagnostic Bauen

```csharp
var diag = new Diagnostic(
    new DiagnosticMessage(DiagnosticLevel.Error, "main message"),
    "file.nano"
)
.WithSourceCode(source)
.WithMessage(DiagnosticLevel.Note, "additional info")
.WithMessage(DiagnosticLevel.Help, "suggestion");

DiagnosticRenderer.Output(diag);
```

## 🔍 Demo Starten

```csharp
// Alle Beispiele ansehen
DiagnosticExample.RunAllDemos();

// Einzelne Beispiele
DiagnosticExample.DemoBothDiagnosticLevels();
DiagnosticExample.DemoDiagnosticCollection();
DiagnosticExample.DemoParserIntegration();
```

## ✅ Tests Starten

```csharp
DiagnosticTests.RunAllTests();
```

## 📋 Checkliste für Integration

- [ ] `ctx.SetSourceCode(source)` nach Context-Erstellung
- [ ] `BaseParser.ReportError/Warning/Note()` statt Exception werfen
- [ ] `BaseParser.OutputDiagnostics(ctx)` am Ende aufrufen
- [ ] `HasErrors` prüfen für Exit-Code
- [ ] Optional: Custom `RenderOptions` für Formatting

## 🎯 Beispiel-Vollständig

```csharp
// 1. Lexen
var lexer = new Lexer()...;
var lexResult = lexer.Lex(sourceCode);

// 2. Parser-Context erstellen
var parserCtx = new BasicAParserContext(lexResult);
parserCtx.SetSourceCode(sourceCode);

// 3. Parser-State vorbereiten
var state = new CancellationState();
var ctx = new BaseParserContext(parserCtx, state);

// 4. Parsen und Fehler sammeln
while (parserCtx.HasMoreTokens()) {
    if (!parserCtx.MatchToken("EXPECTED")) {
        BaseParser.ReportError(ctx, "expected EXPECTED");
        break;
    }
    // ... parsing logic
}

// 5. Diagnostics ausgeben
BaseParser.OutputDiagnostics(ctx);

// 6. Status prüfen
if (state.HasErrors) {
    var summary = BaseParser.GetDiagnosticSummary(ctx);
    Console.WriteLine($"error: {summary}");
    return 1;
}

return 0;
```

## 🎨 Diagnostic Levels

| Level | Farbe | Wann |
|-------|-------|------|
| Error | Red | Stoppender Fehler |
| Warning | Yellow | Potentielles Problem |
| Note | Cyan | Extra-Info |
| Help | Green | Lösungs-Vorschlag |

## 💾 Speicherort der Dateien

```
Parseus/src/Parser/Diagnostics/     ← Neue Klassen
  DiagnosticLevel.cs
  TextLocation.cs
  Diagnostic.cs
  DiagnosticRenderer.cs
  README.md

Parseus/src/Parser/Common/          ← Erweiterte Klassen
  CancellationState.cs (✓ updated)
  BasicAParserContext.cs (✓ updated)
  ParseException.cs (✓ updated)

Parseus/src/Parser/Implicit/        ← Helper-Methoden
  Parser.cs (✓ updated)

Parseus/src/Example/
  DiagnosticExample.cs              ← Demos
  
Parseus/src/Tests/
  DiagnosticTests.cs                ← Tests
```

## 🔗 Verweise

- Haupt-Dokumentation: `DIAGNOSTIC_SYSTEM_GUIDE.md`
- API-Referenz: `Parseus/src/Parser/Diagnostics/README.md`
- Beispiele: `DiagnosticExample.cs`
- Tests: `DiagnosticTests.cs`


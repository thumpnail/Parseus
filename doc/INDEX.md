# 🚀 Diagnostic System - Navigationshilfe

## 📍 Wo anfangen?

### Für Eilige (5 min)
1. Öffne: `Parseus/src/Parser/Diagnostics/QUICK_REFERENCE.md`
2. Kopiere ein Beispiel
3. Fertisch!

### Für Neulinge (20 min)
1. Lese: `DIAGNOSTIC_SYSTEM_GUIDE.md`
2. Schaue: `DiagnosticExample.RunAllDemos()`
3. Teste: `DiagnosticTests.RunAllTests()`

### Für Deep Dive (1h)
1. Lese: `DIAGNOSTIC_SYSTEM_GUIDE.md` (komplett)
2. Lese: `Parseus/src/Parser/Diagnostics/README.md` (API)
3. Studiere: `DiagnosticExample.cs` (Implementierung)
4. Führe Tests aus: `DiagnosticTests.RunAllTests()`

---

## 📚 Dokumentations-Hierarchie

```
┌─ DIAGNOSTIC_SYSTEM_GUIDE.md (diese Datei) ←─ START HIER
│  └─ Überblick, Features, Beispiele
│
├─ Parseus/src/Parser/Diagnostics/README.md ←─ DETAILLIERT
│  └─ Vollständige API-Referenz, Integration Guide
│
├─ Parseus/src/Parser/Diagnostics/QUICK_REFERENCE.md ←─ SCHNELL
│  └─ Copy-Paste Snippets für häufige Aufgaben
│
├─ IMPLEMENTATION_SUMMARY.md ←─ ÜBERBLICK
│  └─ Was wurde implementiert, Datei-Übersicht
│
└─ Code-Beispiele:
   ├─ DiagnosticExample.cs ← Arbeitsbeispiele
   └─ DiagnosticTests.cs ← Automatisierte Tests
```

---

## 🗺️ Datei-Lokationen

### Core-System (5 Dateien)
```
Parseus/src/Parser/Diagnostics/
├── DiagnosticLevel.cs           ← Enum: Error, Warning, Note, Help
├── TextLocation.cs              ← Records: TextLocation, TextSpan
├── Diagnostic.cs                ← Klasse: Diagnostic, LineColumnCache
├── DiagnosticRenderer.cs        ← Engine: Rendering mit ANSI-Farben
└── README.md                    ← API-Dokumentation
```

### Erweiterte Klassen (4 Dateien)
```
Parseus/src/Parser/Common/
├── CancellationState.cs         ✓ + Diagnostics-Sammlung
├── BasicAParserContext.cs       ✓ + Source-Code-Tracking
└── ParseException.cs            ✓ + Diagnostic-Support

Parseus/src/Parser/Implicit/
└── Parser.cs                    ✓ + Helper-Methoden
```

### Beispiele & Tests
```
Parseus/src/Example/
└── DiagnosticExample.cs         ← 3 Demos (kopiere SetSourceCode!)

Parseus/src/Tests/
└── DiagnosticTests.cs           ← 10 Test-Methoden
```

---

## 🎯 Häufige Aufgaben

### "Ich will gerade anfangen"
→ Gehe zu: `DIAGNOSTIC_SYSTEM_GUIDE.md` → "Quick Start"

### "Ich brauch' die APIs"
→ Gehe zu: `Parseus/src/Parser/Diagnostics/README.md` → "API-Referenz"

### "Ich brauch' ein Beispiel"
→ Gehe zu: `DiagnosticExample.cs` oder `QUICK_REFERENCE.md`

### "Ich will sehen, ob es funktioniert"
→ Führe aus: `DiagnosticTests.RunAllTests()`

### "Ich will alle Demos sehen"
→ Führe aus: `DiagnosticExample.RunAllDemos()`

### "Ich will es integrieren"
→ Gehe zu: `DIAGNOSTIC_SYSTEM_GUIDE.md` → "Integration"

### "Ich brauch' nur die Syntax"
→ Gehe zu: `QUICK_REFERENCE.md`

---

## 🔍 Nach Thema suchen

### **Error Reporting**
- Basis: `CancellationState.ReportError()`
- Mit Position: `TextSpan.At(index)`
- Mit Range: `TextSpan.Range(start, end)`
- Helper: `BaseParser.ReportError()`

### **Quellcode anzeigen**
- Setzen: `ctx.SetSourceCode(source)`
- Abrufen: `ctx.GetCurrentSpan()`
- Renderer: `DiagnosticRenderer.Render()`

### **Positionen**
- `TextLocation`: Zeile, Spalte, Index
- `TextSpan`: Start + Length
- `LineColumnCache`: O(log n) Lookups

### **Diagnostics**
- `Diagnostic`: Haupt-Report
- `DiagnosticMessage`: Einzelne Meldung
- `DiagnosticLevel`: Error/Warning/Note/Help

### **Rendering**
- `DiagnosticRenderer.Output()`: Einzeln
- `DiagnosticRenderer.OutputAll()`: Mehrere
- `RenderOptions`: Farbsteuerung

### **Performance**
- `LineColumnCache`: Precomputed Offsets
- O(log n) Position Lookups
- ~2-3 Bytes pro Zeile

---

## 💻 Code-Snippets nach Kategorie

### Copy-Paste: Initialisierung
```csharp
var ctx = new BasicAParserContext(tokens);
ctx.SetSourceCode(sourceCode);
var state = new CancellationState();
var parserCtx = new BaseParserContext(ctx, state);
```

### Copy-Paste: Fehler melden
```csharp
BaseParser.ReportError(parserCtx, "message");
BaseParser.ReportWarning(parserCtx, "message");
BaseParser.ReportNote(parserCtx, "message");
```

### Copy-Paste: Ausgeben
```csharp
BaseParser.OutputDiagnostics(parserCtx);
if (parserCtx.State.HasErrors) {
    var summary = BaseParser.GetDiagnosticSummary(parserCtx);
    Console.WriteLine($"error: {summary}");
}
```

Mehr Copy-Paste-Snippets: siehe `QUICK_REFERENCE.md`

---

## 🧪 Testing

### Alle Tests ausführen
```csharp
DiagnosticTests.RunAllTests();
```

### Alle Demos ausführen
```csharp
DiagnosticExample.RunAllDemos();
```

### Einzelnes Test-Beispiel
```csharp
DiagnosticTests.TestTextSpanCreation();
DiagnosticTests.TestDiagnosticRenderer();
```

---

## 📞 Troubleshooting

### "Ich sehe keine Farben"
→ Use: `RenderOptions { UseColors = true }`

### "Zu viele Context-Zeilen"
→ Use: `RenderOptions { ContextLines = 1 }`

### "Fehler werden nicht gesammelt"
→ Check: `ctx.SetSourceCode(source)` ist aufgerufen?

### "Zeile/Spalte ist falsch"
→ Check: TextSpan-Indices sind korrekt?

### "Kompiliert nicht"
→ Check: `using Parseus.Parser.Diagnostics;` hinzufügen

---

## 🎓 Lernpfad

### Level 1: Basics (10 min)
- [ ] Lese QUICK_REFERENCE.md
- [ ] Kopiere ein Beispiel
- [ ] Führe es aus

### Level 2: Integration (30 min)
- [ ] Lese DIAGNOSTIC_SYSTEM_GUIDE.md
- [ ] Integriere in deinen Parser
- [ ] Führe DiagnosticExample.RunAllDemos() aus

### Level 3: Mastery (1h)
- [ ] Lese README.md vollständig
- [ ] Studiere DiagnosticExample.cs
- [ ] Führe DiagnosticTests.RunAllTests() aus
- [ ] Experimentiere mit RenderOptions

---

## ✅ Checkliste vor dem Einsatz

- [ ] `using Parseus.Parser.Diagnostics;` hinzugefügt
- [ ] `ctx.SetSourceCode(source)` aufgerufen
- [ ] `BaseParser.ReportError()` statt Exception
- [ ] `BaseParser.OutputDiagnostics()` aufgerufen
- [ ] Tests laufen: `DiagnosticTests.RunAllTests()`
- [ ] Demos laufen: `DiagnosticExample.RunAllDemos()`

---

## 📖 Weitere Ressourcen

| Ressource | Zweck | Zeit |
|-----------|-------|------|
| QUICK_REFERENCE.md | Schnelle Syntax | 5 min |
| README.md | API-Details | 20 min |
| DIAGNOSTIC_SYSTEM_GUIDE.md | Komplett-Guide | 30 min |
| DiagnosticExample.cs | Arbeitsbeispiele | 15 min |
| DiagnosticTests.cs | Test-Validierung | 10 min |

---

## 🚀 Schnelleinstig (< 5 min)

1. Kopiere diese 3 Zeilen:
```csharp
var ctx = new BasicAParserContext(tokens);
ctx.SetSourceCode(sourceCode);
BaseParser.ReportError(parserCtx, "message");
```

2. Führe dies aus:
```csharp
BaseParser.OutputDiagnostics(parserCtx);
```

3. Fertig! 🎉

---

**Last Updated**: 2024  
**Status**: ✅ Vollständig  
**Version**: 1.0  


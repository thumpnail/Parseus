using Parseus.Lexer;
using Parseus.Parser.Common;
using Parseus.Parser.Diagnostics;
using Parseus.Parser.Implicit;

namespace Parseus.Tests;

/// <summary>
/// Manual tests for the diagnostic system (for documentation and quick validation).
/// Can be run by calling the static methods.
/// </summary>
public static class DiagnosticTests {
    
    public static void TestTextSpanCreation() {
        Console.WriteLine("Test: TextSpan Creation");
        var span1 = TextSpan.At(5);
        Assert(span1.StartIndex == 5);
        Assert(span1.Length == 1);
        Assert(span1.EndIndex == 6);
        
        var span2 = TextSpan.Range(10, 15);
        Assert(span2.StartIndex == 10);
        Assert(span2.Length == 6);
        Assert(span2.EndIndex == 16);
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestLineColumnCache() {
        Console.WriteLine("Test: LineColumnCache");
        var source = "line1\nline2\nline3";
        var cache = new LineColumnCache(source);
        
        // First line
        var (line, col) = cache.GetLineColumn(0);
        Assert(line == 1);
        Assert(col == 1);
        
        // Second line
        (line, col) = cache.GetLineColumn(6);
        Assert(line == 2);
        Assert(col == 1);
        
        // Third line
        (line, col) = cache.GetLineColumn(12);
        Assert(line == 3);
        Assert(col == 1);
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestDiagnosticCreation() {
        Console.WriteLine("Test: Diagnostic Creation");
        var message = new DiagnosticMessage(DiagnosticLevel.Error, "test error");
        var diag = new Diagnostic(message, "test.txt");
        
        Assert(diag.Message.Level == DiagnosticLevel.Error);
        Assert(diag.Message.Text == "test error");
        Assert(diag.SourceLabel == "test.txt");
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestDiagnosticWithRelatedMessages() {
        Console.WriteLine("Test: Diagnostic With Related Messages");
        var diag = new Diagnostic(
            new DiagnosticMessage(DiagnosticLevel.Error, "main error"),
            "test.txt"
        )
        .WithMessage(DiagnosticLevel.Note, "note 1")
        .WithMessage(DiagnosticLevel.Help, "help text");
        
        Assert(diag.RelatedMessages.Count == 2);
        Assert(diag.RelatedMessages[0].Level == DiagnosticLevel.Note);
        Assert(diag.RelatedMessages[1].Level == DiagnosticLevel.Help);
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestCancellationStateReporting() {
        Console.WriteLine("Test: CancellationState Reporting");
        var state = new CancellationState();
        Assert(!state.HasErrors);
        Assert(!state.HasWarnings);
        
        state.ReportError("error 1");
        Assert(state.HasErrors);
        Assert(!state.HasWarnings);
        
        state.ReportWarning("warning 1");
        Assert(state.HasWarnings);
        
        Assert(state.Diagnostics.Count == 2);
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestDiagnosticRenderer() {
        Console.WriteLine("Test: Diagnostic Renderer");
        var source = @"let x = 10
let y = x +";
        
        var diag = new Diagnostic(
            new DiagnosticMessage(DiagnosticLevel.Error, "unexpected end"),
            "test.nano"
        )
        .WithSourceCode(source);
        
        var rendered = DiagnosticRenderer.Render(diag, new() { UseColors = false });
        
        Assert(rendered.Contains("error"));
        Assert(rendered.Contains("unexpected end"));
        Assert(rendered.Contains("let y = x +"));
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestDiagnosticSummary() {
        Console.WriteLine("Test: Diagnostic Summary");
        var diags = new List<Diagnostic> {
            new(new DiagnosticMessage(DiagnosticLevel.Error, "error 1")),
            new(new DiagnosticMessage(DiagnosticLevel.Error, "error 2")),
            new(new DiagnosticMessage(DiagnosticLevel.Warning, "warning 1")),
        };
        
        var summary = DiagnosticRenderer.GetSummary(diags);
        
        Assert(summary.Contains("2 errors"));
        Assert(summary.Contains("1 warning"));
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestParserContextIntegration() {
        Console.WriteLine("Test: Parser Context Integration");
        var source = "let x = 10";
        var tokens = new List<TokenElement> {
            new("KEYWORD", "let", 0, 3),
            new("IDENT", "x", 4, 1),
            new("ASSIGN", "=", 6, 1),
            new("NUMBER", "10", 8, 2),
        };
        
        var ctx = new BasicAParserContext(tokens);
        ctx.SetSourceCode(source);
        
        Assert(ctx.SourceCode != null);
        Assert(ctx.LineCache != null);
        
        var span = ctx.GetCurrentSpan();
        Assert(span.StartIndex == 0);
        Assert(span.Length == 3);
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestDifferentDiagnosticLevels() {
        Console.WriteLine("Test: Different Diagnostic Levels");
        var levels = new[] {
            DiagnosticLevel.Error,
            DiagnosticLevel.Warning,
            DiagnosticLevel.Note,
            DiagnosticLevel.Help
        };
        
        foreach (var level in levels) {
            var diag = new Diagnostic(
                new DiagnosticMessage(level, $"{level} message"),
                "test.txt"
            );
            
            var rendered = DiagnosticRenderer.Render(diag, new() { UseColors = false });
            var levelName = level.ToString().ToLower();
            
            Assert(rendered.Contains(levelName), $"Expected '{levelName}' in output");
        }
        Console.WriteLine("✓ PASSED\n");
    }

    public static void TestParseExceptionWithDiagnostic() {
        Console.WriteLine("Test: ParseException With Diagnostic");
        var diag = new Diagnostic(
            new DiagnosticMessage(DiagnosticLevel.Error, "parse error"),
            "test.txt"
        );
        
        var ex = new ParseException(diag);
        
        Assert(ex.Diagnostic != null);
        Assert(ex.Message == "parse error");
        Console.WriteLine("✓ PASSED\n");
    }

    /// <summary>
    /// Run all tests.
    /// </summary>
    public static void RunAllTests() {
        Console.WriteLine("\n════════════════════════════════════════════════════════════════");
        Console.WriteLine("  Diagnostic System - Manual Tests");
        Console.WriteLine("════════════════════════════════════════════════════════════════\n");
        
        try {
            TestTextSpanCreation();
            TestLineColumnCache();
            TestDiagnosticCreation();
            TestDiagnosticWithRelatedMessages();
            TestCancellationStateReporting();
            TestDiagnosticRenderer();
            TestDiagnosticSummary();
            TestParserContextIntegration();
            TestDifferentDiagnosticLevels();
            TestParseExceptionWithDiagnostic();
            
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  ✓ ALL TESTS PASSED");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");
        } catch (Exception ex) {
            Console.WriteLine($"\n✗ TEST FAILED: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    /// <summary>
    /// Simple assertion helper.
    /// </summary>
    private static void Assert(bool condition, string? message = null) {
        if (!condition) {
            throw new Exception(message ?? "Assertion failed");
        }
    }
}



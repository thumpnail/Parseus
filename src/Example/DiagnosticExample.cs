using Parseus.Lexer;
using Parseus.Lexer.RegExBased;
using Parseus.Parser.Common;
using Parseus.Parser.Diagnostics;
using Parseus.Parser.Implicit;

namespace Parseus.Example;

/// <summary>
/// Demo example showing how to use the Rust-like diagnostic system.
/// </summary>
public static class DiagnosticExample {
    
    /// <summary>
    /// Simple arithmetic expression lexer for demonstration.
    /// </summary>
    public static LexerResult LexArithmetic(string source) {
        var lexer = new Lexer.RegExBased.Lexer()
            .Child("NUMBER", @"[0-9]+")
            .Child("IDENT", @"[a-zA-Z_][a-zA-Z0-9_]*")
            .Child("PLUS", @"\+")
            .Child("MINUS", @"\-")
            .Child("STAR", @"\*")
            .Child("SLASH", @"\/")
            .Child("LPAREN", @"\(")
            .Child("RPAREN", @"\)")
            .Child("ASSIGN", @"\=")
            .Skippable("WHITESPACE", " ", "\t", "\n", "\r\n");
        
        return lexer.Lex(source);
    }

    /// <summary>
    /// Demonstrates various diagnostic levels and formatting.
    /// </summary>
    public static void DemoBothDiagnosticLevels() {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  Diagnostics System Demo - Rust-like Error Formatting");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        // Example 1: Simple error with context
        {
            Console.WriteLine("Example 1: Syntax Error\n");
            
            var source = @"let x = 42
let y = x + 
let z = 10";

            var diag = new Diagnostic(
                new DiagnosticMessage(
                    DiagnosticLevel.Error, 
                    "unexpected end of expression: expected operand after '+'"
                ),
                "arithmetic.nano"
            )
            .WithSourceCode(source)
            .WithMessage(DiagnosticLevel.Help, "add an expression after the '+' operator");

            DiagnosticRenderer.Output(diag);
            Console.WriteLine();
        }

        // Example 2: Multiple related messages
        {
            Console.WriteLine("Example 2: Type Mismatch Error\n");
            
            var source = @"fn calculate(x: int, y: int) -> int {
    return x + ""hello"";
}";

            var diag = new Diagnostic(
                new DiagnosticMessage(
                    DiagnosticLevel.Error,
                    "cannot add 'int' and 'string'"
                ),
                "types.nano"
            )
            .WithSourceCode(source)
            .WithMessage(DiagnosticLevel.Note, "left operand is of type 'int'")
            .WithMessage(DiagnosticLevel.Note, "right operand is of type 'string'")
            .WithMessage(DiagnosticLevel.Help, "consider converting one of the operands");

            DiagnosticRenderer.Output(diag);
            Console.WriteLine();
        }

        // Example 3: Warning with notes
        {
            Console.WriteLine("Example 3: Warning - Unused Variable\n");
            
            var source = @"fn main() {
    let unused_var = 42;
    let result = 10 + 20;
    return result;
}";

            var diag = new Diagnostic(
                new DiagnosticMessage(
                    DiagnosticLevel.Warning,
                    "unused variable: 'unused_var'"
                ),
                "warnings.nano"
            )
            .WithSourceCode(source)
            .WithMessage(DiagnosticLevel.Note, "if this is intentional, prefix with '_'")
            .WithMessage(DiagnosticLevel.Help, "consider renaming to '_unused_var'");

            DiagnosticRenderer.Output(diag);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Demonstrates diagnostic collection during parsing.
    /// </summary>
    public static void DemoDiagnosticCollection() {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  Diagnostic Collection During Parsing");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var source = @"let x = 10
let y = x + ;
let z = y * 2
let = 5";

        // Lex the source
        var lexResult = LexArithmetic(source);
        
        // Create parser context
        var parserCtx = new BasicAParserContext(lexResult) {SourceCode = source};
        parserCtx.SetSourceCode(source);
        
        var state = new CancellationState();
        var ctx = new BaseParserContext(parserCtx, state);

        // Simulate parsing with errors
        Console.WriteLine("Parsing with error collection:\n");
        
        // Simulate first error at line 2
        parserCtx.Pos = 5; // Position at the error
        state.ReportError(
            "expected expression after '+'",
            parserCtx.GetCurrentSpan(),
            "parse.nano"
        );

        // Simulate warning at line 4
        parserCtx.Pos = 14;
        state.ReportWarning(
            "missing identifier in assignment",
            parserCtx.GetCurrentSpan(),
            "parse.nano"
        );

        // Output all diagnostics at once
        DiagnosticRenderer.OutputAll(state.Diagnostics);
        
        // Show summary
        var summary = DiagnosticRenderer.GetSummary(state.Diagnostics);
        Console.WriteLine($"\nerror: {summary}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates using diagnostics in a real parser flow.
    /// </summary>
    public static void DemoParserIntegration() {
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("  Real Parser Integration Example");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        var source = @"fn add(a, b) {
    if a == null
        return a + b;
    return 0;
}";

        Console.WriteLine("Source code:");
        Console.WriteLine(source);
        Console.WriteLine();

        var lexResult = LexArithmetic(source);
        var parserCtx = new BasicAParserContext(lexResult) {SourceCode = source};
        parserCtx.SetSourceCode(source);
        
        var state = new CancellationState();
        var ctx = new BaseParserContext(parserCtx, state);

        // Use the helper method to report an error
        parserCtx.Pos = 8;
        //BaseParser.ReportError(ctx, "missing closing parenthesis in function parameters");
        
        parserCtx.Pos = 25;
        //BaseParser.ReportWarning(ctx, "missing return type annotation");
        
        parserCtx.Pos = 40;
        //BaseParser.ReportNote(ctx, "comparing with null might cause runtime errors");

        // Output diagnostics
        //BaseParser.OutputDiagnostics(ctx);
        
        // Show summary
        //var summary = BaseParser.GetDiagnosticSummary(ctx);
        //Console.WriteLine($"error: {summary}");
        Console.WriteLine();
    }

    /// <summary>
    /// Run all demos.
    /// </summary>
    public static void RunAllDemos() {
        DemoBothDiagnosticLevels();
        Console.WriteLine("\n");
        
        DemoDiagnosticCollection();
        Console.WriteLine("\n");
        
        DemoParserIntegration();
    }
}



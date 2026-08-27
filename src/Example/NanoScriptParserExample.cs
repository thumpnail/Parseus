using Parseus.Parser.Common;
using Parseus.Parser.Diagnostics;
using Parseus.Parser.Implicit;
namespace Parseus.Example;
/// <summary>
/// Praktisches Integration-Beispiel: NanoScript Parser mit Diagnostic System
/// </summary>
public static class NanoScriptParserExample {
    /// <summary>
    /// Erstellt einen Lexer für NanoScript.
    /// </summary>
    public static Lexer.RegExBased.Lexer CreateNanoScriptLexer() {
        var lexer = new Lexer.RegExBased.Lexer();
        lexer.Child("WHILE", "whl");
        lexer.Child("FOR", "for");
        lexer.Child("IF", "if");
        lexer.Child("PRINT", "prt");
        lexer.Child("EXIT", "ext");
        lexer.Child("STRING", "\"[^\"]*\"");
        lexer.Child("NUMBER", "[0-9]+");
        lexer.Child("IDENT", "[a-zA-Z_][a-zA-Z0-9_]*");
        lexer.Child("COLON", ":");
        lexer.Child("ASSIGN", "=");
        lexer.Child("PLUS", @"\+");
        lexer.Child("LPAREN", @"\(");
        lexer.Child("RPAREN", @"\)");
        lexer.Skippable("WS", " ", "\t");
        lexer.Skippable("NL", "\n", "\r\n");
        return lexer;
    }
    /// <summary>
    /// Demonstriert Parsing mit Diagnostic-Ausgabe.
    /// </summary>
    public static void DemoParseWithErrors() {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  NanoScript Parser - Diagnostic System Demo                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        var source = "whl 1:\n  prt print \"Hello\"\n  ext";
        Console.WriteLine("📝 Input Code:");
        Console.WriteLine(source);
        Console.WriteLine("\n🔍 Lexing...");
        var lexer = CreateNanoScriptLexer();
        var lexResult = lexer.Lex(source);
        Console.WriteLine("   ✓ " + lexResult.result.Count + " Tokens\n");
        var parserCtx = new BasicAParserContext(lexResult.result);
        parserCtx.SetSourceCode(source);
        var state = new CancellationState();
        var ctx = new BaseParserContext(parserCtx, state);
        Console.WriteLine("⚙️  Parsing...\n");
        SimulateParsing(ctx, parserCtx);
        if (state.HasDiagnostics) {
            Console.WriteLine("═════════════════════════════════════════════════════════════════");
            Console.WriteLine("📋 Diagnostics:\n");
            //BaseParser.OutputDiagnostics(ctx, new() { UseColors = true });
            //Console.WriteLine("\nerror: " + BaseParser.GetDiagnosticSummary(ctx));
        }
        Console.WriteLine("\n═════════════════════════════════════════════════════════════════\n");
    }
    private static void SimulateParsing(BaseParserContext ctx, BasicAParserContext parserCtx) {
        if (parserCtx.MatchValue("whl")) {
            parserCtx.Consume();
            Console.WriteLine("   ✓ 'whl' parsed");
        } else {
            //BaseParser.ReportError(ctx, "expected 'whl'");
            return;
        }
        if (parserCtx.MatchToken("NUMBER")) {
            parserCtx.Consume();
            Console.WriteLine("   ✓ condition parsed");
        } else {
            //BaseParser.ReportError(ctx, "expected condition");
            return;
        }
        if (parserCtx.MatchValue(":")) {
            parserCtx.Consume();
            Console.WriteLine("   ✓ ':' parsed");
        } else {
            //BaseParser.ReportError(ctx, "expected ':'");
            return;
        }
        if (parserCtx.MatchValue("prt")) {
            parserCtx.Consume();
            Console.WriteLine("   ✓ 'prt' parsed");
        }
        if (parserCtx.MatchToken("IDENT")) {
            var token = parserCtx.Consume();
            if (token.Value == "print") {
                parserCtx.Pos--;
                //BaseParser.ReportError(ctx, "unexpected '" + token.Value + "'");
                //BaseParser.ReportNote(ctx, "'prt' needs no argument");
                return;
            }
        }
        Console.WriteLine("   ✓ Parsing completed");
    }
    public static void DemoDifferentErrorTypes() {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Different Error Types                                        ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");
        var source = "whl x y z:";
        var diag = new Diagnostic(
            new DiagnosticMessage(DiagnosticLevel.Error, "syntax error"),
            "test.nano"
        )
        .WithSourceCode(source)
        .WithMessage(DiagnosticLevel.Help, "fix this");
        DiagnosticRenderer.Output(diag, new() { UseColors = false });
    }
    public static void RunAll() {
        DemoParseWithErrors();
        DemoDifferentErrorTypes();
    }
}

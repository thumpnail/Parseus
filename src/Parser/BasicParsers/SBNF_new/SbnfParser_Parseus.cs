using Parseus.Parser.Common;
using Parseus.Parser.Implicit;
using Parseus.Lexer;
namespace Parseus.Parser.BasicParsers.SBNF_new;

public class SbnfParser_Parseus : BaseParser {
    // SbnfDocument = { rule } ;
    public record SbnfDocument {
        public List<SbnfRule> Rules = new();
    }

    // rule = identifier ':=' alternation ;
    public record SbnfRule {
        public string Identifier;
        public SbnfAlternation Alternation;
    }

    // alternation = concatenation { '|' concatenation } ;
    public record SbnfAlternation {
        public List<SbnfConcatenation> Concatenations = new();
    }

    // concatenation = factor ;
    public record SbnfConcatenation {
        public List<SbnfFactor> Factors = new();
    }

    // factor = term '?' | term '*' | term '+' | term ;
    public record SbnfFactor {
        public SbnfTerm Term;
        public string? Operator;
    }

    // term = '(' alternation ')' 
    // | '[' alternation ']' 
    // | '{' alternation '}' 
    // | string 
    // | identifier ;
    public record SbnfTerm {
        public string Type;
        public string Value;
        public SbnfAlternation Alternation;
    }
    // SbnfDocument = { rule } ;
    private static Parser<SbnfDocument> ParseSbnfDocument = new((c, self) => {
        Opt(c, c => {
            Repeat(c, c => {
                Node(c, SbnfRuleParser, v => self.Rules.Add(v));
            });
        });
    });
    // rule = identifier ':=' alternation ;
    private static Parser<SbnfRule> SbnfRuleParser = new((c, self) => {
        Token(c, "identifier", out self.Identifier);
        Token(c, "assign", out _);
        Node(c, SbnfAlternationParser, v => self.Alternation = v);
        Token(c, "semicolon", out _);
    });
    // alternation = concatenation { '|' concatenation } ;
    private static Parser<SbnfAlternation> SbnfAlternationParser = new((c, self) => {
        Node(c, SbnfConcatenationParser, v => self.Concatenations.Add(v));
        Opt(c, c => {
            Repeat(c, c => {
                Token(c, "bitwise_or", out _);
                Node(c, SbnfConcatenationParser, v => self.Concatenations.Add(v));
            });
        });
    });
    // concatenation = factor ;
    private static Parser<SbnfConcatenation> SbnfConcatenationParser = new((c, self) => {
        Repeat(c, c => {
            Node(c, SbnfFactorParser, v => self.Factors.Add(v));
        });
    });

    // factor = term '?' | term '*' | term '+' | term ;
    private static Parser<SbnfFactor> SbnfFactorParser = new((c, self) => {
        Node(c, SbnfTermParser, out self.Term);
    });

    // term = '(' alternation ')' 
    // 	 | '[' alternation ']' 
    // 	 | '{' alternation '}' 
    // 	 | string 
    // 	 | identifier ;
    private static Parser<SbnfTerm> SbnfTermParser = new((c, self) => {
        Alt(c, c => {
            Token(c, "lparen", out _);
            Node(c, SbnfAlternationParser, v => {
                self.Alternation = v;
                self.Type = "group";
            });
            Token(c, "rparen", out _);
        }, c => {
            Token(c, "lbracket", out _);
            Node(c, SbnfAlternationParser, v => {
                self.Alternation = v;
                self.Type = "optional";
            });
            Token(c, "rbracket", out _);
        }, c => {
            Token(c, "lbrace", out _);
            Node(c, SbnfAlternationParser, v => {
                self.Alternation = v;
                self.Type = "repeat";
            });
            Token(c, "rbrace", out _);
        }, c => {
            Token(c, "string", v => {
                self.Value = v;
                self.Type = "string";
            });
        }, c => {
            Token(c, "identifier", v => {
                self.Value = v;
                self.Type = "identifier";
            });
        });
    });
    
    public override SbnfDocument Parse(string src) {
        var lexerResult = LexerResult(src);
        var parserContext = new BasicAParserContext(lexerResult);
        var state = new CancellationState();
        var result = ParseSbnfDocument.Parse(new BaseParserContext(parserContext,state));
        if (state.Ok) {
            return result;
        }
        throw new ParseException("Parse failed", "SbnfParser_Parseus.Parse");
    }

    private static LexerResult LexerResult(string src) {

        var lexerResult = new Lexer.Lexer()
            // Brackets
            .Child("lparen", "\\(").Child("rparen", "\\)").Child("lbracket", "\\[").Child("rbracket", "\\]").Child("lbrace", "\\{").Child("rbrace", "\\}")
            // Symbols
            .Child("and", "&&").Child("bitwise_or", "\\|").Child("bitwise_and", "&").Child("questionable", "\\?").Child("assign", ":=").Child("equals", "==")
            .Child("not_equals", "!=").Child("less_equal", "<=").Child("greater_equal", ">=").Child("less", "<").Child("greater", ">").Child("plus", "\\+").Child("minus", "-")
            .Child("star", "\\*").Child("slash", "/").Child("percent", "%").Child("arrow", "->").Child("semicolon", ";").Child("colon", ":").Child("comma", ",").Child("dot", "\\.")
            // Literals
            .Child("string", "\"[^\"]*\"", "'[^']*'").Child("number", "\\d+(\\.\\d+)?")
            // Keywords
            .Child("kw_if", "if").Child("kw_else", "else").Child("kw_while", "while").Child("kw_for", "for").Child("kw_return", "return").Child("kw_function", "function")
            .Child("kw_var", "var").Child("kw_let", "let").Child("kw_const", "const").Child("kw_true", "true").Child("kw_false", "false").Child("kw_null", "null")
            .Child("kw_new", "new").Child("kw_this", "this")

            // Identifiers
            .Child("identifier", "[a-zA-Z_][a-zA-Z0-9_]*")

            // Whitespace and comments
            .Skippable("whitespace", "\\s+").Skippable("comment_single", "//[^\\n]*").Skippable("comment_multi", "/\\*[\\s\\S]*?\\*/")
            // Start lexing
            .Lex(src);
        return lexerResult;
    }
    public static void main_test_sbnf_parser() {
        var parser = new SbnfParser_Parseus();
        var result = parser.Parse(
            """
            test := [ 'var' | 'let' ] 'identifier' [ ':' type ] '=' expression ;
            """);
        // print result as ymal using yamldotnet
        Console.WriteLine(new YamlDotNet.Serialization.SerializerBuilder()
            .DisableAliases()
            .IncludeNonPublicProperties()
            .EnsureRoundtrip()
            .Build().Serialize(result));
    }
    
}
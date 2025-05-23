using System.Text;
using Parseus.Parser.Common;
using Parseus.Parser.Implicit;
using Parseus.Lexer;
namespace Parseus.Parser.BasicParsers.SBNF_new;

public class SbnfParser_Parseus : BaseParser {
    // SbnfDocument = { rule } ;
    private static List<(string key, List<string> value)> typeMap = new();
    public record SbnfDocument {
        public List<SbnfRule> Rules = new();
        public override string ToString() {
            StringBuilder sb = new();
            foreach (var rule in Rules) {
                sb.Append(rule.ToString());
            }
            foreach (var type in typeMap) {
                sb.Append($"public record {type.key} {{");
                foreach (var value in type.value) {
                    sb.Append($"public object {value};");
                }
                sb.Append($"}}");
            }
            return sb.ToString();
        }
    }

    // rule = identifier ':=' alternation ;
    public record SbnfRule {
        public string Identifier;
        public SbnfAlternation Alternation;
        public override string ToString() {
            StringBuilder sb = new();
            sb.Append($"private static Parser<Generated_{Identifier.ToUpper()}> Generated_{Identifier.ToUpper()}Parser = new((c, self) => {{");
            typeMap.Add(($"Generated_{Identifier.ToUpper()}", new()));
            sb.Append(Alternation.ToString());
            sb.Append($"}});");
            return sb.ToString();
        }
    }

    // alternation = concatenation { '|' concatenation } ;
    public record SbnfAlternation {
        public List<SbnfConcatenation> Concatenations = new();
        public override string ToString() {
            StringBuilder sb = new();
            if (Concatenations.Count > 1) {
                sb.Append($"Alt(c,");
                foreach (var concat in Concatenations) {
                    sb.Append($"c=> {{");
                    sb.Append(concat.ToString());
                    sb.Append($"}},");
                }
                sb.Append(");");
            } else {
                foreach (var concat in Concatenations) {
                    sb.Append(concat.ToString());
                }
            }
            return sb.ToString();
        }
    }

    // concatenation = { factor } ;
    public record SbnfConcatenation {
        public List<SbnfFactor> Factors = new();
        public override string ToString() {
            StringBuilder sb = new();
            foreach (var factor in Factors) {
                sb.Append(factor.ToString());
            }
            return sb.ToString();
        }
    }

    // factor = term '?' | term '*' | term '+' | term ;
    public record SbnfFactor {
        public SbnfTerm Term;
        public string? Operator;
        public override string ToString() {
            StringBuilder sb = new();
            sb.Append(Term.ToString());
            return sb.ToString();
        }
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
        public override string ToString() {
            StringBuilder sb = new();
            switch (Type) {
                case "identifier":
                    //node
                    var tmp1 = $"{Value.ToUpper()}_{Guid.NewGuid().ToString().Replace('-', '_')}";
                    sb.Append($"Node(c, Generated_{Value.ToUpper()}Parser, out self.{tmp1});");
                    typeMap.Last().value.Add(tmp1);
                    break;
                case "string":
                    //literal
                    var tmp2 = $"{Value.Substring(1,Value.Length-2).ToUpper()}_{Guid.NewGuid().ToString().Replace('-', '_')}";
                    sb.Append($"Literal(c, \"{Value.Substring(1,Value.Length-2).ToUpper()}\", out self.{tmp2});");
                    typeMap.Last().value.Add(tmp2);
                    break;
                case "group":
                    // alt
                    sb.Append($"Alt(c, c => {{");
                    sb.Append(Alternation.ToString());
                    sb.Append($"}});");
                    break;
                case "optional":
                    // opt
                    sb.Append($"Opt(c, c => {{");
                    sb.Append(Alternation.ToString());
                    sb.Append($"}});");
                    break;
                case "repeat":
                    // repeat
                    sb.Append($"Repeat(c, c => {{");
                    sb.Append(Alternation.ToString());
                    sb.Append($"}});");
                    break;
            }
            return sb.ToString();
        }
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
            program         := { statement } ;
            statement       := variable_declaration
                            | constant_declaration
                            | operation
                            | function_definition
                            | function_call
                            | jump_statement
                            | conditional_statement
                            | loop_statement
                            | struct_definition
                            | package_definition
                            | include_statement
                            | destruct_statement
                            | error_statement ;
            variable_declaration := "let" identifier value { value } ;
            constant_declaration := "cst" identifier value { value } ;
            value           := identifier
                            | number
                            | string
                            | character
                            | array_definition ;
            array_definition := "LBRACKET" value { "COMMA" value } "RBRACKET" ;
            operation       := identifier identifier operator identifier ;
            operator        := "PLUS" | "MINUS" | "STAR" | "SLASH" ;
            function_definition := "fnc" identifier [ parameter_list ] block "ret" [ return_value { return_value } ] ;
            parameter_list  := identifier { identifier } ;
            return_value    := identifier | value ;
            function_call   := "cll" identifier { identifier | value } ;
            jump_statement  := "jmp" identifier | "COLON" identifier ;
            conditional_statement := "iff" condition block [ "elf" condition block ] [ "els" block ] "ext" ;
            condition       := identifier logical_operator identifier ;
            logical_operator := "EQL" | "LES" | "GRT" | "TRU" | "FLS" ;
            loop_statement  := "whl" condition block "ext"
                            | "for" identifier value condition value value block "ext"
                            | "for" identifier identifier block "ext" ;
            struct_definition := "tbl" identifier [ parameter_list ] block "ext" ;
            package_definition := "pck" identifier block "ext" ;
            include_statement := "inc" identifier [ identifier ] ;
            destruct_statement := "TILDE" identifier ;
            error_statement := "err" number [ string ] ;
            block           := { statement } ;
            """);
        // print result as ymal using yamldotnet
        Console.WriteLine(new YamlDotNet.Serialization.SerializerBuilder()
            .DisableAliases()
            .IncludeNonPublicProperties()
            .EnsureRoundtrip()
            .Build().Serialize(result));
        File.AppendAllText("out.cs", result.ToString().Trim());
    }
}
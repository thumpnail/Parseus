using System.ComponentModel;
using System.Security.Principal;
using Parseus.Lexer;
using Parseus.Parser;
using static Parseus.Parser.Parser<Token>;

public enum Token {
    NONE = -1,
    COMMENT = 0,
    EOL = 1,
    CLASS = 2,
    STRUCT = 3,
    FNC = 4,
    LET = 5,
    VAR = 6,
    SEMICOLON = 7,
    IDENTIFIER = 8,
    STRING = 9,
    NUMBER = 10,
    EQUALS = 11
}

class Program {
    static void Main(string[] args) {
        var lexer = new Lexer<Token>()
            .skipable(Token.COMMENT, @"\/\/.*")
            .child(Token.EOL, Environment.NewLine)
            .child(Token.CLASS, "class")
            .child(Token.STRUCT, "struct")
            .child(Token.FNC, "fnc")
            .child(Token.LET, "let")
            .child(Token.VAR, "var")
            .child(Token.SEMICOLON, ";")
            .child(Token.IDENTIFIER, "[a-zA-Z_][a-zA-Z0-9_]*")
            .child(Token.STRING, @"'(\\.|[^'\\])*'", "\"" + @"(\\.|[^" + "\"" + @"\\])*" + "\"")
            .child(Token.NUMBER, @"-?(0[xX][0-9a-fA-F]+|\d*[,.]\d+([eE][+-]?\d+)?|\d+([,.]\d*)?([eE][+-]?\d+)?)");

        // Parser
        var parser = ParseTreeFactory(
            Rule("program", Repeat(RuleRef("statement"))),
            Rule("statement", Alt(
                Branch(RuleRef("assignment")),
                Branch(RuleRef("if_statement")),
                Branch(RuleRef("while_statement")),
                Branch(RuleRef("expression_statement"))
            )),
            // assignment = "let" identifier "=" expression ";";
            Rule("assignment", Token("let"), RuleRef("identifier"), Token("="), RuleRef("expression"), Token(";")),
            // if_statement = "if" "(" expression ")" "{" statement* "}" [ "else" "{" statement* "}" ] ;
            Rule("if_statement", Token("if"), Token("("), RuleRef("expression"), Token(")"), RuleRef("block")),
            // "while" "(" expression ")" "{" statement* "}" ;
            Rule("while_statement", Token("while"), Token("("), RuleRef("expression"), Token(")"), RuleRef("block")),
            // expression_statement = expression ";" ;
            Rule("expression_statement", RuleRef("expression"), Token(";")),

            // expressions
            // expression = logical_expression
            Rule("expression",
                Alt(
                    Branch(RuleRef("logical_expression"))
                )
            ),

            // logical_expression = relational_expression [ ("&&" | "||") relational_expression ]* ;
            Rule("logical_expression",
                RuleRef("relational_expression"),
                Repeat(
                    Alt(
                        Branch(Token("&&")),
                        Branch(Token("||"))
                    ),
                    RuleRef("relational_expression")
                )
            ),
            // relational_expression = additive_expression [ ( "==" | "!=" | "<" | ">" | "<=" | ">=" ) additive_expression ]* ;
            Rule("relational_expression",
                RuleRef("additive_expression"),
                Repeat(
                    Alt(
                        Branch(Token("==")),
                        Branch(Token("!=")),
                        Branch(Token("||")),
                        Branch(Token("<")),
                        Branch(Token(">")),
                        Branch(Token("<=")),
                        Branch(Token(">="))
                    ),
                    RuleRef("additive_expression")
                )
            ),
            // additive_expression = multiplicative_expression [ ( "+" | "-" ) multiplicative_expression ]* ;
            Rule("additive_expression",
                RuleRef("multiplicative_expression"),
                Repeat(
                    Alt(
                        Branch(Token("+")),
                        Branch(Token("-"))
                    ),
                    RuleRef("multiplicative_expression"))
            ),
            // multiplicative_expression = primary_expression [ ( "*" | "/" | "%" ) primary_expression ]* ;
            Rule("multiplicative_expression",
                RuleRef("primary_expression"),
                Repeat(
                    Alt(
                        Branch(Token("*")),
                        Branch(Token("/")),
                        Branch(Token("%"))
                        ),
                    RuleRef("primary_expression")
                    )
                ),
            // primary_expression = number | identifier | string | "(" expression ")" ;
            Rule("primary_expression", Alt(
                    Branch(RuleRef("number")),
                    Branch(RuleRef("identifier")),
                    Branch(RuleRef("string")),
                    Branch(RuleRef("funccall")),
                    Branch(Token("("), RuleRef("expression"), Token(")"))
                    )),

            Rule("block", Token("{"), Repeat(RuleRef("statement")), Token("}")),
            Rule("argument_list", Token("("), Optional(RuleRef("expression"), Repeat(Token(","), RuleRef("expression"))), Token(")")),

            Rule("number", Token(Token.NUMBER)),
            Rule("identifier", Token(Token.IDENTIFIER)),
            Rule("string", Token(Token.STRING)),
            Rule("funccall", Token(Token.IDENTIFIER),RuleRef("argument_list"))
        ).ToContext();

        //Parse It
        var lexres = lexer.Lex("let abc = \"Hello World\";");
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(
            lexres,
            Newtonsoft.Json.Formatting.Indented,
            new Newtonsoft.Json.JsonSerializerSettings() {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            }
        ));
        //TODO: parser.Parse(lexres);
        var pout = parser.Parse(lexres);
    }
}

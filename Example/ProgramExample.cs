using System.ComponentModel;
using System.Security.Principal;
using Parseus.Lexer;
using Parseus.Parser;
using static Parseus.Parser.Parseus<Token>;

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

		//Parser
		var tree = Parser(
			Rule("program", Repeat(RuleRef("statement"))),
			Rule("statement", Alt(
				//If case
				Branch(Token("if")),
				Branch(Token("fnc"), Parse("fncIdentifier", RuleRef("identifier"), Token.COMMENT))
			))
		);

		Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(
			tree, 
			Newtonsoft.Json.Formatting.Indented, 
			new Newtonsoft.Json.JsonSerializerSettings() {
				NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
			}
		));

		var nodes = new Node<Token>("program", null, Token.STRUCT, null, new Node<Token>("statement", null, Token.IDENTIFIER, null));
		Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(
			nodes,
			Newtonsoft.Json.Formatting.Indented,
			new Newtonsoft.Json.JsonSerializerSettings() {
				NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
			}
		));
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
	}
}
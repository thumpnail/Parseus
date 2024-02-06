using System.ComponentModel;
using System.Security.Principal;
using Parseus.Lexer;
using Parseus.Parser;
using static Parseus.Parser.ParserModule;

class Program {
	class Tk {
		public const int 
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
			EQUALS = 11;
    }
	static void Main(string[] args) {
		var lexer = new Lexer()
			.skipable(Tk.COMMENT, @"\/\/.*")
            .child(Tk.EOL, Environment.NewLine)
            .child(Tk.CLASS, "class")
            .child(Tk.STRUCT, "struct")
            .child(Tk.FNC, "fnc")
            .child(Tk.LET, "let")
            .child(Tk.VAR, "var")
            .child(Tk.SEMICOLON, ";")
            .child(Tk.IDENTIFIER, "[a-zA-Z_][a-zA-Z0-9_]*")
            .child(Tk.STRING, @"'(\\.|[^'\\])*'", "\"" + @"(\\.|[^" + "\"" + @"\\])*" + "\"")
            .child(Tk.NUMBER, @"-?(0[xX][0-9a-fA-F]+|\d*[,.]\d+([eE][+-]?\d+)?|\d+([,.]\d*)?([eE][+-]?\d+)?)");

		//Parser
		var parser = new RuleList(
			//program: { assign-statement }; => program: ( { assign-statement } );
			Rule("program", Tk.CLASS, Alt(
				Group(Repeat(RefRule("assign-statement")))
			)),
			//assign-statement: ('let'|'var') ID '=' STRING;
			Rule("assign-statement", Alt(
				Group(Alt(
					Group(Lit(Tk.LET)),
					Group(Lit(Tk.VAR))
				),
				Lit(Tk.IDENTIFIER),
				// Checks for token on parsing 
				Lit(Tk.EQUALS), // since this is not string dependent, we can ignore tokens and maybe even a lexer in that regard
				// finds subrule and parses it
				Lit(Tk.STRING), 
				// Checks just for the literal
				Lit(";"))
			))
		);
		//Parse It
		var lexres = lexer.Lex("let abc = \"Hello World\";");
		parser.Parse(lexres);
	}
}
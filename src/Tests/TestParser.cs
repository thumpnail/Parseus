using Parseus.Parser.Common;
using Parseus.Parser.Implicit;
namespace Parseus.Tests;

public static class Tokens {
	public const string NONE = "NONE";
	
	public const string DEF = "DEF";
	public const string FNC = "FNC";
	public const string RET = "RET";
	public const string IFF = "IFF";
	public const string ELF = "ELF";
	public const string ELS = "ELS";
	public const string EXT = "EXT";
	public const string WHL = "WHL";
	public const string FOR = "FOR";
	public const string LET = "LET";
	public const string INC = "INC";
	public const string TBL = "TBL";
	public const string PCK = "PCK";
	public const string ERR = "ERR";
	public const string CLL = "CLL";
	public const string SET = "SET";
	
	public const string EQL = "EQL";
	public const string NEQ = "NEQ";
	public const string LSS = "LSS";
	public const string GTR = "GTR";
	public const string LEQ = "LEQ";
	public const string GEQ = "GEQ";
	public const string MOD = "MOD";
	public const string POW = "POW";
	public const string AND = "AND";
	public const string OR = "OR";
	public const string XOR = "XOR";
	public const string NOT = "NOT";
	public const string SHL = "SHL";
	public const string SHR = "SHR";
	
	public const string PLUS = "PLUS";
	public const string MINUS = "MINUS";
	public const string STAR = "STAR";
	public const string SLASH = "SLASH";
	public const string PERCENT = "PERCENT";
	public const string COLON = "COLON";
	
	public const string NULL = "NULL";
	public const string TRUE = "TRUE";
	public const string FALSE = "FALSE";
	public const string IDENTIFIER = "IDENTIFIER";
	public const string STRING = "STRING";
	public const string NUMBER = "NUMBER";
	public const string EOL = "EOL";
}
public class TestParser : BaseParser {
	const string ANY = "[.]";
	const string STRING = $"\"{ANY}\"";
	const string WORD = "[a-zA-Z_][a-zA-Z0-9_]*";
	const string IDENTIFIER = $"[\\.]?{WORD}([\\.]{WORD})*([\\:]{WORD})?";
	const string DIGIT = "[0-9]";
	const string NUMBER = $"{DIGIT}+(\\.{DIGIT}+)?";
	private static readonly Parseus.Lexer.Lexer lexer = new Parseus.Lexer.Lexer()
		.Skippable(Tokens.NONE, @"\/\/.*")
		//Keywords
		.Child(Tokens.DEF, "def")
		.Child(Tokens.FNC, "fnc")
		.Child(Tokens.RET, "ret")
		.Child(Tokens.IFF, "iff")
		.Child(Tokens.ELF, "elf")
		.Child(Tokens.ELS, "els")
		.Child(Tokens.EXT, "ext")
		.Child(Tokens.WHL, "whl")
		.Child(Tokens.FOR, "for")
		.Child(Tokens.LET, "let")
		.Child(Tokens.INC, "inc")
		.Child(Tokens.TBL, "tbl")
		.Child(Tokens.PCK, "pck")
		.Child(Tokens.ERR, "err")
		.Child(Tokens.CLL, "cll")
		.Child(Tokens.SET, "set")
		
		// Operators
		.Child(Tokens.COLON, ":")
		.Child(Tokens.EQL, "==")
		.Child(Tokens.NEQ, "!=")
		.Child(Tokens.LSS, "<")
		.Child(Tokens.GTR, ">")
		.Child(Tokens.LEQ, "<=")
		.Child(Tokens.GEQ, ">=")
		.Child(Tokens.MOD, "%")
		.Child(Tokens.POW, "\\*\\*")
		.Child(Tokens.AND, "\\&\\&")
		.Child(Tokens.OR, "\\|\\|")
		.Child(Tokens.XOR, "^^")
		.Child(Tokens.NOT, "!")
		.Child(Tokens.SHL, "<<")
		.Child(Tokens.SHR, ">>")
		.Child(Tokens.PLUS, "\\+")
		.Child(Tokens.MINUS, "-")
		.Child(Tokens.STAR, "\\*")
		.Child(Tokens.SLASH, "/")
		//Literals
		.Child(Tokens.NULL, "null")
		.Child(Tokens.TRUE, "true")
		.Child(Tokens.FALSE, "false")
		
		// regex
		.Skippable(Tokens.NONE, @"\s+")
		.Skippable(Tokens.NONE, @"#.*")
		.Skippable(Tokens.EOL, Environment.NewLine)
		.Child(Tokens.IDENTIFIER, IDENTIFIER)
		.Child(Tokens.STRING, "\"" + @"(\\.|[^" + "\"" + @"\\])*" + "\"")
		.Child(Tokens.STRING, @"'(\\.|[^'\\])*'")
		.Child(Tokens.NUMBER, @"-?(0[xX][0-9a-fA-F]+|\d*[.]\d+([eE][+-]?\d+)?|\d+([.]\d*)?([eE][+-]?\d+)?)");

	public override Script Parse(string src) {
		var lexResult = lexer.Lex(src);
		var context = new BasicAParserContext(lexResult.result.ToArray());
		var state = new CancellationState();
		return ScriptParser.Parse(new BaseParserContext(context, state));
	}

	public class Script() {
		public List<VariableStatement> variableStatements = new();
	}
	public class VariableStatement() {
		public string identifier;
		public List<string> items = new();
	}
	private static readonly Parser<Script> ScriptParser = new((c, self) => {
		Repeat(c, c => {
			Node(c, VariableStatementParser, s => {
				self.variableStatements.Add(s);
			});
		});
	});
	private static readonly Parser<VariableStatement> VariableStatementParser = new((c, self) => {
		Token(c, Tokens.LET, out _);
		Token(c, Tokens.IDENTIFIER, out self.identifier);
		Repeat(c, c => {
			Token(c, Tokens.NUMBER, n => {
				self.items.Add(n);
			});
		});
	});
}
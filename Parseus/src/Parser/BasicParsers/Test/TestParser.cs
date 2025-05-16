using Parseus.Lexer;
using Parseus.Lexer.Helper;
using Parseus.Parser.Common;
using Parseus.Parser.Explicit;
using Parseus.Parser.Implicit;
using Parseus.Util;
namespace Parseus.Parser.BasicParsers.Test;

class TK {
    public const string colon = "colon";
    public const string var = "kw_var";
    public const string identifier = "identifier";
    public const string openBracket = "openBracket";
    public const string closeingBracket = "closeingBracket";
    public const string openBraces = "openBraces";
    public const string closeingBraces = "closeingBraces";
    public const string equal = "equal";
    public const string eol = "eol";
    public const string @string = "@string";
    public const string number = "number";
    public const string comma = "comma";
}

public class TestRoot {
    public Identifier identifier;
    public bool isVar;
    public List<ValuePair> values = new List<ValuePair>();
}

public class Identifier {
    public string value;
    public override string ToString() {
        return $"{value}";
    }
}

public class ValuePair {
    public Identifier Name;
    public string Value;
}

public class TestBaseParser : Implicit.BaseParser {
    const string ANY = "[.]";
    const string STRING = $"\"{ANY}\"";
    const string WORD = "[a-zA-Z_][a-zA-Z0-9_]*";
    const string IDENTIFIER = $"[\\.]?{WORD}([\\.]{WORD})*([\\:]{WORD})?";
    const string DIGIT = "[0-9]";
    const string NUMBER = $"{DIGIT}+(\\.{DIGIT}+)?";
    //?var ident = ident(parameters)
    private Lexer.Lexer<string> lexer = new Lexer<string>()
        .Child(TK.var, @"var")
        .Child(TK.openBracket, @"\(")
        .Child(TK.closeingBracket, @"\)")
        .Child(TK.equal, "=")
        .Child(TK.comma, ",")
        .Child(TK.colon, ":")
        .Skipable(TK.eol, Environment.NewLine)
        .Child(TK.identifier, IDENTIFIER)
        .Child(TK.@string, "\"" + @"(\\.|[^" + "\"" + @"\\])*" + "\"")
        .Child(TK.@string, @"'(\\.|[^'\\])*'")
        .Child(TK.number, @"-?(0[xX][0-9a-fA-F]+|\d*[.]\d+([eE][+-]?\d+)?|\d+([.]\d*)?([eE][+-]?\d+)?)");

    public override TestRoot Parse(string src) {
        var lexres = lexer.Lex(src);
        Console.WriteLine(string.Join("\n", lexres.result.Select(x => $"({x.token}:{(x.Value.Contains("\n") ? "<newline>" : x.Value)})").ToList()));
        var root = TestRootParser.Parse((new BasicParserContext(lexres.ToTokens().ToArray()), new CancelationToken()));
        return root;
    }
    // ?#var <ident> = { <ident> : <number> }
    private static Parser<TestRoot> TestRootParser = new((c, self) => {
        Literal(c, "var", out self.isVar);
        Node(c, IdentifierParser, out self.identifier);
        Opt(c, (c) => {
            Token(c, TK.equal, out _);
            Token(c, TK.openBracket, out _);
            Opt(c, (c) => {
                Repeat(c, (c) => {
                    Node(c, valuePairParser, val => {
                        self.values.Add(val);
                    });
                });
            });
            Token(c, TK.closeingBracket, out _);
        });
    });
    private static Parser<ValuePair> valuePairParser = new((context, self) => {
        Node(context, IdentifierParser, out self.Name);
        Token(context, TK.colon, out _);
        Token(context, TK.number, out self.Value);
    });
    private static Parser<Identifier> IdentifierParser = new((context, self) => {
        Token(context, TK.identifier, out self.value);
    });
}
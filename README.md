# Information
__There might be happening an whole rewrite of this "lib".__
__Currently finishing up a single file, regex based, Lexer.__
__Might add a simple parser here too.__
## Sample of Usage of the WIP lexer
```csharp
public static class Program {
    public enum Token { CLASS, STRUCT, FNC, LET, VAR, NUMBER, STRING, IDENTIFIER,
        COMMENT,
        SEMICOLON,
        EOL
    }

    public static void Main(string[] args) {
        //Source
        var src = "let somename := \"Hello World\";" + Environment.NewLine+"//"+"this is a comment"+Environment.NewLine+"let somenum : 12.21;"+Environment.NewLine;
        //Creating a Lexer Instance
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

        var lexerResult = lexer.Lex(src);

        lexerResult.result.ForEach(element => {
            Console.WriteLine($"({element.token}, {element.value}):({element.index}, {element.length})");
        });
    }
}
```

# Parseus
Experimental Parser based on Ebnf.
The parser uses 'Primitives' known from Ebnf like repeats ``{ ... }`` optionals ``[ ... ]`` and alternitives ``( ... | ... | ... )``.
That makes the parser more maintainable because it is rule based and simple.

Currently ther is not a seamless integration with the lexer i created.

Currently just a template for my Parser generator in [Vlang](https://www.github.com/vlang/v)

Basic usage:
```csharp
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
```

# Documentation

The [Preprocessor](https://github.com/thumpnail/Parseus/blob/main/Lex.cs#L181) takes a program as lines and seperates those in 'words' based on symbols/operators and actual words.

## Lexer
The [Lexer](https://github.com/thumpnail/Parseus/blob/main/Lex.cs#L293) goes over the wordlist and assigns a token to each one based on the Dictionaries provided.

The New [Lexer](https://github.com/thumpnail/Parseus/blob/Parseus-Rework/Lexer/Lexer.cs) which is generic, simple, drop in capable(thus single file) and regex based.

That one is Extremely simple.

I actually didnt check for Performance, so it might be actually pretty bad.


## Ast
The [Ast](https://github.com/thumpnail/Parseus/blob/main/Ast.cs) contains the structs for representing the Abstract Syntax Tree, like expressions and Statemnts.

## Ast Generator
The [AstGen](https://github.com/thumpnail/Parseus/blob/main/AstGen.cs) generates the AST from a linear string of token tuple. That is basically the Parser and I used a Stack- Based- Expression- Parser.

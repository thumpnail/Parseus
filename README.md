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
        Console.WriteLine("Hello World");
        var lexer = CreateLexer();
        var lexerResult = lexer.Lex(
            "let somename := \"Hello World\";" + Environment.NewLine+
            "//"+"this is a comment"+Environment.NewLine+
            "let somenum : 12.21;"+Environment.NewLine
        );
        lexerResult.result.ForEach(element => {
            Console.WriteLine($"({element.token}, {element.value}):({element.index}, {element.length})");
        });
    }
    private static Lexer<Token> CreateLexer() {
        var lexer =
            new Lexer<Token>()
                // Keywords
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
        return lexer;
    }
}
```
# branches
[Rework Branch][https://github.com/thumpnail/Parseus/tree/Parseus-Rework]

# Parseus
Experimental Parser with Source to Ast to Source Code Generator

Currently just a template for my Parser generator in [Vlang](https://www.github.com/vlang/v)

# Documentation

The [Preprocessor](https://github.com/thumpnail/Parseus/blob/main/Lex.cs#L181) takes a program as lines and seperates those in 'words' based on symbols/operators and actual words.

The [Lexer](https://github.com/thumpnail/Parseus/blob/main/Lex.cs#L293) goes over the wordlist and assigns a token to each one based on the Dictionaries provided.

The [Ast](https://github.com/thumpnail/Parseus/blob/main/Ast.cs) contains the structs for representing the Abstract Syntax Tree, like expressions and Statemnts.

The [AstGen](https://github.com/thumpnail/Parseus/blob/main/AstGen.cs) generates the AST from a linear string of token tuple. That is basically the Parser and I used a Stack- Based- Expression- Parser.

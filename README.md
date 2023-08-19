# Information
__There might be happening an whole rewrite of this "lib".__
__Currently finishing up a single file, regex based, Lexer.__
__Might add a simple parser here too.__

# Parseus
Experimental Parser with Source to Ast to Source Code Generator

Currently just a template for my Parser generator in [Vlang](https://www.github.com/vlang/v)

# Documentation

The [Preprocessor](https://github.com/thumpnail/Parseus/blob/main/Lex.cs#L181) takes a program as lines and seperates those in 'words' based on symbols/operators and actual words.

The [Lexer](https://github.com/thumpnail/Parseus/blob/main/Lex.cs#L293) goes over the wordlist and assigns a token to each one based on the Dictionaries provided.

The [Ast](https://github.com/thumpnail/Parseus/blob/main/Ast.cs) contains the structs for representing the Abstract Syntax Tree, like expressions and Statemnts.

The [AstGen](https://github.com/thumpnail/Parseus/blob/main/AstGen.cs) generates the AST from a linear string of token tuple. That is basically the Parser and I used a Stack- Based- Expression- Parser.

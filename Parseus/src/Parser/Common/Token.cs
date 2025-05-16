<<<<<<< Updated upstream
﻿using Parseus.Lexer;
using Parseus.Util;
namespace Parseus.Parser.Common;

public class Token {
    public string Type { get; private set; }
    public string Value { get; }
    public Token(string type, string value) {
        Type = type;
        Value = value;
    }
    public Token(string type) {
        Type = type;
        Value = type;
    }
    public Token(int type, string value) {
        Type = type.ToString();
        Value = value;
    }
    public Token(TokenElement element) {
        
    }
=======
﻿using Parseus.Lexer;
using Parseus.Util;
namespace Parseus.Parser.Common;

public class Token {
    public string Type { get; private set; }
    public string Value { get; }
    public Token(string type, string value) {
        Type = type;
        Value = value;
    }
    public Token(string type) {
        Type = type;
        Value = type;
    }
    public Token(int type, string value) {
        Type = type.ToString();
        Value = value;
    }
    public Token(TokenElement element) {
        
    }
>>>>>>> Stashed changes
}
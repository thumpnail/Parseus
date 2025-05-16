<<<<<<< Updated upstream
﻿namespace Parseus.Lexer;

public struct TokenElement {
    public string token;
    public string value;
    public int index;
    public int length;
    public bool isSkipable;
    public TokenElement(string token, string value, int index, int length, bool isSkipable = false) {
        this.token = token;
        this.value = value;
        this.index = index;
        this.length = length;
        this.isSkipable = isSkipable;
    }
=======
﻿namespace Parseus.Lexer;

public struct TokenElement {
    public string token;
    public string value;
    public int index;
    public int length;
    public bool isSkipable;
    public TokenElement(string token, string value, int index, int length, bool isSkipable = false) {
        this.token = token;
        this.value = value;
        this.index = index;
        this.length = length;
        this.isSkipable = isSkipable;
    }
>>>>>>> Stashed changes
}
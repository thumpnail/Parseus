namespace Parseus.Lexer;

struct Category {
    public string token;
    public bool isSkipable = false;
    public string[] literals;
    public Category(string token, params string[] literals) {
        this.token = token;
        this.literals = literals;
    }
    public Category(string token, bool skipable, params string[] literals) {
        this.token = token;
        this.literals = literals;
        this.isSkipable = skipable;
    }
}
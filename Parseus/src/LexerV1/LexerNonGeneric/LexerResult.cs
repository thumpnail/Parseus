namespace Parseus.Lexer;

public struct LexerResult {
    public List<TokenElement> result;
    public string source;
    public LexerResult(string source, List<TokenElement> result) {
        this.result = result;
        this.source = source;
    }
}
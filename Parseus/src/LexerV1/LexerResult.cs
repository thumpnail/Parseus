namespace Parseus.Lexer;

public struct LexerResult<T> {
    public List<TokenElement<T>> result;
    public string source;
    public LexerResult(string source, List<TokenElement<T>> result) {
        this.result = result;
        this.source = source;
    }
}
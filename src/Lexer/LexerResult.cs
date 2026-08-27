namespace Parseus.Lexer;

public struct LexerResult {
    public List<TokenElement> result;
    public List<(int lineNumber, int sourceIndex)> lineReference = new();
    public string source;
    public LexerResult(string source, List<TokenElement> result, List<(int lineNumber, int sourceIndex)> lineReference) {
        this.result = result;
        this.source = source;
        this.lineReference = lineReference;
    }
}
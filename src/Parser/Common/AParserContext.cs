using Parseus.Lexer;
namespace Parseus.Parser.Common;

public abstract class AParserContext {
    public int Pos { get; set; }
    public LexerResult LexerResult { get; protected set; }
    public string SourceCode { get; protected set; }
    public abstract TokenElement Consume();
    public abstract TokenElement PeekToken(int offset = 0);
    public abstract bool HasMoreTokens();
    public abstract bool MatchToken(string token);
    public abstract bool MatchValue(string token);
    public abstract bool IsAtEnd();
}
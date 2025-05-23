using Parseus.Lexer;
namespace Parseus.Parser.Common;

public abstract class AParserContext {
    public int pos;
    public abstract TokenElement Consume();
    public abstract TokenElement PeekToken(int offset = 0);
    public abstract bool HasMoreTokens();
    public abstract bool MatchToken(string token);
    public abstract bool MatchValue(string token);
}
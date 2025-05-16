using Parseus.Lexer;
namespace Parseus.Parser.Common;

public interface IParserContext {
    int pos { get; set; }
    List<TokenElement> tokens { get; set; }
    TokenElement Consume();
    TokenElement PeekToken(int offset = 0);
    bool MatchToken(string value);
    bool MatchToken(Predicate<TokenElement> pedicate);
    bool HasMoreTokens();
}
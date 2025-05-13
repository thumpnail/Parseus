using Parseus.Lexer;
namespace Parseus.Parser.Common;

public interface IParserContext {
    int pos { get; set; }
    List<Token> tokens { get; set; }
    Token Consume();
    Token PeekToken(int offset = 0);
    bool MatchToken(string value);
    bool MatchToken(Predicate<Token> pedicate);
    bool HasMoreTokens();
}
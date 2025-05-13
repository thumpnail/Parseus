
using Parseus.Lexer;
namespace Parseus.Parser.Common;
public class BasicParserContext : IParserContext {
    public int pos { get; set; }
    public List<Token> tokens { get; set; }
    public BasicParserContext() {
        this.tokens = new();
        this.pos = 0;
    }
    public BasicParserContext(params Token[] tokens) {
        this.tokens = tokens.ToList();
        this.pos = 0;
    }
    public Token Consume() {
        if (pos < tokens.Count()) {
            return tokens[pos++];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicParserContext)}.Consume");
    }
    public Token PeekToken(int offset = 0) {
        if (pos + offset < tokens.Count()) {
            return tokens[pos + offset];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicParserContext)}.PeekToken");
    }
    public bool MatchToken(string token) => MatchToken(x=>x.Value == token);
    
    public bool MatchToken(Predicate<Token> pedicate) {
        if (HasMoreTokens()) {
            if (pedicate.Invoke(tokens[pos])) {
                return true;
            }
        }
        return false;
    }
    public bool HasMoreTokens() {
        if (pos < tokens.Count()) {
            return true;
        }
        return false;
    }
}
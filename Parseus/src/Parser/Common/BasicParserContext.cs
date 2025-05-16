
using Parseus.Lexer;
namespace Parseus.Parser.Common;
public class BasicParserContext : IParserContext {
    public int pos { get; set; }
    public List<TokenElement> tokens { get; set; }
    public BasicParserContext() {
        this.tokens = new();
        this.pos = 0;
    }
    public BasicParserContext(params TokenElement[] tokens) {
        this.tokens = tokens.ToList();
        this.pos = 0;
    }
    public TokenElement Consume() {
        if (pos < tokens.Count()) {
            return tokens[pos++];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicParserContext)}.Consume");
    }
    public TokenElement PeekToken(int offset = 0) {
        if (pos + offset < tokens.Count()) {
            return tokens[pos + offset];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicParserContext)}.PeekToken");
    }
    public bool MatchToken(string token) => MatchToken(x=>x.Value == token);
    
    public bool MatchToken(Predicate<TokenElement> pedicate) {
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
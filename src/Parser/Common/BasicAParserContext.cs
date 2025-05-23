
using Parseus.Lexer;
namespace Parseus.Parser.Common;
public class BasicAParserContext : AParserContext {
    private int Pos { get; set; }
    private List<TokenElement> Tokens { get; set; }
    public BasicAParserContext() {
        this.Tokens = new();
        this.Pos = 0;
    }
    public BasicAParserContext(params TokenElement[] tokens) {
        this.Tokens = tokens.ToList();
        this.Pos = 0;
    }
    public BasicAParserContext(List<TokenElement> tokens) {
        this.Tokens = tokens;
        this.Pos = 0;
    }
    public BasicAParserContext(LexerResult lexerResult) {
        this.Tokens = lexerResult.result;
        this.Pos = 0;
    }
    public override TokenElement Consume() {
        if (Pos < Tokens.Count()) {
            return Tokens[Pos++];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicAParserContext)}.Consume");
    }
    public override TokenElement PeekToken(int offset = 0) {
        if (Pos + offset < Tokens.Count()) {
            return Tokens[Pos + offset];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicAParserContext)}.PeekToken");
    }
    
    
    public override bool  MatchToken(string token) => PeekToken().Token.Equals(token);
    public override bool  MatchValue(string value) => PeekToken().Value.Equals(value);
    
    public override bool HasMoreTokens() {
        if (Pos < Tokens.Count()) {
            return true;
        }
        return false;
    }
    public override string ToString() {
        return $"{pos}";
    }
}
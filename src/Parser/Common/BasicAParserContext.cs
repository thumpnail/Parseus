
using Parseus.Lexer;
using Parseus.Parser.Diagnostics;
namespace Parseus.Parser.Common;
public class BasicAParserContext : AParserContext {
	/// <inheritdoc />
	public override int Pos { get; set; }
    private List<TokenElement> Tokens { get; set; }
    
    /// <summary>Original source code for diagnostic reporting.</summary>
    public string? SourceCode { get; private set; }
    
    /// <summary>Precomputed line/column cache for fast lookups.</summary>
    internal LineColumnCache? LineCache { get; private set; }
    
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
    
    /// <summary>Sets the source code and precomputes line/column information.</summary>
    public void SetSourceCode(string source) {
        this.SourceCode = source;
        this.LineCache = new LineColumnCache(source);
    }
    
    /// <summary>Gets a TextSpan from the current token position.</summary>
    public TextSpan GetSpanAt(int tokenIndex) {
        if (tokenIndex < 0 || tokenIndex >= Tokens.Count) {
            return TextSpan.At(0);
        }
        var token = Tokens[tokenIndex];
        return new TextSpan(token.Index, token.Length);
    }
    
    /// <summary>Gets a TextSpan for the current token.</summary>
    public TextSpan GetCurrentSpan() => GetSpanAt(Pos);
    
    /// <summary>Gets a TextSpan between two token indices.</summary>
    public TextSpan GetSpanBetween(int startTokenIndex, int endTokenIndex) {
        if (startTokenIndex < 0 || endTokenIndex >= Tokens.Count) {
            return TextSpan.At(0);
        }
        var startToken = Tokens[startTokenIndex];
        var endToken = Tokens[endTokenIndex];
        return new TextSpan(
            startToken.Index, 
            (endToken.Index + endToken.Length) - startToken.Index
        );
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
        return $"{Pos}";
    }
}
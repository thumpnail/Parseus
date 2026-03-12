using System.Runtime.CompilerServices;
using Parseus.Lexer;
using Parseus.Util;
namespace Parseus.Parser.Common;

public class IncrementalParserContext : AParserContext {
	/// <inheritdoc />
	public override int Pos { get; set; }
	private char[] source;
    public IncrementalParserContext() {
        source = [];
        Pos = 0;
    }
    public IncrementalParserContext(char[] source) {
        this.source = source;
        Pos = 0;
    }
    public override TokenElement Consume() {
        if (HasMoreTokens()) {
            var result = new TokenElement();
            var tmp = "";
            for (; HasMoreTokens(); Pos++) {
                var c = source[Pos];
                // Skip white spaces and new lines
                while((c.IsWhiteSpace() || c.IsNewLine()) && HasMoreTokens()) Pos++;
                // Check if we reached the end of the source
                if (c.IsWhiteSpace() || c.IsNewLine()) {
                    if (tmp.Length > 0) {
                        result = new(tmp,tmp,Pos-tmp.Length,tmp.Length);
                        Pos++;
                        return result;
                    }
                    continue;
                }
                if (c.IsAlpha() || c.Is('_')) {
                    //word
                    tmp += c;
                }
                if (c.IsDigit()) {
                    //number
                    tmp += c;
                }
                if (c.IsSpecial()) {
                    //Specials
                    tmp += c;
                }
            }
            return result;
        }
        throw new ParseException("Unexpected end of input",$"{typeof(IncrementalParserContext)}.Consume");
    }
    public override TokenElement PeekToken(int offset = 0) {
        if (Pos + offset < source.Count()) {
            var tmp = Pos;
            var result = Consume();
            Pos = tmp;
            return result;
        }
        throw new ParseException("Unexpected end of input",$"{typeof(IncrementalParserContext)}.PeekToken");
    }
    public override bool  MatchToken(string token) => PeekToken().Token.Equals(token);
    public override bool  MatchValue(string value) => PeekToken().Value.Equals(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool HasMoreTokens() {
        if (Pos < source.Count()) {
            return true;
        }
        return false;
    }
}
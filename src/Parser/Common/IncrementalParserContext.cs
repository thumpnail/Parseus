using System.Runtime.CompilerServices;
using Parseus.Lexer;
using Parseus.Util;
namespace Parseus.Parser.Common;

public class IncrementalParserContext : AParserContext {
    private char[] source;
    public IncrementalParserContext() {
        source = [];
        pos = 0;
    }
    public IncrementalParserContext(char[] source) {
        this.source = source;
        pos = 0;
    }
    public override TokenElement Consume() {
        if (HasMoreTokens()) {
            var result = new TokenElement();
            var tmp = "";
            for (; HasMoreTokens(); pos++) {
                var c = source[pos];
                // Skip white spaces and new lines
                while((c.IsWhiteSpace() || c.IsNewLine()) && HasMoreTokens()) pos++;
                // Check if we reached the end of the source
                if (c.IsWhiteSpace() || c.IsNewLine()) {
                    if (tmp.Length > 0) {
                        result = new(tmp,tmp,pos-tmp.Length,tmp.Length);
                        pos++;
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
        if (pos + offset < source.Count()) {
            var tmp = pos;
            var result = Consume();
            pos = tmp;
            return result;
        }
        throw new ParseException("Unexpected end of input",$"{typeof(IncrementalParserContext)}.PeekToken");
    }
    public override bool  MatchToken(string token) => PeekToken().Token.Equals(token);
    public override bool  MatchValue(string value) => PeekToken().Value.Equals(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool HasMoreTokens() {
        if (pos < source.Count()) {
            return true;
        }
        return false;
    }
}
using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static TK_t TK(int token)  {
        return new TK_t() { token = token };
    }

    public struct TK_t : IEbnfElement {
        public int token;
        public AstNode ParseElement(ref ArrayReader ar) {
            throw new NotImplementedException();
        }
    }
}
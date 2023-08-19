using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static TK_t<T> TK<T>(T token) where T : Enum {
        return new TK_t<T>() { token = token };
    }

    public struct TK_t<T> : IEbnfElement<T> where T : Enum {
        public T token;
        public AstNode<T> ParseElement(ref ArrayReader<TokenElement<T>> ar) {
            throw new NotImplementedException();
        }
    }
}
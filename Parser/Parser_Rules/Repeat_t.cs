using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Repeat_t<T> Repeat<T>(params IEbnfElement<T>[] childs) where T : Enum {
        return new Repeat_t<T>() { childs = childs };
    }

    public struct Repeat_t<T> : IEbnfElement<T> where T : Enum {
        public IEbnfElement<T>[] childs;
        public AstNode<T> ParseElement(ref ArrayReader<TokenElement<T>> ar) {
            throw new NotImplementedException();
        }
    }
}
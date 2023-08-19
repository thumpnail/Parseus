using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Repeat_t Repeat(params IEbnfElement[] childs)  {
        return new Repeat_t() { childs = childs };
    }

    public struct Repeat_t : IEbnfElement  {
        public IEbnfElement[] childs;
        public AstNode ParseElement(ref ArrayReader ar) {
            throw new NotImplementedException();
        }
    }
}
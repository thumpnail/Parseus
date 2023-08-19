using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Optional_t Optional(params IEbnfElement[] childs)  {
        return new Optional_t() { childs = childs };
    }

    public struct Optional_t : IEbnfElement  {
        public IEbnfElement[] childs;

		public bool HasToken(T t) {
            //TODO: implement
            //if(childs.First().HasToken(t)) return true;
            return false;
		}

		public AstNode ParseElement(ref ArrayReader ar) {
            throw new NotImplementedException();
        }
    }
}
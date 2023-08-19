using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Group_t Group(params IEbnfElement[] childs)  {
        return new Group_t() { childs = childs };
    }

    public struct Group_t : IEbnfElement  {
        public IEbnfElement[] childs;

		public bool HasToken(T t) {
            if(childs.First().HasToken(t)) return true;
            return false;
		}

		public AstNode ParseElement(ref ArrayReader ar) {
            throw new NotImplementedException();
        }
        
    }
}
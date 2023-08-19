using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Group_t<T> Group<T>(params IEbnfElement<T>[] childs) where T : Enum {
        return new Group_t<T>() { childs = childs };
    }

    public struct Group_t<T> : IEbnfElement<T> where T : Enum {
        public IEbnfElement<T>[] childs;

		public bool HasToken(T t) {
            if(childs.First().HasToken(t)) return true;
            return false;
		}

		public AstNode<T> ParseElement(ref ArrayReader<TokenElement<T>> ar) {
            throw new NotImplementedException();
        }
        
    }
}
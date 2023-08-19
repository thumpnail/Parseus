using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser; 

public static partial class ParserModule {
    public static Alt_t<T> Alt<T>(params Group_t<T>[] childs) where T : Enum {
        return new Alt_t<T>() { childs = childs };
    }

    public struct Alt_t<T> : IEbnfElement<T> where T : Enum {
        public Group_t<T>[] childs;

		public bool HasToken(T t) {
            foreach(var group in childs) {
                if(group.childs.First().HasToken(t)) return true;
            }
            return false;
		}

		public AstNode<T> ParseElement(ref ArrayReader<TokenElement<T>> ar) {
            foreach(var item in childs) {
                
            }
            return default;
        }		
	}
}
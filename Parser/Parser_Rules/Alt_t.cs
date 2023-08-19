using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser; 

public static partial class ParserModule {
    public static Alt_t Alt(params Group_t[] childs)  {
        return new Alt_t() { childs = childs };
    }

    public struct Alt_t : IEbnfElement  {
        public Group_t[] childs;

		public bool HasToken(T t) {
            foreach(var group in childs) {
                //TODO: implement
                //if(group.childs.First().HasToken(t)) return true;
            }
            return false;
		}

		public AstNode ParseElement(ref ArrayReader ar) {
            foreach(var item in childs) {
                
            }
            return default;
        }		
	}
}
using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static RefRule_t<T> RefRule<T>(string name) where T : Enum {
        return new RefRule_t<T>() {
            name = name
        };
    }

    public struct RefRule_t<T> : IEbnfElement<T> where T : Enum {
        public string name;

		public bool HasToken(T t) {
			
		}

		public AstNode<T> ParseElement(ref ArrayReader<TokenElement<T>> ar) {
            throw new NotImplementedException();
        }
    }
}
using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Literal_t<T> Literal<T>(string value, T ttype = default(T)) where T : Enum {
        return new Literal_t<T>() {
            ttype = ttype,
            value = value
        };
    }

    public struct Literal_t<T> : IEbnfElement<T> where T : Enum {
        public T ttype;
        public string value;

		public bool HasToken(T t) {
            if(ttype.Equals(t)) return true;
            return false;
		}

		public AstNode<T> ParseElement(ref ArrayReader<TokenElement<T>> ar) {
            var node = new AstNode<T>();
            if(ar.Peekc().token.Equals(ttype)) {
                var tmp = ar.Consume();
                node.type = tmp.token;
                node.value = tmp.value;
            } else {
                throw new ParserException<T>();
            }
            return node;
        }
    }
}
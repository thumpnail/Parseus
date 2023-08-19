using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Literal_t Literal(string value, int ttype = 0)  {
        return new Literal_t() {
            ttype = ttype,
            value = value
        };
    }

    public struct Literal_t : IEbnfElement {
        public int ttype;
        public string value;

		public bool HasToken(int t) {
            if(ttype.Equals(t)) return true;
            return false;
		}

		public AstNode ParseElement(ref ArrayReader<int> ar) {
            var node = new AstNode();
            if(ar.Peekc().token.Equals(ttype)) {
                var tmp = ar.Consume();
                node.type = tmp.token;
                node.value = tmp.value;
            } else {
                throw new ParserException();
            }
            return node;
        }
    }
}
using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static RefRule_t RefRule(string name)  {
        return new RefRule_t() {
            name = name
        };
    }

    public struct RefRule_t : IEbnfElement  {
        public string name;

		public AstNode ParseElement(ref ArrayReader ar) {
            throw new NotImplementedException();
        }
    }
}
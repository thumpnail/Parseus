using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Rule_t Rule(string name, params IEbnfElement[] childs)  {
        return new Rule_t() { name = name, childs = childs };
    }
    public static Rule_t Rule(string name, int token, params IEbnfElement[] childs)  {
        return new Rule_t() { name = name, token = token, childs = childs };
    }

    public struct Rule_t : IEbnfElement {
        public int token;
        public string name;
        public IEbnfElement[] childs;

		public bool HasToken(int t) {
			throw new NotImplementedException();
		}

		public AstNode ParseElement(ref RuleList rl, AbstractSyntaxTree ast)  {
			throw new NotImplementedException();
		}
	}
}
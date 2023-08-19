using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public static Rule_t<T> Rule<T>(string name, params IEbnfElement<T>[] childs) where T : Enum {
        return new Rule_t<T>() { name = name, childs = childs };
    }
    public static Rule_t<T> Rule<T>(string name, T token, params IEbnfElement<T>[] childs) where T : Enum {
        return new Rule_t<T>() { name = name, token = token, childs = childs };
    }

    public struct Rule_t<T> : IEbnfElement<T> where T : Enum {
        public T token;
        public string name;
        public IEbnfElement<T>[] childs;

		public bool HasToken(T t) {
			throw new NotImplementedException();
		}

		public AstNode<T> ParseElement(ref RuleList<T> rl, AbstractSyntaxTree<T> ast) {
			throw new NotImplementedException();
		}
	}
}
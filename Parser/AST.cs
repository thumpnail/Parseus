using System.Text;
using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public struct AstNode<T> where T : Enum {
	//place to save stuff, like type, and things that needs to be saved dynamicly
	//public Dictionary<string, string> attributes;
	//public Dictionary<string, bool> flags;
	public T? @type;
	public string value;

	//Child Nodes
	public List<AstNode<T>> childs;
	public AstNode() {
		childs = new();
		type = default;
		value = "";
	}
	public override string ToString() {
		var sb = new StringBuilder();
		return base.ToString();
	}
}

public class AbstractSyntaxTree<T> where T: Enum {
	public AstNode<T> root;
	public LexerResult<T> Result;
	public ArrayReader<TokenElement<T>> ar;
	public AbstractSyntaxTree(LexerResult<T> result) {
		Result = result;
		ar = new ArrayReader<TokenElement<T>>(result.result.ToArray());
	}
}
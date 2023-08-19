using System.Text;
using Parseus.Lexer;
using Parseus.Util;

namespace Parseus.Parser;

public struct AstNode  {
	//place to save stuff, like type, and things that needs to be saved dynamicly
	//public Dictionary<string, string> attributes;
	//public Dictionary<string, bool> flags;
	public int? @type;
	public string value;

	//Child Nodes
	public List<AstNode> childs;
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

public class AbstractSyntaxTree<T> where T : Enum {
	public AstNode root;
	public LexerResult<T> Result;
	public ArrayReader<TokenElement<T>> ar;
	public AbstractSyntaxTree(LexerResult<T> result) {
		Result = result;
		ar = new ArrayReader<TokenElement<T>>(result.result.ToArray());
	}
}
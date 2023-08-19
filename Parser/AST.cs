using System.Text;
using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

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

public class AbstractSyntaxTree  {
	public AstNode root;
	public LexerResult Result;
	public ArrayReader ar;
	public AbstractSyntaxTree(LexerResult result) {
		Result = result;
		ar = new ArrayReader(result.result.ToArray());
	}
}
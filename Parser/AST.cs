using System.Text;
using Parseus.Lexer;
using Parseus.Util;

namespace Parseus.Parser;

public struct AstNode {
	public int? @type; // Node Type
	public string value;
	//Child Nodes
	public List<int> childs;
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

public class AbstractSyntaxTree {
	public AstNode root;
	public LexerResult Result;
	public ArrayReader<TokenElement> ar;
	public AbstractSyntaxTree(LexerResult result) {
		Result = result;
		ar = new ArrayReader<TokenElement>(result.result.ToArray());
	}
}
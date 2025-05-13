using System.Text;
using Parseus.Parser.Common;
using Parseus.Parser.Explicit;
namespace Parseus.Parser.Scratches.test;

public class TestNode_VaribleAssignment : Parsable {
    private bool _isLetKeyword = false;
    private bool _isVarKeyword = false;
    private bool _isConstKeyword = false;
    private bool _isPublic;
    private string keyWord;
    private TestNode_Identifier identifier;
    private TestNode_Identifier _typeIdentifier;
    private TestNode_Expression? _expression;

    public TestNode_VaribleAssignment(BasicParserContext context) : base(context) {}

    public override TestNode_VaribleAssignment Parse() {
        ParseOpt(() => ParseLiteral("pub", out _isPublic));
        ParseAlt(
            () => { ParseLiteral("let", out _isLetKeyword); },
            () => { ParseLiteral("var", out _isVarKeyword); },
            () => { ParseLiteral("const", out _isConstKeyword); });
        ParseNode(ref identifier);
        ParseOpt(() => {
            ParseLiteral(":", out _);
            ParseNode(ref _typeIdentifier);
        });
        ParseOpt(() => {
            ParseLiteral("=", out _);
            ParseNode(ref _expression);
        });
        ParseLiteral(";", out _);
        return this;
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.Append("VaribleStatement(");
        sb.Append(_isPublic ? "pub " : "");
        sb.Append(_isLetKeyword ? "let " : _isVarKeyword ? "var " : _isConstKeyword ? "const " : "");
        sb.Append(identifier);
        sb.Append(_typeIdentifier == null ? "" : ": " + _typeIdentifier + " ");
        sb.Append(_expression == null ? "" : _expression);
        sb.Append(")");

        //sb.Append(Util.ToStringUtil.ToFormattedString(this));
        
        return sb.ToString();
    }

    public static void ExecTest() {
        var context = new BasicParserContext(
            [
                new("pub", "pub"), 
                new("let", "let"),
                new("identifier", "x"), 
                new(":", ":"), 
                new("identifier","int"),
                new("=","="),
                new("number", "12"),
                new("operator","+"),
                new("number","23"),
                new(";", ";")
            ]);
        var variable = new TestNode_VaribleAssignment(context);
        var result = variable.Parse();
        Console.WriteLine(result);
    }
}

public record FunctionCallExpression(string value);
public record MathFunctionCallExpression(FunctionCallExpression left, string @operator, FunctionCallExpression right) : FunctionCallExpression(left.value);

public class TestNode_Expression : Parsable {
    public TestNode_Expression() : base() {}
    public TestNode_Expression(BasicParserContext context) : base(context) {}
    FunctionCallExpression left;
    public override TestNode_Expression Parse() {
        ParseToken("number", out string leftValue);
        left = new FunctionCallExpression(leftValue);

        while (Context.HasMoreTokens() && Context.MatchToken("operator")) {
            string op = Context.Consume().Value;
            ParseToken("number", out string rightValue);
            FunctionCallExpression right = new FunctionCallExpression(rightValue);
            left = new MathFunctionCallExpression(left, op, right);
        }
        return this;
    }
    public override string ToString() {
        if (left is MathFunctionCallExpression math) {
            return $"Expression({math.left}, {math.@operator}, {math.right})";
        }
        return $"Expression({left})";
    }
}

public class TestNode_Identifier : Parsable {
    private string identifier = "";
    
    public TestNode_Identifier() : base() {}
    public TestNode_Identifier(BasicParserContext context) : base(context) {}
    public override TestNode_Identifier Parse() {
        ParseToken("identifier", out identifier);
        return this;
    }
    public override string ToString() {
        return "Identifier(" + identifier + ")";
    }
}
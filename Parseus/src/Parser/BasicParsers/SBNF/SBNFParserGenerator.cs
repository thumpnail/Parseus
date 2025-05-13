using System.Text;
namespace Parseus.Parser.BasicParser.SBNF;

public partial class SBNFParserGenerator {
    private StringBuilder sb = new StringBuilder();
    protected void CreateClass(string name, string[] properties, string[] methods, string extends = "Parsable") {
        sb.AppendLine($"public class {name} : {extends}");
        sb.AppendLine("{");
        foreach (var property in properties) {
            sb.AppendLine(property);
        }
        foreach (var method in methods) {
            sb.AppendLine(method);
        }
        sb.AppendLine("}");
        sb.AppendLine("#region ParserCore");
        sb.AppendLine(ParsableClass+"\n");
        sb.AppendLine(ParserExceptionClass+"\n");
        sb.AppendLine(ParserAstNodeClass+"\n");
        sb.AppendLine(ParserTokenClass+"\n");
        sb.AppendLine(ParserContextClass);
        sb.AppendLine("#endregion");
    }
    protected string CreateProperty(string name, string type) {
        var res = new StringBuilder();
        res.Append($"public {type} {name}");
        res.Append("{");
        res.Append("get;");
        res.Append("set;");
        res.AppendLine("}");
        return res.ToString();
    }
    protected string CreateMethod(string name, string[] statements) {
        var res = new StringBuilder();
        res.AppendLine($"public void {name}()");
        res.AppendLine("{");
        foreach (var statement in statements) {
            res.AppendLine(statement);
        }
        res.AppendLine("}");
        return res.ToString();
    }
    protected string CreateStatement(string statement) {
        var res = new StringBuilder();
        res.AppendLine(statement);
        return res.ToString();
    }
    protected string Build() {
        return sb.ToString();
    }
}
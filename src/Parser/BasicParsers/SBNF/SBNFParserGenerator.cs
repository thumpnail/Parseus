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
        // sb.AppendLine(ParsableClass+"\n");
        // sb.AppendLine(ParserExceptionClass+"\n");
        // sb.AppendLine(ParserAstNodeClass+"\n");
        // sb.AppendLine(ParserTokenClass+"\n");
        // sb.AppendLine(ParserContextClass);
        sb.AppendLine("#endregion");
    }
    protected string Build() {
        return sb.ToString();
    }
}
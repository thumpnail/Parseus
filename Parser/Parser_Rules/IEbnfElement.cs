using System.Text;
using ParseKit.Lexer;
using ParseKit.Util;
namespace ParseKit.Parser; 

public static partial class ParserModule {
    public interface IEbnfElement  {
        public AstNode ParseElement(ref RuleList rl, AbstractSyntaxTree ast);
        public bool HasToken(int t);
    }
}
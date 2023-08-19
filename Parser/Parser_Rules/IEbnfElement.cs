using System.Text;
using ParseKit.Lexer;
using ParseKit.Util;
namespace ParseKit.Parser; 

public static partial class ParserModule {
    public interface IEbnfElement<T> where T : Enum {
        public AstNode<T> ParseElement(ref RuleList<T> rl, AbstractSyntaxTree<T> ast);
        public bool HasToken(T t);
    }
}
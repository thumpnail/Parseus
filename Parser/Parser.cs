using ParseKit.Lexer;
using System.Text.Json;
using ParseKit.Util;
using static ParseKit.Parser.ParserModule;

namespace ParseKit.Parser;

public static partial class ParserModule  {    
    public struct RuleList {
        private RuleParser rm;
        private Rule_t[] rules;
        public RuleList(params Rule_t[] rules) {
            this.rules = rules;
        }
        public AbstractSyntaxTree Parse(LexerResult result)  {
            var ast = new AbstractSyntaxTree(result);
            return ast;
        }
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
        //
        //======================================================================
        //
        private AstNode RecParse(AbstractSyntaxTree ast)  {
            AstNode node;
            while (ast.ar.IsEOF()) {
                Console.WriteLine(ast.ar.Consume());
            }
            return default;
        }
        private AstNode? StartParse(AbstractSyntaxTree ast)  {
            return null;
        }
    }
}
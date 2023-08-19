using ParseKit.Lexer;
using System.Text.Json;
using ParseKit.Util;
using static ParseKit.Parser.ParserModule;

namespace ParseKit.Parser;

public static partial class ParserModule {    
    public struct RuleList<T> where T: Enum {
        private RuleParser<T> rm;
        private Rule_t<T>[] rules;
        public RuleList(params Rule_t<T>[] rules) {
            this.rules = rules;
        }
        public AbstractSyntaxTree<T> Parse<T>(LexerResult<T> result) where T : Enum {
            var ast = new AbstractSyntaxTree<T>(result);
            //Build the Ast Based on the Rules
            var r = rules.ToList().ToDictionary(rule => rule.name);
            while(ast.ar.IsEOF()) {
                
            }
            ast.root = r[rules[0].name].ParseElement(ref this, (AbstractSyntaxTree<T>)ast);
            return ast;
        }
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
        //
        //======================================================================
        //
        private AstNode<T> RecParse<T>(AbstractSyntaxTree<T> ast) where T: Enum {
            AstNode<T> node;
            while (ast.ar.IsEOF()) {
                Console.WriteLine(ast.ar.Consume());
            }
            return default;
        }
        private AstNode<T>? StartParse<T>(AbstractSyntaxTree<T> ast) where T: Enum {
            return null;
        }
    }
}
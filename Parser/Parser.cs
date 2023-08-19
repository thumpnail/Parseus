using Parseus.Lexer;
using System.Text.Json;
using Parseus.Util;
using static Parseus.Parser.ParserModule;

namespace Parseus.Parser;

public static partial class ParserModule {    
    public struct RuleList {
        private Rule_t[] rules;
        public RuleList(params Rule_t[] rules) {
            this.rules = rules;
        }
        public AbstractSyntaxTree<T> Parse<T>(LexerResult<T> result) where T : Enum {
            var ast = new AbstractSyntaxTree<T>(result);
            return ast;
        }
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
        //
        //======================================================================
        //
        private AstNode RecParse<T>(AbstractSyntaxTree<T> ast) where T: Enum  {
            AstNode node;
            while (ast.ar.IsEOF()) {
                Console.WriteLine(ast.ar.Consume());
            }
            return default;
        }
        private AstNode? StartParse<T>(AbstractSyntaxTree<T> ast) where T: Enum {
            return null;
        }
    }
}
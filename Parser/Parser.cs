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
        public AbstractSyntaxTree Parse(LexerResult result) {
            var astcontext = new AbstractSyntaxTree(result);
            //Actual Parsing...
            rules.First().Parse(astcontext); //ast works more like a context
            return astcontext;
        }
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
    }
}
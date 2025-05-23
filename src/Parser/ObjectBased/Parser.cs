using Parseus.Parser.Common;
using Parseus.Parser.Implicit;
namespace Parseus.Parser.ObjectBased;

public abstract class Parser {
    public abstract class EbnfNode {
        private bool success;
        public abstract bool Parse(AParserContext ctx, BaseParser.CancellationState state);
    }
    public class Token: EbnfNode {
        private Action<string> onSuccess;
        private readonly string token;
        public Token(string token, Action<string> onSuccess = null) {
            this.token = token;
            this.onSuccess = onSuccess;
        }
        public override bool Parse(AParserContext ctx, BaseParser.CancellationState state) {
            if(!state.Ok) {
                return false;
            }
            if (ctx.PeekToken().Token.Equals(token)) {
                var tokenElement = ctx.Consume();
                onSuccess?.Invoke(tokenElement.Value);
                return true;
            }
            state.Flag($"Token failed: {ctx} - {token}");
            return false;
        }
    }
    public class Literal: EbnfNode {
        private readonly string literal;
        public Literal(string literal) {
            this.literal = literal;
        }
        public override bool Parse(AParserContext ctx, BaseParser.CancellationState state) {
            throw new NotImplementedException();
        }
    }
    public class Opt: EbnfNode {
        public Opt(Group group) {
            
        }
        public override bool Parse(AParserContext ctx, BaseParser.CancellationState state) {
            throw new NotImplementedException();
        }
    }
    public class Repeat: EbnfNode {
        public Repeat(Group nodes) {
            
        }
        public override bool Parse(AParserContext ctx, BaseParser.CancellationState state) {
            throw new NotImplementedException();
        }
    }
    public class Group: EbnfNode {
        public Group(params EbnfNode[] nodes) {
            
        }
        public override bool Parse(AParserContext ctx, BaseParser.CancellationState state) {
            throw new NotImplementedException();
        }
    }
    public class Alt: EbnfNode {
        public Alt(params Group[] groups) {
            
        }
        public override bool Parse(AParserContext ctx, BaseParser.CancellationState state) {
            throw new NotImplementedException();
        }
    }
    public class Node: EbnfNode {
        public override bool Parse(AParserContext ctx, BaseParser.CancellationState state) {
            throw new NotImplementedException();
        }
    }
}
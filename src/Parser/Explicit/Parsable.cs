using Parseus.Parser.Common;
namespace Parseus.Parser.Explicit;

public abstract class Parsable(AParserContext context) {
    protected BaseParserContext ctx = new(context, new());
    
    public abstract Parsable? Parse(BaseParserContext ctx);

    #region Parser Methods

    internal void Alt(params Action<BaseParserContext>[] actions) {
        if (!ctx.state.Ok) return;

        var cpos = ctx.context.pos;
        for (var idx = 0; idx < actions.Length; idx++) {
            actions[idx](ctx);
            if (ctx.state.Ok) {
                return;
            }
            ctx.state.Reset();
            ctx.context.pos = cpos;
        }
        ctx.state.Flag($"Alt failed: {ctx.context.pos}");
    }

    internal static void Literal(BaseParserContext ctx, string token, Action<bool> action) {
        Literal(ctx, token, out var value);
        if (!ctx.state.Ok) 
            return;
        action(value);
    }
    internal static void Literal(BaseParserContext ctx, string literal, out bool success) {
        if (!ctx.state.Ok) {
            success = false;
            return;
        }
        if (!ctx.context.HasMoreTokens()) {
            success = false;
            ctx.state.Flag($"Unexpected end of input in Literal: {ctx.context.pos}");
            return;
        }
        if (ctx.context.MatchValue(literal)) {
            //gut
            success = literal == ctx.context.Consume().Value;
        } else {
            ctx.state.Flag($"Literal failed: {ctx.context.pos}");
            success = false;
        }
    }

    internal static void Token(BaseParserContext ctx, string token, Action<string> action) {
        Token(ctx, token, out var value);
        if (!ctx.state.Ok) 
            return;
        action(value);
    }
    
    internal static void Token(BaseParserContext ctx, string token, out string value) {
        if (!ctx.state.Ok) {
            value = null;
            return;
        }
        if (!ctx.context.HasMoreTokens()) {
            ctx.state.Flag($"Unexpected end of input: {ctx}");
            value = null;
            return;
        }
        if (ctx.context.MatchToken(token)) {
            value = ctx.context.Consume().Value;
        } else {
            ctx.state.Flag($"Token failed: {ctx.context}");
            value = null;
        }
    }

    internal static void Opt(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.state.Ok) return;
        
        if (ctx.state.Ok) {
            var cpos = ctx.context.pos;
            action(ctx);
            if (!ctx.state.Ok) {
                ctx.context.pos = cpos;
                ctx.state.Reset(); // Reset only the last failed attempt
            }
        }
        // allways reset in a optional
        ctx.state.Reset();
    }
    internal void Repeat(Action<BaseParserContext> action) {
        if (!ctx.state.Ok) return;
        
        // run first wich is required to succeed
        var firstRunPos = ctx.context.pos;
        action(ctx);
        if (!ctx.state.Ok) {
            ctx.context.pos = firstRunPos;
            return;
        }
        
        var startPos = ctx.context.pos;
        while (ctx.state.Ok) {
            var loopPos = ctx.context.pos;
            action(ctx);
            if (!ctx.state.Ok) {
                ctx.context.pos = loopPos;
                ctx.state.Reset(); // Reset only the last failed attempt
                break; // Exit loop, keep previously parsed values
            }
        }
        // If nothing was parsed successfully at all, revert to original position
        if (ctx.context.pos == startPos) {
            ctx.state.Reset();
        }
    }
    //parse subnodes
    internal void Node<T>(ref T? node) where T : Parsable?, new() {
        throw new NotImplementedException();
    }
    internal static void Node<T>(BaseParserContext ctx, Parsable parser, out T value) where T : Parsable, new() {
        if (!ctx.state.Ok) {
            value = new();
            return;
        }
        var cpos = ctx.context.pos;
        value = (T)parser.Parse(ctx);
        if (value == null) {
            ctx.state.Flag($"Node<{typeof(T)}> failed: {ctx.context} | Reason: null value");
            ctx.context.pos = cpos;
            return;
        }
        if (!ctx.state.Ok) {
            ctx.context.pos = cpos;
            ctx.state.Flag($"Node<{typeof(T)}> failed: {ctx.context} | Reason: {ctx.state.reasonStack.Peek()}");
        }
    }
    internal static void Node<T>(BaseParserContext ctx, Parsable parser, Action<T> valueAction) where T : Parsable, new() {
        Node<T>(ctx, parser, out var val);
        if (ctx.state.Ok) {
            valueAction(val);
        }
    }

    #endregion
}
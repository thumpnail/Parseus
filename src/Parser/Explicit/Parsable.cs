using Parseus.Parser.Common;
namespace Parseus.Parser.Explicit;

public abstract class Parsable {
    internal BaseParserContext ctx { get; set; }
    
    public abstract Parsable? Parse(BaseParserContext ctx);

    #region Parser Methods

    internal void Alt(params Action<BaseParserContext>[] actions) {
        if (!ctx.State.Ok) return;

        var cpos = ctx.Context.Pos;
        for (var idx = 0; idx < actions.Length; idx++) {
            actions[idx](ctx);
            if (ctx.State.Ok) {
                return;
            }
            ctx.State.Reset();
            ctx.Context.Pos = cpos;
        }
        ctx.State.Flag($"Alt failed: {ctx.Context.Pos}");
    }

    internal static void Literal(BaseParserContext ctx, string token, Action<bool> action) {
        Literal(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action(value);
    }
    internal static void Literal(BaseParserContext ctx, string literal, out bool success) {
        if (!ctx.State.Ok) {
            success = false;
            return;
        }
        if (!ctx.Context.HasMoreTokens()) {
            success = false;
            ctx.State.Flag($"Unexpected end of input in Literal: {ctx.Context.Pos}");
            return;
        }
        if (ctx.Context.MatchValue(literal)) {
            //gut
            success = literal == ctx.Context.Consume().Value;
        } else {
            ctx.State.Flag($"Literal failed: {ctx.Context.Pos}");
            success = false;
        }
    }

    internal static void Token(BaseParserContext ctx, string token, Action<string> action) {
        Token(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action(value);
    }
    
    internal static void Token(BaseParserContext ctx, string token, out string value) {
        if (!ctx.State.Ok) {
            value = null;
            return;
        }
        if (!ctx.Context.HasMoreTokens()) {
            ctx.State.Flag($"Unexpected end of input: {ctx}");
            value = null;
            return;
        }
        if (ctx.Context.MatchToken(token)) {
            value = ctx.Context.Consume().Value;
        } else {
            ctx.State.Flag($"Token failed: {ctx.Context}");
            value = null;
        }
    }

    internal static void Opt(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.State.Ok) return;
        
        if (ctx.State.Ok) {
            var cpos = ctx.Context.Pos;
            action(ctx);
            if (!ctx.State.Ok) {
                ctx.Context.Pos = cpos;
                ctx.State.Reset(); // Reset only the last failed attempt
            }
        }
        // allways reset in a optional
        ctx.State.Reset();
    }
    internal void Repeat(Action<BaseParserContext> action) {
        if (!ctx.State.Ok) return;
        
        // run first wich is required to succeed
        var firstRunPos = ctx.Context.Pos;
        action(ctx);
        if (!ctx.State.Ok) {
            ctx.Context.Pos = firstRunPos;
            return;
        }
        
        var startPos = ctx.Context.Pos;
        while (ctx.State.Ok) {
            var loopPos = ctx.Context.Pos;
            action(ctx);
            if (!ctx.State.Ok) {
                ctx.Context.Pos = loopPos;
                ctx.State.Reset(); // Reset only the last failed attempt
                break; // Exit loop, keep previously parsed values
            }
        }
        // If nothing was parsed successfully at all, revert to original position
        if (ctx.Context.Pos == startPos) {
            ctx.State.Reset();
        }
    }
    //parse subnodes
    internal void Node<T>(ref T? node) where T : Parsable?, new() {
        throw new NotImplementedException();
    }
    internal static void Node<T>(BaseParserContext ctx, Parsable parser, out T value) where T : Parsable, new() {
        if (!ctx.State.Ok) {
            value = new();
            return;
        }
        var cpos = ctx.Context.Pos;
        value = (T)parser.Parse(ctx);
        if (value == null) {
            ctx.State.Flag($"Node<{typeof(T)}> failed: {ctx.Context} | Reason: null value");
            ctx.Context.Pos = cpos;
            return;
        }
        if (!ctx.State.Ok) {
            ctx.Context.Pos = cpos;
            ctx.State.Flag($"Node<{typeof(T)}> failed: {ctx.Context} | Reason: {ctx.State.reasonStack.Peek()}");
        }
    }
    internal static void Node<T>(BaseParserContext ctx, Parsable parser, Action<T> valueAction) where T : Parsable, new() {
        Node<T>(ctx, parser, out var val);
        if (ctx.State.Ok) {
            valueAction(val);
        }
    }

    #endregion
}
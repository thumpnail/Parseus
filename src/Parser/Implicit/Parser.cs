using System.Runtime.CompilerServices;
using Parseus.Parser.Common;
namespace Parseus.Parser.Implicit;

public record BaseParserContext(AParserContext context, BaseParser.CancellationState state);

public abstract class BaseParser {
    public delegate void RefAction<T1,T2>(T1 ctx, ref T2 self);
    internal bool DEBUG = false;
    internal LogLevel LogLevel;
    internal static StreamWriter LogWriter = new StreamWriter("./log.txt");
    internal AParserContext context;
    public abstract object Parse(string src);

    public BaseParser() {
        this.LogLevel = LogLevel.none;
        this.context = new BasicAParserContext();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal static void Repeat(BaseParserContext ctx, Action<BaseParserContext> action) {
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
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Opt(BaseParserContext ctx, Action<BaseParserContext> action) {
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
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Alt(BaseParserContext ctx, params Action<BaseParserContext>[] actions) {
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
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string token, Action<bool> action) {
        Literal(ctx, token, out var value);
        if (!ctx.state.Ok) 
            return;
        action(value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string literal, out bool success) {
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
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Token(BaseParserContext ctx, string token, Action<string> action) {
        Token(ctx, token, out var value);
        if (!ctx.state.Ok) 
            return;
        action(value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Token(BaseParserContext ctx, string token, out string value) {
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
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T> parser, out T value) where T : class,new() {
        if (!ctx.state.Ok) {
            value = new();
            return;
        }
        var cpos = ctx.context.pos;
        value = parser.Parse(ctx);
        if (!ctx.state.Ok) {
            ctx.context.pos = cpos;
            ctx.state.Flag($"Node<{typeof(T)}> failed: {ctx.context}");
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T> parser, Action<T> valueAction) where T : class,new() {
        Node(ctx, parser, out var val);
        if (ctx.state.Ok) {
            valueAction(val);
        }
    }

    #region Parser_type

    
    protected internal class Parser<T> where T : class, new() {
        // add a fild that returns the default get from this class so it returns the T value
        public Action<BaseParserContext, T> action;
        public Parser(Action<BaseParserContext, T> action) {
            this.action = action;
        }
        public T Parse(BaseParserContext ctx) {
            T self = new T();
            action(ctx, self);
            //parse and get the Ast Type
            return self;
        }
    }

    #endregion

    #region Canclelation_Token

    public class CancellationState {
        public bool Ok = true;
        public Stack<string> reasonStack = new Stack<string>();
        public void Reset() {
            Ok = true;
            if (reasonStack.Count > 0) {
                reasonStack.Pop();
            }
        }
        public void Flag(string reason) {
            Ok = false;
            reasonStack.Push(reason);
        }
        public override string ToString() {
            if (reasonStack.Count > 0) {
                return $"{Ok} | {reasonStack.Peek()}";
            }
            return $"{Ok} | ---";
        }
    }

    #endregion
}
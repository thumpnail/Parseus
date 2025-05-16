using System.Runtime.CompilerServices;
using Parseus.Parser.Common;
namespace Parseus.Parser.Implicit;

public record BaseParserContext(IParserContext parserContext, BaseParser.CancelationToken state);

public abstract class BaseParser {
    internal bool DEBUG = false;
    internal LogLevel LogLevel;
    internal static StreamWriter LogWriter = new StreamWriter("./log.txt");
    internal IParserContext context;
    public abstract object Parse(string src);

    public BaseParser() {
        this.LogLevel = LogLevel.none;
        this.context = new BasicParserContext();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal static void Repeat(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.state.Ok) return;
        var startPos = ctx.parserContext.pos;
        while (ctx.state.Ok) {
            var loopPos = ctx.parserContext.pos;
            action(ctx);
            if (!ctx.state.Ok) {
                ctx.parserContext.pos = loopPos;
                ctx.state.Reset(); // Reset only the last failed attempt
                break; // Exit loop, keep previously parsed values
            }
        }
        // If nothing was parsed successfully at all, revert to original position
        if (ctx.parserContext.pos == startPos) {
            ctx.state.Reset();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Opt(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.state.Ok) return;
        var cpos = ctx.parserContext.pos;
        if (ctx.state.Ok) {
            action(ctx);
        }
        // allways reset in a optional
        ctx.state.Reset();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Alt(BaseParserContext ctx, params Action<BaseParserContext>[] actions) {
        if (!ctx.state.Ok) return;

        var cpos = ctx.parserContext.pos;
        foreach (var action in actions) {
            action(ctx);
            if (ctx.state.Ok) {
                return;
            }
            ctx.state.Reset();
            ctx.parserContext.pos = cpos;
        }
        ctx.state.Flag();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string literal, out bool success) {
        if (!ctx.state.Ok) {
            success = false;
            return;
        }
        if (!ctx.parserContext.HasMoreTokens()) {
            success = false;
            ctx.state.Flag();
            return;
        }
        if (literal == ctx.parserContext.PeekToken().Value) {
            //gut
            success = literal == ctx.parserContext.Consume().Value;
        } else {
            ctx.state.Flag();
            success = false;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Token(BaseParserContext ctx, string token, out string value) {
        if (!ctx.state.Ok) {
            value = null;
            return;
        }
        if (!ctx.parserContext.HasMoreTokens()) {
            ctx.state.Flag();
            value = null;
            return;
        }
        if (token == ctx.parserContext.PeekToken().Token) {
            //gut
            value = ctx.parserContext.Consume().Value;
        } else {
            ctx.state.Flag();
            value = null;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T> parser, out T value) where T : new() {
        if (!ctx.state.Ok) {
            value = new();
            return;
        }
        var cpos = ctx.parserContext.pos;
        value = parser.Parse(ctx);
        if (!ctx.state.Ok) {
            ctx.parserContext.pos = cpos;
            ctx.state.Flag();
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T> parser, Action<T> valueAction) where T : new() {
        Node(ctx, parser, out var val);
        if (ctx.state.Ok) {
            valueAction(val);
        }
    }

    #region Parser_type

    protected internal class Parser<T> where T : new() {
        // add a fild that returns the default get from this class so it returns the T value
        public Action<BaseParserContext, T> action;
        public Parser(Action<BaseParserContext, T> action) {
            this.action = action;
        }
        public T Parse(BaseParserContext ctx) {
            var _self = new T();
            action(ctx, _self);
            //parse and get the Ast Type
            return _self;
        }
    }

    #endregion

    #region Canclelation_Token

    public class CancelationToken {
        public bool Ok = true;
        public void Reset() {
            Ok = true;
        }
        public void Flag() {
            Ok = false;
        }
    }

    #endregion
}
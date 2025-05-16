using System.Runtime.CompilerServices;
using Parseus.Parser.Common;
namespace Parseus.Parser.Implicit;

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
    public BaseParser(IParserContext context) {
        this.LogLevel = LogLevel.none;
        this.context = context;
        var _context = (context, new CancelationToken());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal static void Repeat((IParserContext parserContext, CancelationToken state) ctx, Action<(IParserContext, CancelationToken)> action) {
        if (!ctx.state.Ok) return;
        var cpos = ctx.parserContext.pos;
        action(ctx);
        if (!ctx.state.Ok) {
            ctx.parserContext.pos = cpos;
            ctx.state.Reset();
            return;
        }

        while (ctx.state.Ok) {
            action(ctx);
            if (!ctx.state.Ok) {
                ctx.parserContext.pos = cpos;
                ctx.state.Reset();
                return;
            }
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Opt((IParserContext parserContext, CancelationToken state) context, Action<(IParserContext, CancelationToken)> action) {
        if (!context.state.Ok) return;
        var cpos = context.parserContext.pos;
        if (context.state.Ok) {
            action(context);
        }
        // allways reset in a optional
        context.state.Reset();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Alt((IParserContext parserContext, CancelationToken state) context, params Action<(IParserContext, CancelationToken)>[] actions) {
        if (!context.state.Ok) return;

        var success = false;
        var cpos = context.parserContext.pos;
        foreach (var action in actions) {
            action(context);
            if (context.state.Ok) {
                success = true;
                break;
            }
            context.parserContext.pos = cpos;
        }
        context.state.Flag();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal((IParserContext parserContext, CancelationToken state) ctx, string literal, out bool success) {
        if (!ctx.state.Ok) {
            success = false;
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
    protected internal static void Token((IParserContext parserContext, CancelationToken state) ctx, string token, out string value) {
        if (!ctx.state.Ok) {
            value = null;
            return;
        }
        if (token == ctx.parserContext.PeekToken().Type) {
            //gut
            value = ctx.parserContext.Consume().Value;
        } else {
            ctx.state.Flag();
            value = null;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>((IParserContext parserContext, CancelationToken state) ctx, Parser<T> parser, out T value) where T : new() {
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
    protected internal static void Node<T>((IParserContext parserContext, CancelationToken state) ctx, Parser<T> parser, Action<T> valueAction) where T : new() {
        Node(ctx, parser, out var val);
        if (ctx.state.Ok) {
            valueAction(val);
        }
    }

    #region Parser_type

    protected internal class Parser<T> where T : new() {
        // add a fild that returns the default get from this class so it returns the T value
        public T self { get; private set; }
        public Action<(IParserContext parserContext, CancelationToken state), T> action;
        public Parser(Action<(IParserContext parserContext, CancelationToken state), T> action) {
            this.action = action;
        }
        public T Parse((IParserContext parserContext, CancelationToken state) ctx) {
            self = new();
            action(ctx, self);
            //parse and get the Ast Type
            return self;
        }
    }

    #endregion

    #region Canclelation_Token

    protected internal class CancelationToken {
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

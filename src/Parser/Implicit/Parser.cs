using System.Runtime.CompilerServices;

using Parseus.Parser.Common;

namespace Parseus.Parser.Implicit;

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
	protected internal static void RepeatOpt(BaseParserContext ctx, Action<BaseParserContext> action) {
		Opt(ctx, c => Repeat(c, action));
		Console.WriteLine(ctx.State.ToString("RepeatOpt"));
	}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal static void Repeat(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.IsOk) return;
		
		// save start position for potential revert on first failure
		var startPos = ctx.Context.pos;
		// first run must succeed and make progress
		action.Invoke(ctx);

		// if the first run failed, restore start position and leave the failure flagged
		if (!ctx.IsOk) {
			ctx.Context.pos = startPos; // revert any consumed tokens
			return;
		}

		// if the first run didn't advance, it's an error: revert and flag
		//if (ctx.context.pos == startPos) {
		//	ctx.context.pos = startPos;
		//	ctx.state.Flag("Repeat failed: no progress at pos " + startPos);
		//	return;
		//}
		
		// subsequent runs: repeat until an attempt fails or makes no progress
		while (true) {
			var prevPos = ctx.Context.pos;
			action.Invoke(ctx);

			// if this attempt failed or made no progress, revert the attempt and clear the failure
			if (!ctx.IsOk) {
				ctx.Context.pos = prevPos; // revert last failed/no-progress attempt
				ctx.State.Reset(); // clear the failure from the last attempt
				break;
			}
		}
		
		Console.WriteLine(ctx.State.ToString("Repeat"));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Opt(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.State.Ok) return;
        
        var cpos = ctx.Context.pos;
        action(ctx);
        if (!ctx.State.Ok) {
            // revert only if action failed
            ctx.Context.pos = cpos;
            ctx.State.Reset(); // Reset only the last failed attempt
        }
		Console.WriteLine(ctx.State.ToString("Opt"));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Alt(BaseParserContext ctx, params Action<BaseParserContext>[] actions) {
        if (!ctx.State.Ok) return;

        var cpos = ctx.Context.pos;
        for (var idx = 0; idx < actions.Length; idx++) {
            actions[idx].Invoke(ctx);
            if (ctx.State.Ok) {
				Console.WriteLine(ctx.State.ToString("Alt<"+actions.Length+">"));
                return;
            }
            ctx.State.Reset();
            ctx.Context.pos = cpos;
        }
        ctx.State.Flag($"Alt failed: {ctx.Context.pos}");
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string token, Action<bool> action) {
        Literal(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action(value);
		Console.WriteLine(ctx.State.ToString("Literal<"+token+">"));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string literal, out bool success) {
        if (!ctx.State.Ok) {
            success = false;
            return;
        }
        if (!ctx.Context.HasMoreTokens()) {
            success = false;
            ctx.State.Flag($"Unexpected end of input in Literal at pos {ctx.Context.pos}");
            return;
        }
        if (ctx.Context.MatchValue(literal)) {
            // consume the matching value and mark success
            ctx.Context.Consume();
            success = true;
        } else {
            ctx.State.Flag($"Literal failed at pos {ctx.Context.pos}: expected '{literal}'");
            success = false;
        }
		Console.WriteLine(ctx.State.ToString("Literal<"+literal+">"));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Token(BaseParserContext ctx, string token, Action<string> action) {
        Token(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action(value);
		Console.WriteLine(ctx.State.ToString("Token<"+token+">"));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Token(BaseParserContext ctx, string token, out string value) {
        if (!ctx.State.Ok) {
            value = null!;
            return;
        }
        if (!ctx.Context.HasMoreTokens()) {
            ctx.State.Flag($"Unexpected end of input at pos {ctx.Context.pos}, expected token '{token}'");
            value = null!;
            return;
        }
        if (ctx.Context.MatchToken(token)) {
            value = ctx.Context.Consume().Value;
        } else {
            ctx.State.Flag($"Token failed at pos {ctx.Context.pos}: expected '{token}'");
            value = null!;
        }
		Console.WriteLine(ctx.State.ToString("Token<"+token+">"));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T> parser, out T value) where T : class,new() {
        if (!ctx.State.Ok) {
            // don't allocate a default instance when parsing already failed; return null so callers can see the failure
            value = null!;
            return;
        }
        var cpos = ctx.Context.pos;
        value = parser.Parse(ctx);
        if (!ctx.State.Ok) {
            ctx.Context.pos = cpos;
            ctx.State.Flag($"Node<{typeof(T)}> failed: {ctx.Context}");
        }
		Console.WriteLine(ctx.State.ToString("Node<"+typeof(T).Name+">"));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T> parser, Action<T> valueAction) where T : class,new() {
        Node(ctx, parser, out var val);
        if (ctx.State.Ok) {
            valueAction(val);
        }
		Console.WriteLine(ctx.State.ToString("Node<"+typeof(T).Name+">"));
	}

    #region Parser_type

	public class Parser<T> where T : class, new() {
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
}


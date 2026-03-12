using Parseus.Parser.Common;
using System.Reflection;
using System.Collections.Generic;

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
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected internal static void RepeatOpt(BaseParserContext ctx, Action<BaseParserContext> action) {
		Opt(ctx, c => Repeat(c, action));
		//Console.WriteLine(ctx.State.ToString("RepeatOpt"));
	}
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal static void Repeat(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.IsOkAndNotEof) return;
		
		// save start position for potential revert on first failure
		var startPos = ctx.Context.Pos;
		// first run must succeed and make progress
		action.Invoke(ctx);

		// if the first run failed, restore start position and leave the failure flagged
		if (!ctx.IsOkAndNotEof) {
			ctx.Context.Pos = startPos; // revert any consumed tokens
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
			var prevPos = ctx.Context.Pos;
			action.Invoke(ctx);

			// if this attempt failed or made no progress, revert the attempt and clear the failure
			if (!ctx.IsOkAndNotEof) {
				ctx.Context.Pos = prevPos; // revert last failed/no-progress attempt
				ctx.State.Reset(); // clear the failure from the last attempt
				break;
			}
		}
		
		//Console.WriteLine(ctx.State.ToString("Repeat"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Opt(BaseParserContext ctx, Action<BaseParserContext> action) {
        if (!ctx.State.Ok) return;
        
        var cpos = ctx.Context.Pos;
        action(ctx);
        if (!ctx.State.Ok) {
            // revert only if action failed
            ctx.Context.Pos = cpos;
            ctx.State.Reset(); // Reset only the last failed attempt
        }
		//Console.WriteLine(ctx.State.ToString("Opt"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Alt(BaseParserContext ctx, params Action<BaseParserContext>[] actions) {
        if (!ctx.State.Ok) return;

        var cpos = ctx.Context.Pos;
        for (var idx = 0; idx < actions.Length; idx++) {
            actions[idx].Invoke(ctx);
            if (ctx.State.Ok) {
				//Console.WriteLine(ctx.State.ToString("Alt<"+actions.Length+">"));
                return;
            }
            ctx.State.Reset();
            ctx.Context.Pos = cpos;
        }
        ctx.State.Flag($"Alt failed: {ctx.Context.Pos}");
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string token, Action<bool> action) {
        Literal(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action(value);
		//Console.WriteLine(ctx.State.ToString("Literal<"+token+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string literal, out bool success) {
        if (!ctx.State.Ok) {
            success = false;
            return;
        }
        if (!ctx.Context.HasMoreTokens()) {
            success = false;
            ctx.State.Flag($"Unexpected end of input in Literal at pos {ctx.Context.Pos}");
            return;
        }
        if (ctx.Context.MatchValue(literal)) {
            // consume the matching value and mark success
            ctx.Context.Consume();
            success = true;
        } else {
            ctx.State.Flag($"Literal failed at pos {ctx.Context.Pos}: expected '{literal}'");
            success = false;
        }
		//Console.WriteLine(ctx.State.ToString("Literal<"+literal+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Token(BaseParserContext ctx, string token, Action<string> action) {
        Token(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action(value);
		//Console.WriteLine(ctx.State.ToString("Token<"+token+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Token(BaseParserContext ctx, string token, out string value) {
        if (!ctx.State.Ok) {
            value = null!;
            return;
        }
        if (!ctx.Context.HasMoreTokens()) {
            ctx.State.Flag($"Unexpected end of input at pos {ctx.Context.Pos}, expected token '{token}'");
			ctx.State.ToString($"Token");
            value = null!;
            return;
        }
        if (ctx.Context.MatchToken(token)) {
            var tk = ctx.Context.Consume();
            value = tk.Value;
        } else {
            ctx.State.Flag($"Token failed at pos {ctx.Context.Pos}: expected '{token}'");
			ctx.State.ToString($"Token");
            value = null!;
        }
		//Console.WriteLine(ctx.State.ToString("Token<"+token+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T> parser, out T value) {
        // Works for reference types and interfaces as well as concrete types.
        if (!ctx.State.Ok) {
            value = default!;
            return;
        }
        var cpos = ctx.Context.Pos;

        value = parser.Parse(ctx);
        // If parser signalled failure, revert and keep the failure
        if (!ctx.State.Ok) {
            ctx.Context.Pos = cpos;
            ctx.State.Flag($"Node<{typeof(T)}> failed: {ctx.Context}");
            return;
        }
        // If parser returned success but made no progress and also returned default value, treat as failure
        if (ctx.Context.Pos == cpos && EqualityComparer<T>.Default.Equals(value, default!)) {
            value = default!;
            ctx.State.Flag($"Node<{typeof(T)}> failed: no progress at pos {cpos}");
            return;
        }
        // Otherwise succeed; allow parsers to return a concrete object even if no tokens were consumed
        // (some parsers may legitimately produce values without consuming tokens).
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T>? parser, Action<T> valueAction) {
        if (parser is null) {
            ctx.State.Flag($"Node<{typeof(T)}> failed: parser is null");
            return;
        }
        Node(ctx, parser, out var val);
        if (ctx.State.Ok) {
            valueAction(val);
        }
    }

    /// <summary>
    /// Generic left-fold helper for parsing left-associative binary operator sequences.
    /// Example usage in a grammar:
    ///   FoldLeft(ctx, LogicalAndExpressionParser, Tokens.ORParser, LogicalAndExpressionParser,
    ///       acc => { self.Left = acc.Left; self.Operator = acc.Operator; self.Right = acc.Right; },
    ///       (left, op, right) => new BinaryExpression { Left = left, Operator = op, Right = right });
    /// </summary>
    protected internal static void FoldLeft<TAcc, TOp, TNext>(BaseParserContext ctx,
        Parser<TAcc> firstParser,
        Parser<TOp> opParser,
        Parser<TNext> nextParser,
        Action<TAcc> resultAction,
        Func<TAcc, TOp, TNext, TAcc> combiner)
    {
        if (!ctx.State.Ok) return;

        TAcc acc = default!;
        Node(ctx, firstParser, out acc);
        if (!ctx.State.Ok) return;

        RepeatOpt(ctx, c => {
            TOp op = default!;
            TNext right = default!;
            Node(c, opParser, out op);
            if (!c.State.Ok) return;
            Node(c, nextParser, out right);
            if (!c.State.Ok) return;
            acc = combiner(acc, op, right);
        });

        if (ctx.State.Ok) resultAction(acc);
    }

    #region Parser_type

    public class Parser<T> {
        // Support both legacy Action-based parser bodies (which expect a pre-allocated instance)
        // and newer ref-based parser bodies (which may create and assign an instance, e.g. for interfaces).
        private readonly Action<BaseParserContext, T>? actionBody;
        private readonly RefAction<BaseParserContext, T>? refActionBody;

        public Parser(Action<BaseParserContext, T> action) {
            this.actionBody = action;
        }

        public Parser(RefAction<BaseParserContext, T> refAction) {
            this.refActionBody = refAction;
        }

        public T Parse(BaseParserContext ctx) {
            var startPos = ctx.Context.Pos;
            // Start with default(T). For reference types (including interfaces), this is null.
            T self = default!;

            if (refActionBody != null) {
                // Give the parser a chance to allocate and assign a concrete value
                refActionBody(ctx, ref self);
            } else if (actionBody != null) {
                // For the legacy action form we need to provide an instance to be populated.
                // Try to create one; if that fails (e.g. T is interface) we leave it default and call the action
                // which will likely fail — callers should prefer the refAction form for interfaces.
                try {
                    self = Activator.CreateInstance<T>();
                } catch {
                    // If the type is an interface or abstract, try to create a DispatchProxy-based
                    // proxy that implements the interface and records property sets/gets.
                    try {
                        self = CreateInterfaceProxyInstance<T>();
                    } catch {
                        self = default!;
                    }
                }
                actionBody(ctx, self);
            } else {
                throw new InvalidOperationException($"Parser<{typeof(T)}> has no action body");
            }

            // If the action signalled failure, return what we have (caller will handle revert)
            if (!ctx.State.Ok) {
                return self;
            }

            // If the parser made no progress and also didn't produce a concrete value, signal failure.
            if (ctx.Context.Pos == startPos && EqualityComparer<T>.Default.Equals(self, default!)) {
                ctx.State.Flag($"Parser<{typeof(T)}> failed: no progress at pos {startPos}");
                return self;
            }

            return self;
        }
    }

    // A DispatchProxy that captures property sets/gets into a dictionary so legacy parsers
    // that expect a concrete instance can operate on an interface type without NREs.
    private class InterfaceProxy : DispatchProxy {
        private Dictionary<string, object?> store = new Dictionary<string, object?>();

        public void SetStore(Dictionary<string, object?> s) {
            this.store = s ?? new Dictionary<string, object?>();
        }

        protected override object? Invoke(MethodInfo targetMethod, object?[] args) {
            if (targetMethod == null) return null;
            var name = targetMethod.Name;
            // property setter
            if (name.StartsWith("set_") && args.Length == 1) {
                var prop = name.Substring(4);
                store[prop] = args[0];
                return null;
            }
            // property getter
            if (name.StartsWith("get_") && args.Length == 0) {
                var prop = name.Substring(4);
                if (store.TryGetValue(prop, out var val)) return val;
                // return default for the property type
                var rt = targetMethod.ReturnType;
                if (rt.IsValueType) return Activator.CreateInstance(rt);
                return null;
            }
            // For other methods return default for return type
            var ret = targetMethod.ReturnType;
            if (ret == typeof(void)) return null;
            if (ret.IsValueType) return Activator.CreateInstance(ret);
            return null;
        }
    }

    private static T CreateInterfaceProxyInstance<T>() {
        var t = typeof(T);
        if (!t.IsInterface && !t.IsAbstract) return default!;
        // Create a proxy instance that implements T
        var proxy = DispatchProxy.Create<T, InterfaceProxy>();
        // initialize store
        ((InterfaceProxy)(object)proxy).SetStore(new Dictionary<string, object?>());
        return proxy;
    }

    #endregion
}


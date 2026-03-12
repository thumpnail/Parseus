using System.Runtime.CompilerServices;

using Parseus.Parser.Common;
using Parseus.Parser.Diagnostics;

namespace Parseus.Parser.Implicit;

public abstract class BaseParser {
    //public delegate void RefAction<T1,T2>(T1 ctx, ref T2 self);
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
				ctx.State.FullReset(); // clear the failure from the last attempt
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
            ctx.State.FullReset(); // Reset only the last failed attempt
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
            ctx.State.FullReset();
            ctx.Context.Pos = cpos;
        }
        ctx.State.Flag($"Alt failed: {ctx.Context.Pos}");
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Literal(BaseParserContext ctx, string token, Action<bool> action = null!) {
        Literal(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action?.Invoke(value);
		//Console.WriteLine(ctx.State.ToString("Literal<"+token+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void Literal(BaseParserContext ctx, string literal, out bool success) {
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
    protected internal static void Token(BaseParserContext ctx, string token, Action<string>? action = null!) {
        Token(ctx, token, out var value);
        if (!ctx.State.Ok) 
            return;
        action?.Invoke(value);
		//Console.WriteLine(ctx.State.ToString("Token<"+token+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void Token(BaseParserContext ctx, string token, out string value) {
        if (!ctx.State.Ok) {
            value = null!;
            return;
        }
        if (!ctx.Context.HasMoreTokens()) {
            if (/*DEBUG access*/ false) { }
            ctx.State.Flag($"Unexpected end of input at pos {ctx.Context.Pos}, expected token '{token}'");
            // debug info
            if (/* base debug flag (cannot access instance) */ false) { }
            value = null!;
            return;
        }
        if (ctx.Context.MatchToken(token)) {
            var tk = ctx.Context.Consume();
            value = tk.Value;
        } else {
            // Provide optional debug logging to help trace why tokens are null.
            // Use the global DEBUG flag on BaseParser if enabled.
            try {
                var dbg = typeof(BaseParser).GetField("DEBUG", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
                var isDbg = dbg != null && dbg.GetValue(null) is bool b && b;
                if (isDbg) {
                    string peek = "<no-peek>";
                    try {
                        var tkp = ctx.Context.PeekToken();
                        peek = tkp.Token + "/" + tkp.Value;
                    } catch { peek = "<eof>"; }
                    Console.WriteLine($"[Parseus DEBUG] Token mismatch at pos {ctx.Context.Pos}: expected '{token}', peek={peek}");
                }
            } catch {
                // ignore reflection errors
            }
            ctx.State.Flag($"Token failed at pos {ctx.Context.Pos}: expected '{token}'");
            value = null!;
        }
		//Console.WriteLine(ctx.State.ToString("Token<"+token+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void Node<T>(BaseParserContext ctx, Parser<T> parser, out T value) where T : new(), allows ref struct {
        if (!ctx.State.Ok) {
            // don't allocate a default instance when parsing already failed; return null so callers can see the failure
            value = default;
            return;
        }
        var cpos = ctx.Context.Pos;
        value = parser.Parse(ctx);
        if (!ctx.State.Ok) {
            ctx.Context.Pos = cpos;
            ctx.State.Flag($"Node<{typeof(T)}> failed: {ctx.Context}");
        }
		//Console.WriteLine(ctx.State.ToString("Node<"+typeof(T).Name+">"));
    }
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected internal static void Node<T>(BaseParserContext ctx, Parser<T>? parser, Action<T> valueAction) where T : new(), allows ref struct {
        Node(ctx, parser, out var val);
        if (ctx.State.Ok) {
            valueAction(val);
        }
		//Console.WriteLine(ctx.State.ToString("Node<"+typeof(T).Name+">"));
	}

    /// <summary>
    /// Reports an error with automatic position tracking from the current token.
    /// </summary>
    protected internal static void ReportError(BaseParserContext ctx, string message, string? sourceLabel = null) {
        if (ctx.Context is BasicAParserContext basicCtx) {
            var span = basicCtx.GetCurrentSpan();
            ctx.State.ReportError(message, span, sourceLabel);
        } else {
            ctx.State.ReportError(message, sourceLabel: sourceLabel);
        }
    }

    /// <summary>
    /// Reports a warning with automatic position tracking from the current token.
    /// </summary>
    protected internal static void ReportWarning(BaseParserContext ctx, string message, string? sourceLabel = null) {
        if (ctx.Context is BasicAParserContext basicCtx) {
            var span = basicCtx.GetCurrentSpan();
            ctx.State.ReportWarning(message, span, sourceLabel);
        } else {
            ctx.State.ReportWarning(message, sourceLabel: sourceLabel);
        }
    }

    /// <summary>
    /// Reports a note with automatic position tracking from the current token.
    /// </summary>
    protected internal static void ReportNote(BaseParserContext ctx, string message, string? sourceLabel = null) {
        if (ctx.Context is BasicAParserContext basicCtx) {
            var span = basicCtx.GetCurrentSpan();
            ctx.State.ReportNote(message, span, sourceLabel);
        } else {
            ctx.State.ReportNote(message, sourceLabel: sourceLabel);
        }
    }

    /// <summary>
    /// Sets the source code for diagnostic reporting on the context.
    /// Should be called after creating the parser context.
    /// </summary>
    protected internal static void SetSourceCode(BaseParserContext ctx, string source) {
        if (ctx.Context is BasicAParserContext basicCtx) {
            basicCtx.SetSourceCode(source);
        }
    }

    /// <summary>
    /// Outputs all collected diagnostics to the console using Rust-like formatting.
    /// </summary>
    protected internal static void OutputDiagnostics(BaseParserContext ctx, DiagnosticRenderer.RenderOptions? options = null) {
        if (ctx.State.HasDiagnostics && ctx.Context is BasicAParserContext basicCtx) {
            // Inject source code into diagnostics for code snippet rendering
            foreach (var diag in ctx.State.Diagnostics) {
                if (diag.SourceCode == null && basicCtx.SourceCode != null) {
                    diag.WithSourceCode(basicCtx.SourceCode);
                }
                if (diag.LineCache == null && basicCtx.LineCache != null) {
                    diag.WithLineCache(basicCtx.LineCache);
                }
            }
            DiagnosticRenderer.OutputAll(ctx.State.Diagnostics, options);
        }
    }

    /// <summary>
    /// Gets a formatted diagnostic summary like "error: aborting due to 1 error and 2 warnings"
    /// </summary>
    protected internal static string GetDiagnosticSummary(BaseParserContext ctx) {
        return DiagnosticRenderer.GetSummary(ctx.State.Diagnostics);
    }

    #region Parser_type

	public class Parser<T>(Action<BaseParserContext, T> action) where T : new(), allows ref struct {
        // add a fild that returns the default get from this class so it returns the T value
		public T Parse(BaseParserContext ctx) {
			//Console.WriteLine("Parsing " + typeof(T).Name);
            T self = new();
            action(ctx, self);
            //parse and get the Ast Type
            return self;
        }
    }

    #endregion
}


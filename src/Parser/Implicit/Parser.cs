using Parseus.Parser.Common;

namespace Parseus.Parser.Implicit;

public abstract partial class BaseParser {
	#region CONSTS
	protected internal const string REGEX_ANY = "[.]";
	protected internal const string REGEX_STRING1 = $"\"{REGEX_ANY}\"";
	protected internal const string REGEX_STRING2 = "\"" + @"(\\.|[^" + "\"" + @"\\])*" + "\"";
	protected internal const string REGEX_STRING3 = @"'(\\.|[^'\\])*'";
	protected internal const string REGEX_WORD = "[a-zA-Z_][a-zA-Z0-9_]*";
	protected internal const string REGEX_IDENTIFIER = $"[\\.]?{REGEX_WORD}([\\.]{REGEX_WORD})*([\\:]{REGEX_WORD})?";
	protected internal const string REGEX_DIGIT = "[0-9]";
	protected internal const string REGEX_NUMBER1 = $"{REGEX_DIGIT}+(\\.{REGEX_DIGIT}+)?";

	protected internal const string REGEX_NUMBER2 =
		@"-?(0[xX][0-9a-fA-F]+|\d*[.]\d+([eE][+-]?\d+)?|\d+([.]\d*)?([eE][+-]?\d+)?)";
	#endregion
	//public delegate void RefAction<T1,T2>(T1 ctx, ref T2 self);
	internal bool DEBUG = false;
	internal LogLevel LogLevel = LogLevel.none;
	internal static StreamWriter LogWriter = new StreamWriter("./log.txt");
	internal AParserContext Context = new BasicAParserContext();
	public abstract object Parse(string src);

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void RepeatOpt(BaseParserContext ctx, Action<BaseParserContext> action, Action<bool> successCallback = null!, Action<(string errorLine, int line, int charInLine)> errorCallback = null!) {
		Opt(ctx, c => Repeat(c, action, successCallback!, errorCallback!));
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static void Repeat(BaseParserContext ctx, Action<BaseParserContext> action, Action<bool> successCallback = null!, Action<(string errorLine, int line, int charInLine)> errorCallback = null!) {
		if (!ctx.IsOk) return;

		// save start position for potential revert on first failure
		var startPos = ctx.Context.Pos;
		// first run must succeed and make progress
		action.Invoke(ctx);
		// if the first run failed, restore start position and leave the failure flagged
		if (!ctx.IsOk) {
			ctx.Context.Pos = startPos; // revert any consumed tokens
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
			return;
		}

		// subsequent runs: repeat until an attempt fails or makes no progress
		while (ctx.IsOkAndNotEof) {
			var prevPos = ctx.Context.Pos;
			action.Invoke(ctx);

			// if this attempt failed or made no progress, revert the attempt and clear the failure
			if (!ctx.IsOk) {
				ctx.Context.Pos = prevPos; // revert last failed/no-progress attempt
				ctx.State.FullReset(); // clear the failure from the last attempt
				break;
			}
		}
		successCallback?.Invoke(true);
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	protected static void Opt(BaseParserContext ctx, Action<BaseParserContext> action, Action<bool>? successCallback = null!, Action<(string errorLine, int line, int charInLine)>? errorCallback = null!) {
		if (!ctx.IsOk) return;

		var cpos = ctx.Context.Pos;
		action.Invoke(ctx);
		if (!ctx.IsOk) {
			// revert only if action failed
			ctx.Context.Pos = cpos;
			ctx.State.FullReset(); // Reset only the last failed attempt
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
		}
		successCallback?.Invoke(true);
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	protected static void Alt(BaseParserContext ctx, Action<BaseParserContext>[] actions, Action<bool> successCallback = null!, Action<(string errorLine, int line, int charInLine)> errorCallback = null!) {
		if (!ctx.IsOk) return;

		var cpos = ctx.Context.Pos;
		var lastError = "";
		foreach (var action in actions) {
			action.Invoke(ctx);
			if (ctx.State.Ok) {
				successCallback?.Invoke(true);
				return;
			}

			ctx.State.FullReset();
			ctx.Context.Pos = cpos;
			lastError = $"Alt failed: {ctx.Context.Pos}, {action.Method}";
		}
		ctx.State.Flag($"{lastError}");
		errorCallback?.Invoke(GetErrorSpecifier(ctx));
	}

	/// <summary>
	/// Executes a parsing block and, if it fails, rewinds the parser so the caller can recover by scanning to the next safe sync point.
	/// </summary>
	protected static void Resync(BaseParserContext ctx, Action<BaseParserContext> action, Action<bool>? successCallback = null!, Action<(string errorLine, int line, int charInLine)>? errorCallback = null!, params string[] syncTokens) {
		if (!ctx.IsOk) return;

		var startPos = ctx.Context.Pos;
		try {
			action.Invoke(ctx);
		} catch (ParseException) {
			ctx.Context.Pos = startPos;
			ctx.State.FullReset();
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
			SkipToSyncPoint(ctx, syncTokens);
			successCallback?.Invoke(false);
			return;
		}

		if (ctx.IsOk) {
			successCallback?.Invoke(true);
			return;
		}

		ctx.Context.Pos = startPos;
		ctx.State.FullReset();
		errorCallback?.Invoke(GetErrorSpecifier(ctx));
		SkipToSyncPoint(ctx, syncTokens);
		successCallback?.Invoke(false);
	}

	private static void SkipToSyncPoint(BaseParserContext ctx, params string[] syncTokens) {
		if (!ctx.Context.HasMoreTokens()) return;

		var syncSet = new HashSet<string>(syncTokens.Where(s => !string.IsNullOrWhiteSpace(s)), StringComparer.OrdinalIgnoreCase);
		while (ctx.Context.HasMoreTokens()) {
			var token = ctx.Context.PeekToken();
			var value = token.Value;
			if (syncSet.Contains(token.Token) || syncSet.Contains(value)
				|| value == ";"
				|| value == "}"
				|| value == ")"
				|| value == "]"
				|| value == ","
				|| value == "\n"
				|| value == "\r"
				|| value == Environment.NewLine) {
				break;
			}
			ctx.Context.Consume();
		}
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	protected internal static void Literal(BaseParserContext ctx, string literal, Action<bool> successCallback = null!, Action<(string errorLine, int line, int charInLine)> errorCallback = null!) {
		if (!ctx.IsOk) return;

		if (!ctx.Context.HasMoreTokens()) {
			ctx.State.Flag($"Unexpected end of input in Literal at pos {ctx.Context.Pos}");
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
			return;
		}

		if (ctx.Context.MatchValue(literal)) {
			// consume the matching value and mark success
			ctx.Context.Consume();
			successCallback?.Invoke(true);
		} else {
			ctx.State.Flag($"Literal failed at pos {ctx.Context.Pos}: expected '{literal}'");
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
		}
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	protected static void Token(BaseParserContext ctx, string token, Action<string>? successCallback = null!, Action<(string errorLine, int line, int charInLine)>? errorCallback = null!) {
		if (!ctx.IsOk) {
			return;
		}

		if (!ctx.Context.HasMoreTokens()) {
			ctx.State.Flag($"Unexpected end of input at pos {ctx.Context.Pos}, expected token '{token}'");
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
			return;
		}

		if (ctx.Context.MatchToken(token)) {
			var tk = ctx.Context.Consume();
			successCallback?.Invoke(tk.Value);
		} else {
			ctx.State.Flag($"Unexpected end of input at pos {ctx.Context.Pos}, expected token '{token}'");
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
		}
	}

	//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	protected static void Node<T>(BaseParserContext ctx, Parser<T>? parser, Action<T>? successCallback = null, Action<(string errorLine, int line, int charInLine)>? errorCallback = null)
		where T : new(), allows ref struct {
		if (!ctx.IsOk || parser is null) {
			return;
		}

		var cpos = ctx.Context.Pos;
		var value = parser.Parse(ctx);
		if (!ctx.IsOk) {
			ctx.Context.Pos = cpos;
			ctx.State.Flag($"Node<{typeof(T)}> failed: {ctx.Context}");
			errorCallback?.Invoke(GetErrorSpecifier(ctx));
		} else {
			successCallback?.Invoke(value);
		}
	}

	#region Parser_type

	public class Parser<T>(Action<BaseParserContext, T> action) where T : new(), allows ref struct {
		// add a fild that returns the default get from this class so it returns the T value
		public T Parse(BaseParserContext ctx, Action<(string errorLine, int line, int charInLine)> errorCallback = null!) {
			//Console.WriteLine("Parsing " + typeof(T).Name);
			T self = new();
			action(ctx, self);
			// todo: add error reporting
			if (!ctx.IsOk) {
				errorCallback?.Invoke(GetErrorSpecifier(ctx));
			}
			//parse and get the Ast Type
			return self;
		}
	}

	#endregion

	private static (string errorLine, int line, int charInLine) GetErrorSpecifier(BaseParserContext ctx) {
		var token = ctx.Context.PeekToken();

		var item = ctx.Context.LexerResult.lineReference[token.LineIndex];
		var sourceLine = ctx.Context.LexerResult.source.Substring(item.sourceIndex);
		
		return default;
	}
}
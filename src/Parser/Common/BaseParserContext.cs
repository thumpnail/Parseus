using Parseus.Parser.Implicit;
namespace Parseus.Parser.Common;

/// <summary>
/// Base context for parsers, containing the parser context and cancellation state.
/// </summary>
/// <param name="Context"></param>
/// <param name="State"></param>
public record BaseParserContext(AParserContext Context, CancellationState State) {
	public bool IsOkAndNotEof => IsOk && HasMoreTokens;
	public bool IsOk => State.Ok;
	public bool HasMoreTokens => Context.HasMoreTokens();
}
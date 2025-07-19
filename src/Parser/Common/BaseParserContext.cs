using Parseus.Parser.Implicit;
namespace Parseus.Parser.Common;
/// <summary>
/// Base context for parsers, containing the parser context and cancellation state.
/// </summary>
/// <param name="context"></param>
/// <param name="state"></param>
public record BaseParserContext(AParserContext context, CancellationState state);
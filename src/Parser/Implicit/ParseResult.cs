using Parseus.Parser.Common;
using Parseus.Parser.Diagnostics;
namespace Parseus.Parser.Implicit;

public record ParseResult<T>(
	AParserContext Context, 
	CancellationState State, 
	T Value, 
	bool Success,
	DiagnosticMessage? Message = null);

public static class ParseResultExtension {
	public static ParseResult<T> OnError<T>(this ParseResult<T> self, Action<ParseResult<T>> action) {
		if (!self.Success) {
			action(self);
		}
		return self;
	}
}
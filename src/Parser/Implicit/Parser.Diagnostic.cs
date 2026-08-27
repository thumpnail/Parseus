using Parseus.Lexer;

namespace Parseus.Parser.Implicit;

public partial class BaseParser {
	/// <summary>
	/// Returns the current line number of the token by counting the new lines before the token
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	protected int GetLineNumberFromContinuosString(string input, TokenElement token) {
		
		return 0;
	}
}
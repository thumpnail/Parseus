using System.Text;

using Parseus.Lexer;
using Parseus.Parser.Common;

namespace Parseus.Parser.Implicit;

public partial class BaseParser {
	public record DiagPack(int line, int pos, string sourceLine);
	protected static string CreateReportLine(BaseParserContext c, DiagPack pack, string msg) {
		var sb = new StringBuilder();

		var tmp = $"[{pack.line}:{pack.pos}] | {pack.sourceLine.Replace("\n","")}";
		sb.AppendLine(tmp);
		for (int i = 0; i < tmp.Length; i++) {
			sb.Append("-");
		}

		sb.AppendLine();
		sb.AppendLine($"found: {c.Context.PeekToken().Value}");
		sb.AppendLine($"{msg}");
		
		return sb.ToString();
	}
	/// <summary>
	/// Returns the current line number of the token by counting the new lines before the token
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	protected static DiagPack GetLineNumberFromContinuosString(string input, TokenElement token) {
		var chars = input.ToArray();
		int res_line = 0;
		int res_pos = 0;
		List<char> tmp_resSource = new();

		var cap = input.IndexOf(Environment.NewLine, token.Index, StringComparison.Ordinal);
		
		for (int i = cap; i >= 0; i--) {
			if (res_line == 0) {
				res_pos++;
				tmp_resSource.Add(chars[i]);
			}
			if (chars[i].Equals('\n')) {
				res_line++;
			}
		}

		var resSource = "";
		tmp_resSource.Reverse();
		tmp_resSource.ForEach(x=> resSource += x);
		resSource = resSource.Replace("\t", "");
		return new(res_line+1, res_pos, resSource);
	}
}
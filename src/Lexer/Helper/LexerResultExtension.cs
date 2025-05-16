using Parseus.Parser.Common;
namespace Parseus.Lexer.Helper;

public static class LexerResultExtension {
    public static List<TokenElement> ToTokens<T>(this LexerResult result) {
        return result.result.Select(x => new TokenElement(x.Token,x.Value, x.Index, x.Length)).ToList();
    }
}
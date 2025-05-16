<<<<<<< Updated upstream
﻿using Parseus.Parser.Common;
namespace Parseus.Lexer.Helper;

public static class LexerResultExtension {
    public static List<Token> ToTokens<T>(this LexerResult<T> result) {
        return result.result.Select(x => new Token(x.GetToken.ToString(), x.Value)).ToList();
    }
=======
﻿using Parseus.Parser.Common;
namespace Parseus.Lexer.Helper;

public static class LexerResultExtension {
    public static List<Token> ToTokens<T>(this LexerResult<T> result) {
        return result.result.Select(x => new Token(x.GetToken.ToString(), x.Value)).ToList();
    }
>>>>>>> Stashed changes
}
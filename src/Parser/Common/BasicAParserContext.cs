
using Parseus.Lexer;
using Parseus.Lexer.RegExBased;
using Parseus.Parser.Diagnostics;
namespace Parseus.Parser.Common;
public class BasicAParserContext : AParserContext {
	/// <inheritdoc />
    private List<TokenElement> Tokens { get; set; }
    
    /// <summary>Original source code for diagnostic reporting.</summary>
    
    /// <summary>Precomputed line/column cache for fast lookups.</summary>
    internal LineColumnCache? LineCache { get; private set; }
    
    public BasicAParserContext() {
        this.Tokens = new();
        this.Pos = 0;
    }
    public BasicAParserContext(params TokenElement[] tokens) {
        this.Tokens = tokens.ToList();
        this.Pos = 0;
    }
    public BasicAParserContext(List<TokenElement> tokens) {
        this.Tokens = tokens;
        this.Pos = 0;
    }
    public BasicAParserContext(LexerResult lexerResult) {
        this.Tokens = lexerResult.result;
        this.LexerResult = lexerResult;
        this.Pos = 0;
    }
    
    /// <summary>Sets the source code and precomputes line/column information.</summary>
    public void SetSourceCode(string source) {
        this.SourceCode = source;
        this.LineCache = new LineColumnCache(source);
    }
    
    /// <summary>Gets a TextSpan from the current token position.</summary>
    public TextSpan GetSpanAt(int tokenIndex) {
        if (tokenIndex < 0 || tokenIndex >= Tokens.Count) {
            return TextSpan.At(0);
        }
        var token = Tokens[tokenIndex];
        return new TextSpan(token.Index, token.Length);
    }
    
    /// <summary>Gets a TextSpan for the current token.</summary>
    public TextSpan GetCurrentSpan() => GetSpanAt(Pos);
    
    /// <summary>Gets a TextSpan between two token indices.</summary>
    public TextSpan GetSpanBetween(int startTokenIndex, int endTokenIndex) {
        if (startTokenIndex < 0 || endTokenIndex >= Tokens.Count) {
            return TextSpan.At(0);
        }
        var startToken = Tokens[startTokenIndex];
        var endToken = Tokens[endTokenIndex];
        return new TextSpan(
            startToken.Index, 
            (endToken.Index + endToken.Length) - startToken.Index
        );
    }
	public override TokenElement Consume() {
        if (Pos < Tokens.Count()) {
            return Tokens[Pos++];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicAParserContext)}.Consume");
    }
    public override TokenElement PeekToken(int offset = 0) {
        if (Pos + offset < Tokens.Count()) {
            return Tokens[Pos + offset];
        }
        throw new ParseException("Unexpected end of input",$"{typeof(BasicAParserContext)}.PeekToken");
    }
    
    
    public override bool  MatchToken(string token) => PeekToken().Token.Equals(token);
    public override bool  MatchValue(string value) => PeekToken().Value.Equals(value);

    public bool MatchChar(char value) {
        if (!HasMoreTokens()) {
            return false;
        }

        var current = PeekToken().Value;
        return current.Length > 0 && current[0] == value;
    }

    public bool MatchAny(string allowedChars) => MatchAny(allowedChars.ToCharArray());

    public bool MatchAny(params char[] allowedChars) {
        if (!HasMoreTokens()) {
            return false;
        }

        var current = PeekToken().Value;
        if (current.Length == 0) {
            return false;
        }

        foreach (var allowed in allowedChars) {
            if (current[0] == allowed) {
                return true;
            }
        }

        return false;
    }

    public bool MatchRange(char minInclusive, char maxInclusive) {
        if (!HasMoreTokens()) {
            return false;
        }

        var current = PeekToken().Value;
        return current.Length > 0 && current[0] >= minInclusive && current[0] <= maxInclusive;
    }

    public bool MatchSequence(string value, bool ignoreCase = false) {
        if (string.IsNullOrEmpty(value)) {
            return true;
        }

        if (!HasMoreTokens()) {
            return false;
        }

        var current = PeekToken().Value;
        return current.Length >= value.Length && string.Compare(current, 0, value, 0, value.Length, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0;
    }

    public bool MatchPredicate(Func<char, bool> predicate) {
        if (predicate is null || !HasMoreTokens()) {
            return false;
        }

        var current = PeekToken().Value;
        return current.Length > 0 && predicate(current[0]);
    }

    public bool MatchWhile(Func<char, bool> predicate) {
        if (predicate is null || !HasMoreTokens()) {
            return false;
        }

        var current = PeekToken().Value;
        if (current.Length == 0 || !predicate(current[0])) {
            return false;
        }

        var consumed = Consume();
        return consumed.Value.Length > 0;
    }

    public string ConsumeWhile(Func<char, bool> predicate) {
        if (predicate is null || !HasMoreTokens()) {
            return string.Empty;
        }

        var current = PeekToken().Value;
        if (current.Length == 0 || !predicate(current[0])) {
            return string.Empty;
        }

        return Consume().Value;
    }

    public override bool IsAtEnd() => !HasMoreTokens();
    
    public override bool HasMoreTokens() {
        if (Pos < Tokens.Count()) {
            return true;
        }
        return false;
    }
    public override string ToString() {
        return $"{Pos}";
    }
}
using Parseus.Lexer;
using Parseus.Lexer.RegExBased;

namespace Parseus.Parser.Common;

public class LexOnDemandABasicParserContext {
	public int Pos { get; set; }

	public TokenElement Consume() {
		throw new NotImplementedException();
	}

	public TokenElement PeekToken(int offset = 0) {
		throw new NotImplementedException();
	}

	public bool HasMoreTokens() {
		throw new NotImplementedException();
	}

	public bool MatchToken(string token) {
		throw new NotImplementedException();
	}

	public bool MatchValue(string token) {
		throw new NotImplementedException();
	}

	private string _source;
	private string[] _sourceLines;
	private List<Category> cats;

	public LexOnDemandABasicParserContext(string source, List<Category> cats) {
		_source = source;
		_sourceLines = source.Split(Environment.NewLine);
		this.cats = cats;
	}

	public bool IsAtEnd() => Pos >= _source.Length;

	#region Core Lexing Functions

	// these are low level, zero allocation, high performance, functions that get called by Consume, peektoken, etc to read the source and report back what they found

	public bool MatchChar(char value) {
		throw new NotImplementedException();
	}

	private string MatchAny(string allowedChars) => MatchAny(allowedChars.ToCharArray());

	private string MatchAny(params char[] allowedChars) {
		if (allowedChars.Length == 0) {
			return "";
		}

		int start = Pos;

		//skip whitespace
		while (_source[Pos].Equals(' ') || _source[Pos].Equals('\n')) {
			Pos++;
		}

		var res = "";
		while (!IsAtEnd() && allowedChars.Contains(_source[Pos])) {
			res += _source[Pos];
			Pos++;
		}

		return res;
	}

	private string MatchRange(char minInclusive, char maxInclusive) {
		var res = "";
		while (_source[Pos] >= minInclusive && _source[Pos] <= maxInclusive) {
			res += _source[Pos++];
		}

		return res;
	}

	private bool MatchSequence(string value, bool ignoreCase = false) {
		var current = 0;
		var matches = 0;
		for (int i = Pos; i < Pos + value.Length; i++)
			if (value[i - Pos].Equals(_source[i]))
				matches++;

		if (matches == value.Length) return true;
		return false;
	}

	private bool MatchPredicate(Func<char, bool> predicate) {
		throw new NotImplementedException();
	}

	private bool MatchWhile(Func<char, bool> predicate) {
		throw new NotImplementedException();
	}

	private string ConsumeWhile(Func<char, bool> predicate) {
		throw new NotImplementedException();
	}

	#endregion
}
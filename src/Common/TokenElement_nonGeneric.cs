namespace Parseus.Lexer;

public class TokenElement(
		string token,
		string value,
		int index,
		int length,
		bool isSkippable = false,
		int priority = 0) {
	public int Priority { get; private set; } = priority;
	public string Token { get; private set; } = token;
    public string Value { get; private set; } = value;
    public int Index { get; private set; } = index;
    public int Length { get; private set; } = length;
    public bool IsSkippable { get; private set; } = isSkippable;
    public int LineIndex { get; set; }

    public override string ToString() {
        return $"Token: {Token}, Value: {Value}, Index: {Index}, Length: {Length}, IsSkipable: {IsSkippable}";
    }

    public void SetLineIndex(int lineIndex) {
	    this.LineIndex = lineIndex;
    }
}
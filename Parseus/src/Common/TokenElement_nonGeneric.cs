namespace Parseus.Lexer;

public struct TokenElement {
    public string Token { get; private set; }
    public string Value { get; private set; }
    public int Index { get; private set; }
    public int Length { get; private set; }
    public bool IsSkipable { get; private set; }
    public TokenElement(string token, string value, int index, int length, bool isSkipable = false) {
        this.Token = token;
        this.Value = value;
        this.Index = index;
        this.Length = length;
        this.IsSkipable = isSkipable;
    }
    public override string ToString() {
        return $"Token: {Token}, Value: {Value}, Index: {Index}, Length: {Length}, IsSkipable: {IsSkipable}";
    }
}
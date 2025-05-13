namespace Parseus.Lexer;

public struct TokenElement<T> {
    public T token;
    public object GetToken { get => (object)token; }
    public string Value;
    public int prio;
    public int index;
    public int length;
    public bool isSkipable;
    public TokenElement(T token, string value, int index, int length, bool isSkipable = false, int prio = 0) {
        this.token = token;
        this.Value = value;
        this.index = index;
        this.length = length;
        this.isSkipable = isSkipable;
        this.prio = prio;
    }
}
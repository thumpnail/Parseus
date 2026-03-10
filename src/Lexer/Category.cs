namespace Parseus.Lexer;

struct Category {
	public int Priority;
    public string token;
    public bool isSkipable = false;
    public string[] literals;
    public Category(int priority,string token, params string[] literals) {
        this.token = token;
        this.literals = literals;
		Priority = priority;
    }
    public Category(int priority, string token, bool skipable, params string[] literals) {
        this.token = token;
        this.literals = literals;
        this.isSkipable = skipable;
		Priority = priority;
    }
}
namespace Parseus.Lexer.FunctionBased;

public class Lexer {
	private int priorityCounter = 0;
	private List<Category> cats;
	private string source;
	private string[] sourceLines;
	private List<TokenElement> result;

	public Lexer(string source, List<Category>? cats = null) {
		this.cats = cats ?? new();
		this.source = source;
		this.sourceLines = source.Split(Environment.NewLine);
		this.result = new();
	}
	
	public Lexer Child(string tk, params Func<CategoryParameter, CategoryOutput>[] units) {
		if (units is null)
			throw new Exception();
		this.cats.Add(new Category(priorityCounter++, tk, false, units));
		return this;
	}

	public Lexer Skippable(string tk, params Func<CategoryParameter, CategoryOutput>[] units) {
		if (units is null)
			throw new Exception();
		this.cats.Add(new Category(priorityCounter++, tk, true, units));
		return this;
	}

	public LexerResult Lex(string source) {
		var result = new List<LexerResult>();
		return default;
	}
}
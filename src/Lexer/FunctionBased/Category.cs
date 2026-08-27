namespace Parseus.Lexer.FunctionBased;

public ref struct CategoryParameter(ref string source, ref int pos, params string[] AdditionalMetadata);

public ref struct CategoryOutput(bool success, int pos, int length, string content);

public struct Category {
	public int Priority;
	public string token;
	public bool IsSkippable = false;
	public Func<CategoryParameter, CategoryOutput>[] LexUnits;

	public Category(int priority, string token, params Func<CategoryParameter, CategoryOutput>[] units) {
		this.token = token;
		this.LexUnits = units;
		Priority = priority;
	}

	public Category(int priority, string token, bool skippable, params Func<CategoryParameter, CategoryOutput>[] units) {
		this.token = token;
		this.LexUnits = units;
		this.IsSkippable = skippable;
		Priority = priority;
	}
}
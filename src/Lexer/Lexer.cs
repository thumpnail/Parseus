using System.Text.RegularExpressions;
namespace Parseus.Lexer;

public class Lexer {
	private int priorityCounter = 0;
    private List<Category> cats;
    private string source;
    private List<TokenElement> result;
    public Lexer() {
        this.source = String.Empty;
        this.cats = new();
        this.result = new();
    }
    //creates a child category
    public Lexer Child(string tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        this.cats.Add(new Category(priorityCounter++, tk, lit));
        return this;
    }
    public Lexer Skippable(string tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        cats.Add(new Category(priorityCounter++, tk, true, lit));
        return this;
    }
    public LexerResult Lex(string source) {
        this.source = source;
        foreach (var cat in cats) {
            foreach (var str in cat.literals) {
                var rgx = new Regex(str);
                var res = rgx.Matches(this.source);
                for (int i = 0; i < res.Count; i++) {
                    var match = res[i];
                    if(cat.isSkipable)
                        result.Add(new(cat.token, match.Value, match.Index, match.Length, true, cat.Priority));
                    else
                        result.Add(new(cat.token, match.Value, match.Index, match.Length, priority: cat.Priority));
                }
            }
        }
        // Sort by start index to make processing deterministic
        result.Sort((a, b) => a.Index.CompareTo(b.Index));

        // For each start index, pick the longest match; if lengths equal, prefer lower Priority
        // because lower Priority means earlier registration (priorityCounter increments on add).
        result = result
            .GroupBy(o => o.Index)
            .Select(g => g.OrderByDescending(o => o.Length).ThenBy(o => o.Priority).First())
            .ToList();
        var rmlist = new List<TokenElement>();
        foreach (var item1 in result) {
            int eidx = item1.Index + item1.Length;
            foreach (var item2 in result.Where(x => (x.Index < eidx && x.Index > item1.Index) || x.IsSkipable)) {
                rmlist.Add(item2);
            }
        }
        foreach (var item in rmlist) {
            result.Remove(item);
        }
        return new LexerResult(source, result);
    }
}
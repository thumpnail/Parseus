using System.Text.RegularExpressions;

public struct TokenElement<T> where T : Enum {
    public T token;
    public string value;
    public int index;
    public int length;
    public bool isSkipable;
    public TokenElement(T token, string value, int index, int length, bool isSkipable = false) {
        this.token = token;
        this.value = value;
        this.index = index;
        this.length = length;
        this.isSkipable = isSkipable;
    }
}

//Contains token and string/regex for that token
struct Category<T> where T : Enum {
    public T token;
    public bool isSkipable = false;
    public string[] literals;
    public Category(T token, params string[] literals) {
        this.token = token;
        this.literals = literals;
    }
    public Category(T token, bool skipable, params string[] literals) {
        this.token = token;
        this.literals = literals;
        this.isSkipable = skipable;
    }
}

public struct LexerResult<T> where T : Enum {
    public List<TokenElement<T>> result;
    public string source;
    public LexerResult(string source, List<TokenElement<T>> result) {
        this.result = result;
        this.source = source;
    }
}
public class Lexer<T> where T : Enum {
    private List<Category<T>> cats;
    private string source;
    private List<TokenElement<T>> result;
    public Lexer() {
        this.source = String.Empty;
        this.cats = new();
        this.result = new();
    }
    //creates a child category
    public Lexer<T> child(T tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        this.cats.Add(new Category<T>(tk, lit));
        return this;
    }
    public Lexer<T> skipable(T tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        cats.Add(new Category<T>(tk, true, lit));
        return this;
    }
    public LexerResult<T> Lex(string source) {
        this.source = source;
        foreach (var cat in cats) {
            foreach (var str in cat.literals) {
                var rgx = new Regex(str);
                var res = rgx.Matches(this.source);
                for (int i = 0; i < res.Count; i++) {
                    var match = res[i];
                    if(cat.isSkipable)
                        result.Add(new(cat.token, match.Value, match.Index, match.Length, true));
                    else
                        result.Add(new(cat.token, match.Value, match.Index, match.Length));
                }
            }
        }
        result.Sort((element, tokenElement) => {
            if (element.index > tokenElement.index)
                return 1;
            else if (element.index < tokenElement.index)
                return -1;
            else
                return 0;
        });
        result = result
            .GroupBy(o => o.index)
            .Select(g => g.OrderByDescending(o => o.length).First()) // get the one whith highest length
            .ToList()
            ;
        var rmlist = new List<TokenElement<T>>();
        foreach (var item1 in result) {
            int eidx = item1.index + item1.length;
            foreach (var item2 in result.Where(x => (x.index < eidx && x.index > item1.index) || x.isSkipable)) {
                rmlist.Add(item2);
            }
        }
        foreach (var item in rmlist) {
            result.Remove(item);
        }
        return new LexerResult<T>(source,result);
    }
}
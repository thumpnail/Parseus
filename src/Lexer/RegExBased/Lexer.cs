using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;

using Parseus.Lexer.Helper;

namespace Parseus.Lexer.RegExBased;

public class Lexer {
	private int priorityCounter = 0;
    private List<Category> cats;
    private string source;
    private List<TokenElement> result;
    public IReadOnlyList<Category> Categories => cats;
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
        result.Clear();
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
		
		
        //result.Sort((element, tokenElement) => {
        //    if (element.Index > tokenElement.Index)
        //        return 1;
        //    else if (element.Index < tokenElement.Index)
        //        return -1;
        //    else
        //        return 0;
        //});
		//
		//result = result.GroupBy(o => o.Index)
		//	.Select(g => g.OrderByDescending(o => o.Priority).Last()) // get the one whith Lowest Priority
		//	.ToList()
		//	;
		
		result = result
			.GroupBy(o => o.Index)
			.Select(g => g.OrderByDescending(o => o.Length).ThenBy(o => o.Priority).First())
			.ToList();
		//result = result
        //    .GroupBy(o => o.Index)
        //    .Select(g => g.OrderByDescending(o => o.Length).First()) // get the one whith highest length
		//	.ToList()
        //    ;
        var rmlist = new List<TokenElement>();
        foreach (var item1 in result) {
            int eidx = item1.Index + item1.Length;
            foreach (var item2 in result.Where(x => (x.Index < eidx && x.Index > item1.Index) || x.IsSkippable)) {
                rmlist.Add(item2);
            }
        }
        foreach (var item in rmlist) {
            result.Remove(item);
        }

        // todo: get all line numbers and their index
        var lines = GetLineNumbers(source, result);
        return new LexerResult(source, result, lines);
    }
	
    private List<(int lineNumber, int sourceIndex)> GetLineNumbers(string source, List<TokenElement> tokenElements) {
        var lineReference = new List<(int lineNumber, int sourceIndex)> {
            (0, 0)
        };

        for (int i = 0; i < source.Length; i++) {
            if (source[i] == '\r') {
                if (i + 1 < source.Length && source[i + 1] == '\n') {
                    i++;
                }
                lineReference.Add((lineReference.Count, i + 1));
            } else if (source[i] == '\n') {
                lineReference.Add((lineReference.Count, i + 1));
            }
        }

        var currentLine = 0;
        foreach (var token in tokenElements.OrderBy(t => t.Index)) {
            while (currentLine + 1 < lineReference.Count && token.Index >= lineReference[currentLine + 1].sourceIndex) {
                currentLine++;
            }
            token.LineIndex = currentLine;
        }

        return lineReference;
    }
}
<<<<<<< Updated upstream
﻿using System.Text.RegularExpressions;
namespace Parseus.Lexer;

//Contains token and string/regex for that token

public class Lexer {
    private List<Category> cats;
    private string source;
    private List<TokenElement> result;
    public Lexer() {
        this.source = String.Empty;
        this.cats = new();
        this.result = new();
    }
    //creates a child category
    public Lexer child(string tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        this.cats.Add(new Category(tk, lit));
        return this;
    }
    public Lexer skipable(string tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        cats.Add(new Category(tk, true, lit));
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
        var rmlist = new List<TokenElement>();
        foreach (var item1 in result) {
            int eidx = item1.index + item1.length;
            foreach (var item2 in result.Where(x => (x.index < eidx && x.index > item1.index) || x.isSkipable)) {
                rmlist.Add(item2);
            }
        }
        foreach (var item in rmlist) {
            result.Remove(item);
        }
        return new LexerResult(source, result);
    }
=======
﻿using System.Text.RegularExpressions;
namespace Parseus.Lexer;

//Contains token and string/regex for that token

public class Lexer {
    private List<Category> cats;
    private string source;
    private List<TokenElement> result;
    public Lexer() {
        this.source = String.Empty;
        this.cats = new();
        this.result = new();
    }
    //creates a child category
    public Lexer child(string tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        this.cats.Add(new Category(tk, lit));
        return this;
    }
    public Lexer skipable(string tk, params string[] lit) {
        if (lit is null)
            throw new Exception();
        cats.Add(new Category(tk, true, lit));
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
        var rmlist = new List<TokenElement>();
        foreach (var item1 in result) {
            int eidx = item1.index + item1.length;
            foreach (var item2 in result.Where(x => (x.index < eidx && x.index > item1.index) || x.isSkipable)) {
                rmlist.Add(item2);
            }
        }
        foreach (var item in rmlist) {
            result.Remove(item);
        }
        return new LexerResult(source, result);
    }
>>>>>>> Stashed changes
}
using Parseus.Lexer;
using Xunit;

namespace Parseus.Tests;

public class Lexer_Tests {

    [Fact]
    public void RecognizesNumberPlusNumberAndSkipsWhitespace() {
        var lexer = new Parseus.Lexer.Lexer()
            .Skippable("WS", "\\s+")
            .Child("NUMBER", "\\d+")
            .Child("PLUS", "\\+");

        var res = lexer.Lex("12 + 34");

        Assert.Equal(3, res.result.Count);
        Assert.Equal<string>("NUMBER", res.result[0].Token);
        Assert.Equal<string>("12", res.result[0].Value);
        Assert.Equal<string>("PLUS", res.result[1].Token);
        Assert.Equal<string>("+", res.result[1].Value);
        Assert.Equal<string>("NUMBER", res.result[2].Token);
        Assert.Equal<string>("34", res.result[2].Value);
    }

    [Fact]
    public void PrefersLongerTokenOverShorterWhenOverlapping() {
        var lexer = new Parseus.Lexer.Lexer()
            .Child("EQL", "==")
            .Child("ASSIGN", "=");

        var res = lexer.Lex("==");

        Assert.Equal(1, res.result.Count);
        Assert.Equal<string>("EQL", res.result[0].Token);
        Assert.Equal<string>("==", res.result[0].Value);
    }

    [Fact]
    public void ReturnsEmptyResultForEmptySource() {
        var lexer = new Parseus.Lexer.Lexer().Child("A", "a");
        var res = lexer.Lex("");
        Assert.Equal(0, res.result.Count);
    }

    [Fact]
    public void ProducesTokensInSourceOrder() {
        var lexer = new Parseus.Lexer.Lexer()
            .Child("A", "a")
            .Child("B", "b");

        var res = lexer.Lex("ba");

        Assert.Equal(2, res.result.Count);
        Assert.Equal<string>("B", res.result[0].Token);
        Assert.Equal<string>("b", res.result[0].Value);
        Assert.Equal<string>("A", res.result[1].Token);
        Assert.Equal<string>("a", res.result[1].Value);
    }

    [Fact]
    public void ChoosesEarlierAddedTokenWhenPatternsIdentical() {
        var lexer = new Parseus.Lexer.Lexer()
            .Child("FIRST", "a")
            .Child("SECOND", "a");

        var res = lexer.Lex("a");

        Assert.Equal(1, res.result.Count);
        Assert.Equal<string>("FIRST", res.result[0].Token);
    }

    [Fact]
    public void EarlierAddedShorterTokenBeatsLaterLongerTokenDueToPriority() {
        var lexer = new Parseus.Lexer.Lexer()
            .Child("SHORT", "a")
            .Child("LONG", "ab");

        var res = lexer.Lex("ab");

        Assert.Equal(1, res.result.Count);
        Assert.Equal<string>("SHORT", res.result[0].Token);
        Assert.Equal<string>("a", res.result[0].Value);
    }
	
	[Fact]
    public void Lex_SimpleExpression_ProducesThreeTokens() {
        var lexer = new Lexer.Lexer()
            .Child("number","\\d+")
            .Child("plus","\\+")
            .Child("identifier","[a-zA-Z_][a-zA-Z0-9_]*")
            .Skippable("ws","\\s+");
        var src = "x + 42";
        var res = lexer.Lex(src);
        Assert.Equal(3, res.result.Count);
        Assert.Equal("identifier", res.result[0].Token);
        Assert.Equal("x", res.result[0].Value);
        Assert.Equal(0, res.result[0].Index);
        Assert.Equal("plus", res.result[1].Token);
        Assert.Equal("+", res.result[1].Value);
        Assert.Equal(2, res.result[1].Index);
        Assert.Equal("number", res.result[2].Token);
        Assert.Equal("42", res.result[2].Value);
        Assert.Equal(4, res.result[2].Index);
    }

    [Fact]
    public void SkippableTokens_AreNotIncludedInResult() {
        var lexer = new Lexer.Lexer()
            .Child("word","[a-zA-Z]+")
            .Skippable("space","\\s+");
        var src = "hello world";
        var res = lexer.Lex(src);
        Assert.Equal(2, res.result.Count);
        Assert.Equal("word", res.result[0].Token);
        Assert.Equal("hello", res.result[0].Value);
        Assert.Equal("word", res.result[1].Token);
        Assert.Equal("world", res.result[1].Value);
    }

    [Fact]
    public void LongerToken_PreferredOverShorterWhenBothMatchAtSamePosition() {
        var lexer = new Lexer.Lexer()
            .Child("dot","\\.")
            .Child("doubledot","\\.\\.");
        var src = "..";
        var res = lexer.Lex(src);
        Assert.Single(res.result);
        Assert.Equal("doubledot", res.result[0].Token);
        Assert.Equal("..", res.result[0].Value);
        Assert.Equal(0, res.result[0].Index);
        Assert.Equal(2, res.result[0].Length);
    }

    [Fact]
    public void OverlappingTokens_InnerTokensAreRemovedKeepingLongest() {
        var lexer = new Lexer.Lexer()
            .Child("a","a")
            .Child("ab","ab")
            .Child("b","b");
        var src = "ab";
        var res = lexer.Lex(src);
        Assert.Single(res.result);
        Assert.Equal("ab", res.result[0].Token);
        Assert.Equal("ab", res.result[0].Value);
        Assert.Equal(0, res.result[0].Index);
        Assert.Equal(2, res.result[0].Length);
    }

    [Fact]
    public void EmptySource_ReturnsNoTokens() {
        var lexer = new Lexer.Lexer()
            .Child("num","\\d+");
        var res = lexer.Lex(string.Empty);
        Assert.Empty(res.result);
    }
}
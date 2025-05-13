using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Parseus.Parser.Explicit;
namespace Parseus.Parser.BasicParser.SBNF;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SbnfParser : Parsable {
    private readonly string[] _tokens;
    private int _position;

    public SbnfParser(string input) {
        _tokens = Tokenize(input);
        _position = 0;
    }

    public override AstNode Parse() {
        var rules = new List<AstNode>();
        while (!IsAtEnd()) {
            rules.Add(ParseRule());
        }
        return new AstNode("Root", rules);
    }

    private AstNode ParseRule() {
        string ruleName = ConsumeIdentifier();
        Consume("=");
        var expression = ParseRuleBody();
        Consume(";");
        return new AstNode("Rule", expression, ruleName);
    }

    private List<AstNode> ParseRuleBody() {
        var terms = new List<AstNode>();
        while (!Check(";") && !IsAtEnd()) {
            terms.Add(ParseTerm());
        }
        return terms;
    }

    private AstNode ParseTerm() {
        var tmp = _tokens[_position];
        if (Check("{"))
            return new AstNode("Repeat", ParseRepeat());
        if (Check("["))
            return new AstNode("Optional", ParseOptional());
        if (Check("("))
            return new AstNode("Alternative", ParseAlternatives());
        if (CheckRegex("\".*?\""))
            return new AstNode("Literal", Consume());
        if (CheckRegex("/.*?/"))
            return new AstNode("Regex", Consume());
        if (CheckRegex("<.*?>"))
            return new AstNode("Token", Consume());
        return new AstNode("Identifier", ConsumeIdentifier());
    }

    private List<AstNode> ParseRepeat() {
        var result = new List<AstNode>();
        Consume("{");
        while (!Check("}")) {
            result.Add(ParseTerm());
        }
        Consume("}");
        return result;
    }
    private List<AstNode> ParseOptional() {
        var result = new List<AstNode>();
        Consume("[");
        while (!Check("]")) {
            result.Add(ParseTerm());
        }
        Consume("]");
        return result;
    }
    private List<AstNode> ParseAlternatives() {
        var result = new List<AstNode>();
        Consume("(");
        result.AddRange(ParseTerms());
        while (Match("|").success) {
            result.AddRange(ParseTerms());
        }
        result.AddRange(ParseTerms());
        Consume(")");
        return result;
    }
    private List<AstNode> ParseTerms() {
        var terms = new List<AstNode>();
        while (!Check(";") && !IsAtEnd() && !Check("|") && !Check(")")) {
            terms.Add(ParseTerm());
        }
        return terms;
    }
    #region parser

    private string ConsumeIdentifier() {
        if (CheckRegex("[a-zA-Z_][a-zA-Z0-9_]*")) return Consume();
        throw new Exception($"Expected identifier. ({_tokens[_position]},{_position})");
    }

    private string Consume(string? expected = null) {
        if (expected != null && !Check(expected))
            throw new Exception($"Expected '{expected}' but found '{_tokens[_position]}'");
        return _tokens[_position++];
    }

    private bool Check(string expected) => !IsAtEnd() && _tokens[_position] == expected;
    private (string value, bool success) Match(string expected) => Check(expected) ? (Consume(), true) : ("", false);
    private bool CheckRegex(string pattern) => !IsAtEnd() && Regex.IsMatch(_tokens[_position], "^" + pattern + "$", RegexOptions.Compiled);
    private bool IsAtEnd() => _position >= _tokens.Length;

    private string[] Tokenize(string input) {
        var pattern = "([{}\\[\\]()|=;])|\".*?\"|\'.*?\'|/.*?/|<[a-zA-Z_][a-zA-Z0-9_]*>|[a-zA-Z_][a-zA-Z0-9_]*";
        var matches = Regex.Matches(input, pattern);
        var tokenList = new List<string>();
        foreach (Match match in matches) tokenList.Add(match.Value);

        Console.WriteLine(JsonConvert.SerializeObject(tokenList, Formatting.Indented));

        return tokenList.ToArray();
    }

    #endregion
}
public class AstNode : Parsable
{
    public string Type { get; }
    public string Value { get; }
    public List<AstNode> Children { get; }

    public AstNode(string type, string value = null)
    {
        Type = type;
        Value = value;
        Children = new List<AstNode>();
    }

    public AstNode(string type, List<AstNode> children, string value = null)
    {
        Type = type;
        Children = children;
        Value = value;
    }

    public override Parsable? Parse() {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Value != null ? $"{Type}({Value})" : $"{Type}[{string.Join(", ", Children)}]";
    }
}

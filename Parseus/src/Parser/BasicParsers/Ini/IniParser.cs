using Parseus.Lexer;
using Parseus.Lexer.Helper;
using Parseus.Parser.Common;
using Parseus.Parser.Explicit;
using Parseus.Parser.Implicit;
using Parseus.Util;
namespace Parseus.Parser.BasicParsers.Ini;

class TK {
    public const string identifier = "identifier";
    public const string openBracket = "openBracket";
    public const string closeingBracket = "closeingBracket";
    public const string openBraces = "openBraces";
    public const string closeingBraces = "closeingBraces";
    public const string equal = "equal";
    public const string eol = "eol";
    public const string @string = "@string";
    public const string number = "number";
    public const string comma = "comma";
}

public class IniRoot {
    public List<IniGroup> groups = new List<IniGroup>();
    public override string ToString() {
        return $"groups={string.Join(", ", groups.Select(x => x.ToString()))}";
    }
}

public class IniGroup {
    public string name;
    public List<IniField> fields = new();
    public override string ToString() {
        return $"name={name}";
    }
}

public class IniField {
    public string name;
    public IniValue value;
    public override string ToString() {
        return $"{name}={value.ToString()}";
    }
}

public class IniValue {
    public string sValue;
    public string nValue;
    public Array aValue;
    public override string ToString() {
        if (!string.IsNullOrEmpty(sValue)) {
            return sValue.ToString();
        }
        if (!string.IsNullOrEmpty(nValue)) {
            return nValue.ToString();
        }
        if (aValue is not null) {
            return aValue.ToString();
        }
        return $"[-_-]";
    }
}

public class Array {
    public List<IniValue> IniValues = new();
    public override string ToString() {
        return $"[{string.Join(", ", IniValues.Select(x => x.ToString()))}]";
    }
}

public class Identifier {
    public string value;
    public override string ToString() {
        return $"{value}";
    }
}

public class IniParser : Implicit.BaseParser {
    const string ANY = "[.]";
    const string STRING = $"\"{ANY}\"";
    const string WORD = "[a-zA-Z_][a-zA-Z0-9_]*";
    const string IDENTIFIER = $"[\\.]?{WORD}([\\.]{WORD})*([\\:]{WORD})?";
    const string DIGIT = "[0-9]";
    const string NUMBER = $"{DIGIT}+(\\.{DIGIT}+)?";
    private Lexer.Lexer<string> lexer = new Lexer<string>()
        .Child(TK.openBracket, @"\[")
        .Child(TK.closeingBracket, @"\]")
        .Child(TK.openBraces, @"\{")
        .Child(TK.closeingBraces, @"\}")
        .Child(TK.equal, "=")
        .Child(TK.comma, ",")
        .Child(TK.eol, Environment.NewLine)
        .Child(TK.identifier, IDENTIFIER)
        .Child(TK.@string, "\"" + @"(\\.|[^" + "\"" + @"\\])*" + "\"")
        .Child(TK.@string, @"'(\\.|[^'\\])*'")
        .Child(TK.number, @"-?(0[xX][0-9a-fA-F]+|\d*[.]\d+([eE][+-]?\d+)?|\d+([.]\d*)?([eE][+-]?\d+)?)");
    public override IniRoot Parse(string src) {
        var lexres = lexer.Lex(src);
        Console.WriteLine(string.Join("\n", lexres.result.Select(x => $"({x.token}:{(x.Value.Contains("\n") ? "<newline>" : x.Value)})").ToList()));
        //var doc = IniRootParser.Parse(new BasicParserContext(lexres.ToTokens().ToArray()));
        return /*doc*/ new();
    }
/*
    private static readonly Parser<IniRoot> IniRootParser = new((c, self) => {
        Repeat(c, () => {
            var res = Node(c, IniGroupParser, (iniGroup) => {
                self.groups.Add(iniGroup);
            });
            return res;
        }, out _);
    });

    private static readonly Parser<IniGroup> IniGroupParser = new((c, self) => {
        Literal(c, "[", out _);
        Token(c, TK.identifier, out var result);
        self.name = result.Value;
        Literal(c, "]", out _);
        Token(c, TK.eol, out _);
        Repeat(c, () => {
            Node(c, IniFieldParser, out var v);
            self.fields.Add(v.Value);
            return ResultExt.MCheck(v.Success);
        }, out _);
    });

    private static readonly Parser<IniField> IniFieldParser = new((c, self) => {
        Token(c, TK.identifier, out var res);
        self.name = res.Value;
        Literal(c, "=", out _);
        Node(c, IniValueParser, (value) => {
            self.value = value;
        });
        Token(c, TK.eol, out _);
    });
    private static readonly Parser<IniValue> IniValueParser = new((c, self) => {
        Alt(c, out _, () => {
            Token(c, TK.identifier, out var sValue);
            self.sValue = sValue.Value;
            return sValue.Success;
        }, () => {
            Token(c, TK.@string, out var sValue);
            self.sValue = sValue.Value;
            return sValue.Success;
        }, () => {
            Token(c, TK.number, out var nValue);
            self.nValue = nValue.Value;
            return nValue.Success;
        }, () => {
            Node(c, ArrayParser, out var aValue);
            self.aValue = aValue.Value;
            return aValue.Success;
        });
    });
    private static readonly Parser<Array> ArrayParser = new((c, self) => {
        Literal(c, "{", out _);
        Node(c, IniValueParser, (value) => {
            self.IniValues.Add(value);
        });
        Opt(c, () => {
            Repeat(c, () => {
                Literal(c, ",", out _);
                Node(c, IniValueParser, out var value);
                    self.IniValues.Add(value.Value);
                return value.Success;
            }, out var result);
            return result.Success;
        }, out _);
        Literal(c, "}", out _);
    });
    private static readonly Parser<Identifier> IdentifierParser = new((c, self) => {
        Token(c, TK.identifier, out var res);
        self.value = res.Value;
    });

    public static void Main() {
        var src = """
                  [group]
                  abc=12
                  cvf="hello world"
                  isebf={12,12}
                  """;
        var ini = new IniParser();
        var result = ini.Parse(src);
        Console.WriteLine(new YamlDotNet.Serialization.Serializer().Serialize(result));
    }*/
}
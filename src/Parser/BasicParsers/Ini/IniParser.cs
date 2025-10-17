using Parseus.Lexer;
using Parseus.Lexer.Helper;
using Parseus.Parser.Common;
using Parseus.Parser.Explicit;
using Parseus.Parser.Implicit;
using Parseus.Util;
namespace Parseus.Parser.BasicParsers.Ini;

class TK {
    public const string identifier = "identifier";
    public const string openbracket = "openBracket";
    public const string closeingbracket = "closeingBracket";
    public const string openbraces = "openBraces";
    public const string closeingbraces = "closeingBraces";
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
    public Identifier name;
    public List<IniField> fields = new();
    public override string ToString() {
        return $"name={name}";
    }
}

public class IniField {
    public Identifier name;
    public IniValue value;
    public override string ToString() {
        return $"{name}={value.ToString()}";
    }
}

public class IniValue {
    public string ValueType;
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
    private Lexer.Lexer lexer = new Lexer.Lexer()
        .Child(TK.openbracket, @"\[")
        .Child(TK.closeingbracket, @"\]")
        .Child(TK.openbraces, @"\{")
        .Child(TK.closeingbraces, @"\}")
        .Child(TK.equal, "=")
        .Child(TK.comma, ",")
        .Child(TK.eol, Environment.NewLine)
        .Child(TK.identifier, IDENTIFIER)
        .Child(TK.@string, "\"" + @"(\\.|[^" + "\"" + @"\\])*" + "\"",@"'(\\.|[^'\\])*'")
        .Child(TK.number, @"-?(0[xX][0-9a-fA-F]+|\d*[.]\d+([eE][+-]?\d+)?|\d+([.]\d*)?([eE][+-]?\d+)?)");
    public override IniRoot Parse(string src) {
        var lexres = lexer.Lex(src);
        Console.WriteLine(string.Join("\n", lexres.result.Select(x => $"({x.Token}:{(x.Value.Contains("\n") ? "<newline>" : x.Value)})").ToList()));
        var doc = IniRootParser.Parse(new(new BasicAParserContext(lexres.result.ToArray()), new CancellationState()));
        return doc;
    }
    private static readonly Parser<IniRoot> IniRootParser = new((c, self) => {
        Repeat(c, c => {
            Node(c, IniGroupParser, (iniGroup) => { self.groups.Add(iniGroup); });
        });
    });
    // IniGroupParser := "[" identifier "]" eol { IniFieldParser }
    private static readonly Parser<IniGroup> IniGroupParser = new((c, self) => {
        Literal(c, "[", out _);
        Node(c, IdentifierParser, out self.name);
        Literal(c, "]", out _);
        Token(c, TK.eol, out _);
        Repeat(c, c => {
            Node(c, IniFieldParser, v => self.fields.Add(v));
        });
    });
    // IniFieldParser := identifier "=" IniValue eol
    private static readonly Parser<IniField> IniFieldParser = new((c, self) => {
        Node(c, IdentifierParser, out self.name);
        Literal(c, "=", out _);
        Node(c, IniValueParser, out self.value);
        Opt(c, c => Token(c, TK.eol, out _));
    });
    // IniValueParser := identifier | string | number | Array
    private static readonly Parser<IniValue> IniValueParser = new((c, self) => {
        Alt(c, c => {
            Token(c, TK.identifier, out self.sValue);
            self.ValueType = TK.identifier;
        }, c => {
            Token(c, TK.@string, out self.sValue);
            self.ValueType = TK.@string;
        }, c => {
            Token(c, TK.number, out self.nValue);
            self.ValueType = TK.number;
        }, c => {
            Node(c, ArrayParser, out self.aValue);
            self.ValueType = "Array";
        });
    });
    // ArrayParser := "{" IniValue { "," IniValue } "}"
    private static readonly Parser<Array> ArrayParser = new((c, self) => {
        Literal(c, "{", out _);
        Node(c, IniValueParser, v => self.IniValues.Add(v));
        Repeat(c, c => {
            Literal(c, ",", out _);
            Node(c, IniValueParser, v => self.IniValues.Add(v));
        });
        Literal(c, "}", out _);
    });
    // IdentifierParser := tk_identifier
    private static readonly Parser<Identifier> IdentifierParser = new((c, self) => {
        Token(c, TK.identifier, out self.value);
    });
    
    public static void TestIniParser() {
        var src = """
                  [group]
                  abc=12
                  cde="hello world"
                  efg={12,"Hello",12,"world"}
                  """;
        var ini = new IniParser();
        var result = ini.Parse(src);
        Console.WriteLine(new YamlDotNet.Serialization.Serializer().Serialize(result));
    }
}
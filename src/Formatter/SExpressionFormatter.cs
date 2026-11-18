using System.Text;

namespace Parseus.Formatter;

public sealed class SExpressionFormatter
{
    private readonly SExpressionFormatterOptions _options;

    public SExpressionFormatter() : this(new SExpressionFormatterOptions())
    {
    }

    public SExpressionFormatter(SExpressionFormatterOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public SExpressionFormatter Inline(params string[] heads)
    {
        foreach (var head in heads)
        {
            if (!string.IsNullOrEmpty(head))
                _options.InlinePrefixes.Add(head);
        }
        return this;
    }

    public string Format(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _options.Validate();

        var parser = new SExpressionParser(source);
        var roots = parser.ParseRoots();
        var output = new List<string>();

        foreach (var root in roots)
            output.AddRange(Render(root, 0).Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None));

        return string.Join(_options.NewLine, output);
    }

    private string Render(SExpressionNode node, int level)
    {
        var indent = string.Concat(Enumerable.Repeat(_options.Indentation, level));

        if (node.Items.Count == 0)
            return indent + "()";

        if (node.Items.All(item => item is SExpressionAtom))
            return indent + "(" + string.Join(" ", node.Items.Cast<SExpressionAtom>().Select(x => x.Value)) + ")";

        var output = new StringBuilder();
        output.Append(indent).Append('(');

        var index = 0;
        var hasBlockChild = false;

        foreach (var item in node.Items)
        {
            if (item is SExpressionAtom atom)
            {
                if (output.Length > indent.Length + 1)
                    output.Append(' ');
                output.Append(atom.Value);
            }
            else
            {
                var child = (SExpressionNode)item;
                if (ShouldRenderInline(child))
                {
                    if (output.Length > indent.Length + 1)
                        output.Append(' ');
                    output.Append(RenderInline(child));
                }
                else
                {
                    output.Append(_options.NewLine);
                    output.Append(Render(child, level + 1));
                    hasBlockChild = true;
                }
            }
            index++;
        }

        if (hasBlockChild)
            output.Append(_options.NewLine).Append(indent).Append(')');
        else
            output.Append(')');

        return output.ToString();
    }

    private bool ShouldRenderInline(SExpressionNode node)
    {
        var head = node.Items.OfType<SExpressionAtom>().FirstOrDefault();
        return head != null && _options.InlinePrefixes.Contains(head.Value);
    }

    private string RenderInline(SExpressionNode node)
    {
        var parts = new List<string>();
        foreach (var item in node.Items)
        {
            if (item is SExpressionAtom atom)
                parts.Add(atom.Value);
            else
            {
                var child = (SExpressionNode)item;
                parts.Add(RenderInline(child));
            }
        }
        return "(" + string.Join(" ", parts) + ")";
    }
}

public sealed class SExpressionFormatterOptions
{
    public string Indentation { get; set; } = "\t";
    public string NewLine { get; set; } = Environment.NewLine;
    public ISet<string> InlinePrefixes { get; } = new HashSet<string>(StringComparer.Ordinal) { "param" };

    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(Indentation);
        ArgumentNullException.ThrowIfNull(NewLine);
    }
}

internal abstract record SExpressionItem;
internal sealed record SExpressionAtom(string Value) : SExpressionItem;
internal sealed record SExpressionNode(List<SExpressionItem> Items) : SExpressionItem;

internal sealed class SExpressionParser
{
    private readonly string _source;
    private int _position;

    public SExpressionParser(string source)
    {
        _source = source;
    }

    public List<SExpressionNode> ParseRoots()
    {
        var roots = new List<SExpressionNode>();
        SkipWhitespace();
        while (_position < _source.Length)
        {
            if (_source[_position] != '(')
                throw new FormatException($"Expected '(' at position {_position}.");
            roots.Add(ParseNode());
            SkipWhitespace();
        }
        return roots;
    }

    private SExpressionNode ParseNode()
    {
        _position++;
        var items = new List<SExpressionItem>();

        while (_position < _source.Length)
        {
            SkipWhitespace();
            if (_position >= _source.Length)
                throw new FormatException($"Unclosed '(' at position {_position}.");
            if (_source[_position] == ')')
            {
                _position++;
                return new SExpressionNode(items);
            }
            if (_source[_position] == '(')
                items.Add(ParseNode());
            else
                items.Add(new SExpressionAtom(ReadAtom()));
        }

        throw new FormatException("Unclosed parenthesis.");
    }

    private string ReadAtom()
    {
        var start = _position;
        var quote = '\0';

        if (_source[_position] is '"' or '\'')
            quote = _source[_position++];

        while (_position < _source.Length)
        {
            var character = _source[_position];
            if (quote != 0)
            {
                if (character == '\\' && _position + 1 < _source.Length)
                {
                    _position += 2;
                    continue;
                }
                _position++;
                if (character == quote)
                    break;
            }
            else
            {
                if (char.IsWhiteSpace(character) || character is '(' or ')')
                    break;
                _position++;
            }
        }

        return _source[start.._position];
    }

    private void SkipWhitespace()
    {
        while (_position < _source.Length && char.IsWhiteSpace(_source[_position]))
            _position++;
    }
}

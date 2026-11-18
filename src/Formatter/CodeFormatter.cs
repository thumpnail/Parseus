namespace Parseus.Formatter;

public sealed class CodeFormatter {
	private readonly List<FormatterRule> _rules = new();
	private readonly FormatterOptions _options;

	public CodeFormatter() : this(new FormatterOptions()) {
	}

	public CodeFormatter(FormatterOptions options) {
		_options = options ?? throw new ArgumentNullException(nameof(options));
	}

	public CodeFormatter AddRule(FormatterRule rule) {
		ArgumentNullException.ThrowIfNull(rule);
		_rules.Add(rule);
		return this;
	}

	public CodeFormatter WithIndentation(string indentation) {
		_options.Indentation = indentation;
		return this;
	}

	public CodeFormatter WithNewLine(string newLine) {
		_options.NewLine = newLine;
		return this;
	}

	public CodeFormatter Block(string openMarker, string closeMarker) {
		AddRule(FormatterRule.IndentAfter(openMarker));
		AddRule(FormatterRule.DedentBefore(closeMarker));
		return this;
	}

	public CodeFormatter NewLineBefore(params string[] markers) {
		foreach (var marker in markers)
			AddRule(FormatterRule.NewLineBefore(marker));
		return this;
	}

	public CodeFormatter NewLineAfter(params string[] markers) {
		foreach (var marker in markers)
			AddRule(FormatterRule.NewLineAfter(marker));
		return this;
	}

	public string FormatSExpressions(string source, params string[] inlinePrefixes) {
		var formatter = new SExpressionFormatter(new SExpressionFormatterOptions {
			Indentation = _options.Indentation,
			NewLine = _options.NewLine
		});
		return formatter.Inline(inlinePrefixes).Format(source);
	}

	public string Format(string source) {
		ArgumentNullException.ThrowIfNull(source);
		_options.Validate();
		if (source.Length == 0)
			return source;

		var inputLines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
		var expandedLines = new List<string>();

		foreach (var inputLine in inputLines) {
			var line = inputLine.Trim();
			if (line.Length == 0) {
				expandedLines.Add(string.Empty);
				continue;
			}

			var parts = new List<string> { line };
			for (var i = 0; i < parts.Count;) {
				var split = false;
				foreach (var rule in _rules.Where(x =>
					         x.Action is FormattingAction.NewLineBefore or FormattingAction.NewLineAfter)) {
					if (TrySplit(parts[i], rule, out var before, out var after)) {
						parts.RemoveAt(i);
						if (before.Length > 0)
							parts.Insert(i++, before);
						if (after.Length > 0)
							parts.Insert(i, after);
						split = true;
						break;
					}
				}

				if (!split)
					i++;
			}

			expandedLines.AddRange(parts);
		}

		var output = new List<string>();
		var indentationLevel = 0;
		var blockComment = false;
		var blankLines = 0;

		foreach (var line in expandedLines) {
			if (line.Length == 0) {
				if (!_options.PreserveBlankLines)
					continue;
				if (blankLines < _options.MaximumConsecutiveBlankLines)
					output.Add(string.Empty);
				blankLines++;
				continue;
			}

			blankLines = 0;
			var trimmed = line.Trim();

			var indentBefore = CountLeadingActions(trimmed, FormattingAction.DedentBefore);
			var indentAfter = CountActions(trimmed, FormattingAction.IndentAfter, ref blockComment);

			var effectiveLevel = Math.Max(0, indentationLevel - indentBefore);
			output.Add(_options.Indentation.Length == 0
				? trimmed
				: string.Concat(Enumerable.Repeat(_options.Indentation, effectiveLevel)) + trimmed);

			indentationLevel = Math.Max(0, indentationLevel - indentBefore + indentAfter);
		}

		while (output.Count > 0 && output[^1].Length == 0)
			output.RemoveAt(output.Count - 1);

		return string.Join(_options.NewLine, output);
	}

	private int CountLeadingActions(string line, FormattingAction action) {
		var count = 0;
		foreach (var rule in _rules.Where(x => x.Action == action)) {
			if (line.StartsWith(rule.Marker, StringComparison.Ordinal)) {
				count++;
				line = line[rule.Marker.Length..].TrimStart();
			}
		}

		return count;
	}

	private int CountActions(string line, FormattingAction action, ref bool blockComment) {
		var count = 0;
		var inString = (char)0;
		var lineComment = false;

		for (var i = 0; i < line.Length;) {
			if (blockComment) {
				var end = line.IndexOf("*/", i, StringComparison.Ordinal);
				if (end < 0)
					break;
				blockComment = false;
				i = end + 2;
				continue;
			}

			if (lineComment)
				break;
			if (inString != 0) {
				if (line[i] == '\\') {
					i += Math.Min(2, line.Length - i);
					continue;
				}

				if (line[i] == inString)
					inString = (char)0;
				i++;
				continue;
			}

			if (i + 1 < line.Length && line[i..(i + 2)] == "//") {
				lineComment = true;
				break;
			}

			if (i + 1 < line.Length && line[i..(i + 2)] == "/*") {
				blockComment = true;
				i += 2;
				continue;
			}

			if (line[i] is '"' or '\'') {
				inString = line[i];
				i++;
				continue;
			}

			var matched = false;
			foreach (var rule in _rules.Where(x => x.Action == action)) {
				if (line.AsSpan(i).StartsWith(rule.Marker, StringComparison.Ordinal)) {
					count++;
					i += rule.Marker.Length;
					matched = true;
					break;
				}
			}

			if (!matched)
				i++;
		}

		return count;
	}

	private static bool TrySplit(string line, FormatterRule rule, out string before, out string after) {
		before = line;
		after = string.Empty;
		var index = FindMarkerOutsideStrings(line, rule.Marker);

		if (index < 0)
			return false;

		if (rule.Action == FormattingAction.NewLineBefore) {
			if (index == 0)
				return false;
			before = line[..index].TrimEnd();
			after = line[index..].Trim();
		} else {
			var end = index + rule.Marker.Length;
			if (end == line.Length)
				return false;
			before = line[..end].Trim();
			after = line[end..].TrimStart();
		}

		return before.Length > 0 && after.Length > 0;
	}

	private static int FindMarkerOutsideStrings(string line, string marker) {
		var inString = (char)0;
		for (var i = 0; i <= line.Length - marker.Length;) {
			if (inString != 0) {
				if (line[i] == '\\') {
					i += Math.Min(2, line.Length - i);
					continue;
				}

				if (line[i] == inString)
					inString = (char)0;
				i++;
				continue;
			}

			if (line[i] is '"' or '\'') {
				inString = line[i];
				i++;
				continue;
			}

			if (i + 1 < line.Length && line.Substring(i, 2) == "//")
				break;
			if (line.AsSpan(i).StartsWith(marker, StringComparison.Ordinal))
				return i;
			i++;
		}

		return -1;
	}
}
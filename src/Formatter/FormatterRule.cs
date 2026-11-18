namespace Parseus.Formatter;

public enum FormattingAction
{
    IndentAfter,
    DedentBefore,
    NewLineBefore,
    NewLineAfter
}

public sealed class FormatterRule
{
    public string Marker { get; }
    public FormattingAction Action { get; }

    public FormatterRule(string marker, FormattingAction action)
    {
        if (string.IsNullOrEmpty(marker))
            throw new ArgumentException("A formatter marker cannot be empty.", nameof(marker));
        Marker = marker;
        Action = action;
    }

    public static FormatterRule IndentAfter(string marker) =>
        new(marker, FormattingAction.IndentAfter);

    public static FormatterRule DedentBefore(string marker) =>
        new(marker, FormattingAction.DedentBefore);

    public static FormatterRule NewLineBefore(string marker) =>
        new(marker, FormattingAction.NewLineBefore);

    public static FormatterRule NewLineAfter(string marker) =>
        new(marker, FormattingAction.NewLineAfter);
}

namespace Parseus.Formatter;

public sealed class FormatterOptions
{
    public string Indentation { get; set; } = "    ";
    public string NewLine { get; set; } = Environment.NewLine;
    public bool PreserveBlankLines { get; set; } = true;
    public int MaximumConsecutiveBlankLines { get; set; } = 1;

    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(Indentation);
        ArgumentNullException.ThrowIfNull(NewLine);
        if (MaximumConsecutiveBlankLines < 0)
            throw new ArgumentOutOfRangeException(nameof(MaximumConsecutiveBlankLines));
    }
}

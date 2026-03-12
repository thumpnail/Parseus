namespace Parseus.Parser.Diagnostics;

/// <summary>
/// A single diagnostic message with level, text, and optional span information.
/// </summary>
public record DiagnosticMessage(DiagnosticLevel Level, string Text, TextSpan? Span = null) {
    public override string ToString() => $"{Level}: {Text}";
}

/// <summary>
/// Represents a complete diagnostic report, similar to Rust's diagnostic format.
/// Includes the primary message, related notes/hints, and source code context.
/// </summary>
public class Diagnostic {
    /// <summary>Primary error/warning message.</summary>
    public DiagnosticMessage Message { get; set; }

    /// <summary>All related messages (Notes, Help, etc.).</summary>
    public List<DiagnosticMessage> RelatedMessages { get; set; } = new();

    /// <summary>The source code this diagnostic refers to.</summary>
    public string? SourceCode { get; set; }

    /// <summary>The filename/label for this diagnostic.</summary>
    public string? SourceLabel { get; set; }

    /// <summary>Line/column information precomputed for performance.</summary>
    internal LineColumnCache? LineCache { get; set; }

    public Diagnostic(DiagnosticMessage message, string? sourceLabel = null) {
        this.Message = message;
        this.SourceLabel = sourceLabel ?? "input";
    }

    /// <summary>
    /// Adds a related message (note, hint, etc.) to this diagnostic.
    /// </summary>
    public Diagnostic WithMessage(DiagnosticLevel level, string text, TextSpan? span = null) {
        RelatedMessages.Add(new DiagnosticMessage(level, text, span));
        return this;
    }

    /// <summary>
    /// Sets the source code for this diagnostic to enable code snippets.
    /// </summary>
    public Diagnostic WithSourceCode(string source) {
        this.SourceCode = source;
        return this;
    }

    /// <summary>
    /// Precompute line/column information for performance.
    /// </summary>
    internal Diagnostic WithLineCache(LineColumnCache cache) {
        this.LineCache = cache;
        return this;
    }
}

/// <summary>
/// Precomputed line and column information for fast lookups.
/// Maps character indices to their line/column positions.
/// </summary>
internal class LineColumnCache {
    private List<int> lineStartIndices = new();

    public LineColumnCache(string source) {
        lineStartIndices.Add(0);
        for (int i = 0; i < source.Length; i++) {
            if (source[i] == '\n') {
                lineStartIndices.Add(i + 1);
            }
        }
    }

    /// <summary>
    /// Gets the line number for a character index (0-based).
    /// </summary>
    public int GetLine(int index) {
        var lineIdx = lineStartIndices.BinarySearch(index);
        return lineIdx < 0 ? (~lineIdx - 1) : lineIdx;
    }

    /// <summary>
    /// Gets the column number for a character index (0-based).
    /// </summary>
    public int GetColumn(int index) {
        var line = GetLine(index);
        return index - lineStartIndices[line];
    }

    /// <summary>
    /// Gets both line and column for a character index.
    /// </summary>
    public (int line, int column) GetLineColumn(int index) {
        var line = GetLine(index);
        var column = index - lineStartIndices[line];
        return (line + 1, column + 1); // 1-based output like Rust
    }

    /// <summary>
    /// Gets the start index of a specific line.
    /// </summary>
    public int GetLineStartIndex(int line) => 
        line >= 0 && line < lineStartIndices.Count ? lineStartIndices[line] : -1;

    /// <summary>
    /// Gets the end index of a specific line.
    /// </summary>
    public int GetLineEndIndex(int line) {
        if (line < 0 || line >= lineStartIndices.Count - 1) return -1;
        return lineStartIndices[line + 1] - 1;
    }

    public int LineCount => lineStartIndices.Count;
}


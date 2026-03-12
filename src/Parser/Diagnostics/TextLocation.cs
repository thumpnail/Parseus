namespace Parseus.Parser.Diagnostics;

/// <summary>
/// Represents a location in source code with line and column information.
/// </summary>
public record TextLocation(int Line, int Column, int Index) {
    /// <summary>
    /// Creates a TextLocation with just index (line/column calculated later if needed).
    /// </summary>
    public TextLocation(int index) : this(0, 0, index) { }

    public override string ToString() => $"{Line}:{Column}";
}

/// <summary>
/// Represents a span of text in source code.
/// </summary>
public record TextSpan(int StartIndex, int Length, TextLocation StartLocation, TextLocation? EndLocation = null) {
    /// <summary>
    /// Creates a TextSpan with just indices (line/column calculated later if needed).
    /// </summary>
    public TextSpan(int startIndex, int length) : this(startIndex, length, new(startIndex), null) { }

    /// <summary>
    /// Gets the end index of this span.
    /// </summary>
    public int EndIndex => StartIndex + Length;

    /// <summary>
    /// Gets a single-character span at the given index.
    /// </summary>
    public static TextSpan At(int index) => new(index, 1);

    /// <summary>
    /// Gets a span from one index to another (inclusive).
    /// </summary>
    public static TextSpan Range(int start, int end) => new(start, end - start + 1);

    public override string ToString() => 
        EndLocation != null 
            ? $"{StartLocation} -> {EndLocation}" 
            : $"{StartLocation}";
}


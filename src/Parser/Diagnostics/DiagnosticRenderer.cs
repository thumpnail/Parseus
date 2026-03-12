namespace Parseus.Parser.Diagnostics;

/// <summary>
/// Renders diagnostics in a Rust-like format with colors, code snippets, and visual markers.
/// Supports ANSI color codes with automatic TTY detection.
/// </summary>
public static class DiagnosticRenderer {
    /// <summary>ANSI color codes for different output components.</summary>
    private static class Colors {
        public const string Reset = "\u001b[0m";
        
        public const string RedBold = "\u001b[1;31m";
        public const string YellowBold = "\u001b[1;33m";
        public const string CyanBold = "\u001b[1;36m";
        public const string GreenBold = "\u001b[1;32m";
        
        public const string Red = "\u001b[31m";
        public const string Cyan = "\u001b[36m";
        
        public const string RedBg = "\u001b[101m";
    }

    /// <summary>Configuration for diagnostic rendering.</summary>
    public class RenderOptions {
        /// <summary>Enable colored output. If null, auto-detects TTY.</summary>
        public bool? UseColors { get; init; }
        
        /// <summary>Number of context lines to show before/after the error.</summary>
        public int ContextLines { get; init; } = 2;
        
        /// <summary>Maximum width for output (for wrapping).</summary>
        public int MaxWidth { get; init; } = 120;
    }

    private static RenderOptions DefaultOptions = new();

    /// <summary>
    /// Determines if we should use colors. Auto-detects TTY if not specified.
    /// </summary>
    private static bool ShouldUseColors(RenderOptions options) {
        if (options.UseColors.HasValue) return options.UseColors.Value;
        
        try {
            // Check if stdout is connected to a terminal
            return Console.IsOutputRedirected == false;
        } catch {
            return false; // Default to no colors on any error
        }
    }

    /// <summary>
    /// Gets the visual marker character for a diagnostic level.
    /// </summary>
    private static char GetMarkerChar(DiagnosticLevel level) => level switch {
        DiagnosticLevel.Error => '^',
        DiagnosticLevel.Warning => '-',
        DiagnosticLevel.Note => '~',
        DiagnosticLevel.Help => '*',
        _ => '?'
    };

    /// <summary>
    /// Gets the color prefix for a diagnostic level.
    /// </summary>
    private static string GetLevelColor(DiagnosticLevel level, bool useColors) => 
        useColors ? (level switch {
            DiagnosticLevel.Error => Colors.RedBold,
            DiagnosticLevel.Warning => Colors.YellowBold,
            DiagnosticLevel.Note => Colors.CyanBold,
            DiagnosticLevel.Help => Colors.GreenBold,
            _ => Colors.Reset
        }) : string.Empty;

    /// <summary>
    /// Gets the level name with proper formatting.
    /// </summary>
    private static string GetLevelName(DiagnosticLevel level) => level switch {
        DiagnosticLevel.Error => "error",
        DiagnosticLevel.Warning => "warning",
        DiagnosticLevel.Note => "note",
        DiagnosticLevel.Help => "help",
        _ => "message"
    };

    /// <summary>
    /// Renders a single diagnostic to a string.
    /// </summary>
    public static string Render(Diagnostic diagnostic, RenderOptions? options = null) {
        options ??= DefaultOptions;
        var useColors = ShouldUseColors(options);
        
        var result = new System.Text.StringBuilder();
        
        // Primary message header
        var levelColor = GetLevelColor(diagnostic.Message.Level, useColors);
        var levelName = GetLevelName(diagnostic.Message.Level);
        var reset = useColors ? Colors.Reset : string.Empty;
        
        result.Append($"{levelColor}{levelName}{reset}");
        
        if (!string.IsNullOrEmpty(diagnostic.SourceLabel)) {
            result.Append($": {diagnostic.SourceLabel}");
        }
        
        result.AppendLine();
        result.Append("  ");
        result.AppendLine(diagnostic.Message.Text);
        result.AppendLine();
        
        // Code snippet with visual markers
        if (diagnostic.SourceCode != null && diagnostic.Message.Span != null) {
            var snippet = RenderCodeSnippet(
                diagnostic.SourceCode, 
                diagnostic.Message.Span, 
                diagnostic.LineCache,
                options,
                useColors
            );
            result.Append(snippet);
        }
        
        // Related messages (notes, help, etc.)
        foreach (var msg in diagnostic.RelatedMessages) {
            var relatedColor = GetLevelColor(msg.Level, useColors);
            var relatedName = GetLevelName(msg.Level);
            
            result.Append($"{relatedColor}{relatedName}{reset}: {msg.Text}");
            result.AppendLine();
            
            if (diagnostic.SourceCode != null && msg.Span != null) {
                var snippet = RenderCodeSnippet(
                    diagnostic.SourceCode,
                    msg.Span,
                    diagnostic.LineCache,
                    options,
                    useColors
                );
                result.Append(snippet);
            }
        }
        
        return result.ToString();
    }

    /// <summary>
    /// Renders a code snippet with context and visual markers.
    /// </summary>
    private static string RenderCodeSnippet(
        string source,
        TextSpan span,
        LineColumnCache? cache,
        RenderOptions options,
        bool useColors) {
        
        // Build or use cache
        cache ??= new LineColumnCache(source);
        
        var (startLine, startCol) = cache.GetLineColumn(span.StartIndex);
        var (endLine, endCol) = cache.GetLineColumn(span.EndIndex - 1);
        
        var result = new System.Text.StringBuilder();
        var lines = source.Split('\n');
        
        // Determine which lines to show
        int contextStart = Math.Max(0, startLine - options.ContextLines - 1);
        int contextEnd = Math.Min(lines.Length - 1, endLine + options.ContextLines - 1);
        
        // Line number width for alignment
        int lineNumWidth = (contextEnd + 1).ToString().Length;
        
        // Render each line with context
        for (int i = contextStart; i <= contextEnd; i++) {
            if (i >= lines.Length) break;
            
            var lineNum = (i + 1).ToString().PadLeft(lineNumWidth);
            var lineText = i < lines.Length ? lines[i] : string.Empty;
            
            // Header with line number
            var separator = useColors ? Colors.Cyan : string.Empty;
            var reset = useColors ? Colors.Reset : string.Empty;
            result.Append($"{separator}  {lineNum} |{reset} ");
            
            // Highlight the error region in this line if it spans it
            if (i >= startLine - 1 && i <= endLine - 1) {
                // Calculate the actual start index of this line in the source
                int lineStartIdx = 0;
                for (int j = 0; j < i; j++) {
                    lineStartIdx += lines[j].Length + 1; // +1 for newline
                }
                
                var errorStart = Math.Max(0, span.StartIndex - lineStartIdx);
                var errorEnd = Math.Min(lineText.Length, span.EndIndex - lineStartIdx);
                
                if (errorEnd > errorStart) {
                    var before = lineText.Substring(0, errorStart);
                    var error = lineText.Substring(errorStart, errorEnd - errorStart);
                    var after = lineText.Substring(errorEnd);
                    
                    var errorColor = useColors ? Colors.RedBg : string.Empty;
                    result.Append($"{before}{errorColor}{error}{reset}{after}");
                } else {
                    result.Append(lineText);
                }
            } else {
                result.Append(lineText);
            }
            
            result.AppendLine();
            
            // Error marker line
            if (i >= startLine - 1 && i <= endLine - 1) {
                result.Append(new string(' ', lineNumWidth + 3)); // Align with line content
                
                var markerColor = useColors ? Colors.Red : string.Empty;
                
                if (i == startLine - 1 && i == endLine - 1) {
                    // Single-line error: show markers under the error region
                    result.Append(new string(' ', startCol - 1));
                    result.Append($"{markerColor}");
                    result.Append(new string(GetMarkerChar(DiagnosticLevel.Error), Math.Max(1, endCol - startCol + 1)));
                    result.Append($"{reset}");
                } else if (i == startLine - 1) {
                    // First line of multi-line error
                    result.Append(new string(' ', startCol - 1));
                    result.Append($"{markerColor}");
                    result.Append(new string('^', lineText.Length - startCol + 1));
                    result.Append($"{reset}");
                } else if (i == endLine - 1) {
                    // Last line of multi-line error
                    result.Append($"{markerColor}");
                    result.Append(new string('^', endCol));
                    result.Append($"{reset}");
                } else {
                    // Middle lines of multi-line error
                    result.Append($"{markerColor}");
                    result.Append(new string('^', lineText.Length));
                    result.Append($"{reset}");
                }
                
                result.AppendLine();
            }
        }
        
        return result.ToString();
    }

    /// <summary>
    /// Renders multiple diagnostics at once.
    /// </summary>
    public static string RenderAll(IEnumerable<Diagnostic> diagnostics, RenderOptions? options = null) {
        var result = new System.Text.StringBuilder();
        foreach (var diagnostic in diagnostics) {
            result.Append(Render(diagnostic, options));
            result.AppendLine();
        }
        return result.ToString();
    }

    /// <summary>
    /// Outputs a diagnostic directly to the console.
    /// </summary>
    public static void Output(Diagnostic diagnostic, RenderOptions? options = null) {
        Console.Write(Render(diagnostic, options));
    }

    /// <summary>
    /// Outputs multiple diagnostics directly to the console.
    /// </summary>
    public static void OutputAll(IEnumerable<Diagnostic> diagnostics, RenderOptions? options = null) {
        Console.Write(RenderAll(diagnostics, options));
    }

    /// <summary>
    /// Gets a summary line like "error: aborting due to 1 error and 2 warnings"
    /// </summary>
    public static string GetSummary(IEnumerable<Diagnostic> diagnostics) {
        var errors = 0;
        var warnings = 0;
        var notes = 0;
        
        foreach (var diag in diagnostics) {
            switch (diag.Message.Level) {
                case DiagnosticLevel.Error: errors++; break;
                case DiagnosticLevel.Warning: warnings++; break;
                case DiagnosticLevel.Note: notes++; break;
            }
            errors += diag.RelatedMessages.Count(m => m.Level == DiagnosticLevel.Error);
            warnings += diag.RelatedMessages.Count(m => m.Level == DiagnosticLevel.Warning);
            notes += diag.RelatedMessages.Count(m => m.Level == DiagnosticLevel.Note);
        }
        
        var parts = new List<string>();
        if (errors > 0) parts.Add($"{errors} error{(errors != 1 ? "s" : "")}");
        if (warnings > 0) parts.Add($"{warnings} warning{(warnings != 1 ? "s" : "")}");
        if (notes > 0) parts.Add($"{notes} note{(notes != 1 ? "s" : "")}");
        
        if (parts.Count == 0) return "completed successfully";
        
        return $"aborting due to {string.Join(" and ", parts)}";
    }
}






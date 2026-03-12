namespace Parseus.Parser.Diagnostics;

/// <summary>
/// Severity level for diagnostics, similar to Rust's diagnostic levels.
/// </summary>
public enum DiagnosticLevel {
    /// <summary>Error: Fatal problem that prevents compilation</summary>
    Error = 0,
    
    /// <summary>Warning: Potential problem that should be addressed</summary>
    Warning = 1,
    
    /// <summary>Note: Additional informational message</summary>
    Note = 2,
    
    /// <summary>Help: Suggestion for fixing the problem</summary>
    Help = 3
}


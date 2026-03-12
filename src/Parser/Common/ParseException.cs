using Parseus.Util;
using Parseus.Parser.Diagnostics;

namespace Parseus.Parser.Common;
public enum LogLevel {
    none = 0,
    error = 1,
    warning = 2,
    info = 3
}
public class ParseException : Exception {
    /// <summary>Optional diagnostic information with code snippets and formatting.</summary>
    public Diagnostic? Diagnostic { get; private set; }

    public ParseException(string message, string caller, StreamWriter? logWriter = null) : base(message) {
        if (logWriter is not null) {
            Console.WriteLine(LogFormater.FormatLog(message,caller));
            logWriter.WriteLine(LogFormater.FormatLog(message,caller));
        }
    }
    public ParseException(string message, string caller, Exception innerException, StreamWriter? logWriter = null) : base(message, innerException) {
        if (logWriter is not null) {
            Console.WriteLine(LogFormater.FormatLog(message,caller));
            logWriter.WriteLine(LogFormater.FormatLog(message,caller));
        }
    }

    /// <summary>
    /// Creates a ParseException from a Diagnostic.
    /// </summary>
    public ParseException(Diagnostic diagnostic) : base(diagnostic.Message.Text) {
        this.Diagnostic = diagnostic;
        // Auto-output the diagnostic with nice formatting
        DiagnosticRenderer.Output(diagnostic);
    }

    /// <summary>
    /// Creates a ParseException from a diagnostic message.
    /// </summary>
    public ParseException(DiagnosticMessage message, string? sourceLabel = null) 
        : this(new Diagnostic(message, sourceLabel)) { }
}
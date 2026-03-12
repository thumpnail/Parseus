using Parseus.Parser.Diagnostics;
namespace Parseus.Parser.Common;

public class CancellationState {
	public bool Ok = true;
    public Stack<string> reasonStack = new();
    
    /// <summary>Collects diagnostics (errors, warnings, notes) during parsing.</summary>
    public List<Diagnostic> Diagnostics { get; private set; } = new();
    
    public void FullReset() {
        Ok = true;
        if (reasonStack.Count > 0) {
            reasonStack.Clear();
        }
    }
    public void Flag(string reason) {
        Ok = false;
        reasonStack.Push(reason);
    }
    
    /// <summary>Reports a diagnostic message.</summary>
    public void ReportDiagnostic(DiagnosticMessage message, string? sourceLabel = null) {
        var diagnostic = new Diagnostic(message, sourceLabel);
        Diagnostics.Add(diagnostic);
        
        if (message.Level == DiagnosticLevel.Error) {
            Ok = false;
        }
    }
    
    /// <summary>Reports an error diagnostic.</summary>
    public void ReportError(string message, TextSpan? span = null, string? sourceLabel = null) {
        ReportDiagnostic(new DiagnosticMessage(DiagnosticLevel.Error, message, span), sourceLabel);
    }
    
    /// <summary>Reports a warning diagnostic.</summary>
    public void ReportWarning(string message, TextSpan? span = null, string? sourceLabel = null) {
        ReportDiagnostic(new DiagnosticMessage(DiagnosticLevel.Warning, message, span), sourceLabel);
    }
    
    /// <summary>Reports a note diagnostic.</summary>
    public void ReportNote(string message, TextSpan? span = null, string? sourceLabel = null) {
        ReportDiagnostic(new DiagnosticMessage(DiagnosticLevel.Note, message, span), sourceLabel);
    }
    
    /// <summary>Returns true if there are any diagnostics.</summary>
    public bool HasDiagnostics => Diagnostics.Count > 0;
    
    /// <summary>Returns true if there are any error diagnostics.</summary>
    public bool HasErrors => Diagnostics.Any(d => d.Message.Level == DiagnosticLevel.Error) ||
                             Diagnostics.Any(d => d.RelatedMessages.Any(m => m.Level == DiagnosticLevel.Error));
    
    /// <summary>Returns true if there are any warning diagnostics.</summary>
    public bool HasWarnings => Diagnostics.Any(d => d.Message.Level == DiagnosticLevel.Warning) ||
                               Diagnostics.Any(d => d.RelatedMessages.Any(m => m.Level == DiagnosticLevel.Warning));
    
    public override string ToString() {
        if (reasonStack.Count > 0 && !Ok) {
            return $"{(Ok?"OK":"ERR")} | {string.Join(",",reasonStack.ToList().Last())}";
        }
        return $"{(Ok?"OK":"ERR")} | ---";
    }
	
	public string? ToString(string source) {
		if (reasonStack.Count > 0 && !Ok) {
			return $"[{source}] {(Ok?"OK":"ERR")} | {string.Join(",",reasonStack.ToList().Last())}";
		}
		return null;
	}
}
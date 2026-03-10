namespace Parseus.Parser.Common;

public class CancellationState {
	public bool Ok = true;
    public Stack<string> reasonStack = new Stack<string>();
    public void Reset() {
        Ok = true;
        if (reasonStack.Count > 0) {
            reasonStack.Pop();
        }
    }
    public void Flag(string reason) {
        Ok = false;
        reasonStack.Push(reason);
    }
    public override string ToString() {
        if (reasonStack.Count > 0) {
            return $"{(Ok?"OK":"ERR")} | {string.Join(",",reasonStack.ToList().Last())}";
        }
        return $"{(Ok?"OK":"ERR")} | ---";
    }
	
	public string ToString(string source) {
		if (reasonStack.Count > 0) {
			return $"[{source}] {(Ok?"OK":"ERR")} | {string.Join(",",reasonStack.ToList().Last())}";
		}
		return $"{(Ok?"OK":"ERR")} | ---";
	}
}
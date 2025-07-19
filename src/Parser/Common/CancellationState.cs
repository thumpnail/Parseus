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
            return $"{Ok} | {reasonStack.Peek()}";
        }
        return $"{Ok} | ---";
    }
}
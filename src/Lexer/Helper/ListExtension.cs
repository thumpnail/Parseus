namespace Parseus.Lexer.Helper;

public static class ListExtension {
	public static bool TryGetValue<T>(this List<T> list, int index, out T value) {
		if (list.Count >= index && index>=0) {
			value = list[index];
			return true;
		} else {
			value = default;
			return false;
		}
	}
}
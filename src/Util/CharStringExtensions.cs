namespace Parseus.Util;

public static class CharStringExtensions {
    public static bool IsAlpha(this char c) {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }
    public static bool IsDigit(this char c) {
        return c >= '0' && c <= '9';
    }
    public static bool IsWhiteSpace(this char c) {
        return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }
    public static bool IsNewLine(this char c) {
        return c == '\n' || c == '\r';
    }
    public static bool IsSymbol(this char c) {
        return !IsAlpha(c) && !IsDigit(c) && !IsWhiteSpace(c) && !IsNewLine(c) && !IsSpecial(c);
    }
    public static bool IsSpecial(this char c) {
        return c == '!' || c == '@' || c == '#' || c == '$' || c == '%' || c == '^' || c == '&' || c == '*' ||
               c == '(' || c == ')' || c == '-' || c == '_' || c == '=' || c == '+' || c == '{' || c == '}' ||
               c == '[' || c == ']' || c == ':' || c == ';' || c == '"' || c == '\'' || c == '<' || c == '>' ||
               c == ',' || c == '.' || c == '?' || c == '/' || c == '\\' || c == '|' || c == '`' || c == '~';
    }
    public static bool Is(this char c, params char[] chars) {
        foreach (var ch in chars) {
            if (c == ch) {
                return true;
            }
        }
        return false;
    }
}
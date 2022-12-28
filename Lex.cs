namespace parser;
public class Lexer {
    public enum TokenType : int {
        //TODO: Gen Enum
        unkown,

        k_identifier,

        t_number,
        t_string,
        t_char,
        t_charArray,
        t_array,
        t_bool,
        t_nil,

        s_bracketOpen,
        s_bracketClose,
        s_rBracketOpen,
        s_rBracketClose,
        s_cBracketOpen,
        s_cBracketClose,

        l_and,
        l_doubleand,
        l_or,
        l_doubleor,
        l_greater,
        l_less,
        l_equals,
        l_lesequals,
        l_greaterequals,
        l_notequals,

        k_break,
        k_continue,
        k_data,
        k_fnc,
        k_loop,
        k_end,
        k_if,
        k_elseif,
        k_else,
        t_void,
        t_any,

        b_false,
        b_true,

        o_dot,
        o_comma,
        o_tilde,
        o_colon,
        o_questionmark,
        o_plus,
        o_minus,
        o_multiply,
        o_divide,
        o_doublePlus,
        o_doubleMinus,
        o_doubleColon,
        o_doubleRight,
        o_right,
        o_left,
        o_doubleLeft,
        o_percent,
        o_exlamationmark,

        s_comment,
        s_char,
        s_string,
        o_simpleeq,
        o_adveq,
        EOL,
        EOF = -1
    }
    public static Dictionary<char, TokenType> symbols = new() {
        //TODO: Gen Symbols
        [':'] = TokenType.o_colon,
        ['='] = TokenType.o_simpleeq,
        ['\''] = TokenType.s_char,
        ['"'] = TokenType.s_string,
        ['{'] = TokenType.s_cBracketOpen,
        ['}'] = TokenType.s_cBracketClose,
        ['['] = TokenType.s_bracketOpen,
        [']'] = TokenType.s_bracketClose,
        ['('] = TokenType.s_rBracketOpen,
        [')'] = TokenType.s_rBracketClose,
        ['<'] = TokenType.o_right,
        ['>'] = TokenType.o_left,
        ['?'] = TokenType.o_questionmark,
        ['!'] = TokenType.o_exlamationmark,
        ['+'] = TokenType.o_plus,
        ['-'] = TokenType.o_minus,
        ['*'] = TokenType.o_multiply,
        ['/'] = TokenType.o_divide,
        ['.'] = TokenType.o_dot,
        [','] = TokenType.o_comma,
        ['#'] = TokenType.s_comment,
        ['?'] = TokenType.o_questionmark,
        ['!'] = TokenType.o_exlamationmark,
        ['|'] = TokenType.l_or,
        ['&'] = TokenType.l_and
    };
    public static Dictionary<string, TokenType> keywords = new() {
          ["EOF"] = TokenType.EOF,
          ["EOL"] = TokenType.EOL,
          // TODO: Gen Keywords
          ["true"] = TokenType.b_true,
          ["false"] = TokenType.b_false,
          ["break"] = TokenType.k_break,
          ["continue"] = TokenType.k_continue,
          ["data"] = TokenType.k_data,
          ["fnc"] = TokenType.k_fnc,
          ["loop"] = TokenType.k_loop,
          ["end"] = TokenType.k_end,
          ["if"] = TokenType.k_if,
          ["elseif"] = TokenType.k_elseif,
          ["else"] = TokenType.k_else,
          ["number"] = TokenType.t_number,
          ["string"] = TokenType.t_string,
          ["bool"] = TokenType.t_bool,
          ["void"] = TokenType.t_void,
          ["any"] = TokenType.t_any,

          ["number"] = TokenType.t_number,
          ["string"] = TokenType.t_string,
          ["char"] = TokenType.t_char,
          ["bool"] = TokenType.t_bool,
          ["any"] = TokenType.t_any,
          ["void"] = TokenType.unkown,
      };
    public static Dictionary<string, TokenType> operators = new() {
        ["+"] = TokenType.o_plus,
        ["++"] = TokenType.o_doublePlus,
        ["-"] = TokenType.o_minus,
        ["--"] = TokenType.o_doubleMinus,
        ["*"] = TokenType.o_multiply,
        ["/"] = TokenType.o_divide,
        ["%"] = TokenType.o_percent,
        ["<<"] = TokenType.o_doubleLeft,
        [">>"] = TokenType.o_doubleRight,
        ["||"] = TokenType.l_doubleor,
        ["&&"] = TokenType.l_doubleand,
        ["<="] = TokenType.l_lesequals,
        [">="] = TokenType.l_greaterequals,
        ["=="] = TokenType.l_equals,
        ["!="] = TokenType.l_notequals,
        ["<"] = TokenType.o_left,
        [">"] = TokenType.o_right,
        ["?="] = TokenType.unkown,
        ["="] = TokenType.o_simpleeq,
        [":="] = TokenType.o_adveq,
        [":"] = TokenType.o_colon,
        ["="] = TokenType.o_simpleeq,
        ["'"] = TokenType.s_char,
        ["\""] = TokenType.s_string,
        ["{"] = TokenType.s_cBracketOpen,
        ["}"] = TokenType.s_cBracketClose,
        ["["] = TokenType.s_bracketOpen,
        ["]"] = TokenType.s_bracketClose,
        ["("] = TokenType.s_rBracketOpen,
        [")"] = TokenType.s_rBracketClose,
        ["<"] = TokenType.o_left,
        [">"] = TokenType.o_right,
        ["?"] = TokenType.o_questionmark,
        ["!"] = TokenType.o_exlamationmark,
        ["+"] = TokenType.o_plus,
        ["-"] = TokenType.o_minus,
        ["*"] = TokenType.o_multiply,
        ["/"] = TokenType.o_divide,
        ["."] = TokenType.o_dot,
        [","] = TokenType.o_comma,
        ["#"] = TokenType.s_comment,
        ["?"] = TokenType.o_questionmark,
        ["!"] = TokenType.o_exlamationmark,
        ["|"] = TokenType.l_or,
        ["&"] = TokenType.l_and,
    };

    public static List<string> preprocessor(string[] scriptLines) {
        bool isComment = false, isString = false, isChar = false, isNumber = false;
        List<string> words = new();
        string word = "";
        (string, int, int) currentpos; // for errors // TODO:
        foreach (var line in scriptLines) {
            word = "";
            for (int i = 0; i < line.Length; i++) {

                char c = line[i], next = ' ';
                if (i + 1 < line.Length)
                    next = line[i + 1];

                // string and chars
                if (isString || isChar) {
                    if (c != '\"') {
                        word += c;
                        continue;
                    }
                }
                if (c == '#' | isComment) {
                    isComment = true;
                    continue;
                }

                if ((c == '-' && ArrayContains(next, "1234567890.".ToArray()) ||
                     ArrayContains(c, "1234567890.".ToArray()))) {
                    word += c;
                    continue;
                }

                if (c == ' ') {
                    // whitespace
                    if (word != "") {
                        words.Add(word);
                        word = "";
                    }
                }

                if (ArrayContains(c,
                                  "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
                                      .ToArray())) {
                    // word
                    word += c;
                }

                if (ArrayContains(c, symbols.Keys.ToArray())) {
                    // other symbols
                    if (ArrayContains(next, symbols.Keys.ToArray())) {
                        string tmp = "" + c + next;
                        if (operators.ContainsKey(tmp)) {
                            word = "" + c + next;
                            words.Add(word);
                            word = "";
                            i += 2;
                            continue;
                        }
                    }
                    if (c == '\"') {
                        while (i++ < line.Length && (line[i] != '\"')) {
                            word += line[i];
                        }
                        continue;
                    }
                    if (c == '\'') {
                        while (i++ < line.Length && (line[i] != '\'')) {
                            word += line[i];
                        }
                        continue;
                    }
                    if (word != "") {
                        words.Add(word);
                        word = "";
                    }
                    word += c;
                    if (word != "") {
                        words.Add(word);
                        word = "";
                    }
                }
            }
            isComment = false;
            isString = false;
            isChar = false;
            if (word != "") {
                words.Add(word);
            }
            try {
                if (words[words.Count - 1] != "EOL") {
                    words.Add("EOL");
                }
            } catch (Exception) {
            }
        }
        words.Add("EOF");
        return words;
    }
    private static bool ArrayContains(char c, char[] arr) {
        foreach (var item in arr)
            if (item == c) {
                return true;
            }
        return false;
    }
    private static bool ArrayContains(string c, string[] arr) {
        foreach (var item in arr)
            if (item == c) {
                return true;
            }
        return false;
    }

    public static List<Tuple<TokenType, string>> LexIt(List<string> words) {
        List<Tuple<TokenType, string>> final = new();
        bool insideComment = false;
        for (int i = 0; i < words.Count; i++) {
            TokenType token = TokenType.unkown;
            string str = words[i];
            // is digit
            if (StringExistOf(words[i], "-1234567890".ToArray())) {
                token = TokenType.t_number;
            } else if (StringExistOf(words[i], "-1234567890.".ToArray())) {
                token = TokenType.t_number;
                if (!StringExistOf(words[i], "-1234567890".ToArray()))
                    token = TokenType.o_dot;
            } else {
                // special keywords
                if (keywords.ContainsKey(str)) {
                    token = keywords[str];
                } else
                  // symbols and operator
                  if (operators.ContainsKey(str)) {
                    token = operators[str];
                } else
                    // strings and chars
                    if (str.Contains('"')) {
                    token = TokenType.t_string;
                } else if (str.Contains('\'')) {
                    token = TokenType.t_char;
                    if (str.Length > 3) {
                        token = TokenType.t_charArray;
                    }
                } else {
                    token = TokenType.k_identifier;
                }
            }
            final.Add(new Tuple<TokenType, string>(token, str));
            str = "";
        }
        // commentfix
        insideComment = false;
        List<Tuple<TokenType, string>> remlist = new();
        foreach (var item in final) {
            if (item.Item2.Contains("\n")) {
                insideComment = false;
            } else if (item.Item2.Equals("\n")) {
                remlist.Add(item);
            } else if (item.Item2 == "#") {
                remlist.Add(item);
                insideComment = true;
            } else if (insideComment) {
                remlist.Add(item);
            }
        }
        return final;
    }
    #region helper
    public static bool StringExistOf(string src, char[] arr) {
        int check = 0;
        foreach (var item in src) {
            if (arr.Contains(item)) {
                check++;
            }
        }
        return check == src.Length; // Exist Of
    }
    public static bool StringContains(string src, char[] arr) {
        int check = 0;
        foreach (var item in src) {
            if (arr.Contains(item)) {
                check++;
            }
        }
        return check >= 0; // Contains
    }
    public static bool StringContains(string src, string[] arr) {
        return arr.Contains(src);
    }
    public static bool StringEquals(string src, char[] arr) {
        int check = 0;
        foreach (var c in arr) {
            if (src.Equals(c)) {
                check++;
            }
        }
        return check == src.Length;
    }
    public string DiscoverString() { return ""; }
    #endregion
}

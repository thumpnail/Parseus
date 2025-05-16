namespace Parseus.Util; 

public static class Helper {
    public static string Strip(string value) {
        string result = "";
        int wsc = 0; //Whitespace count
        int nlc = 0; //Newline count
        int dac = 0;
        for (int i = 0; i < value.Length; i++) {
            if (value[i] == 0x20 || value[i] == ' ') {
                if (wsc == 0)
                    result += ' ';
                wsc++;
            } else if (value[i] == '\n' || value[i] == 0x0D || value[i] == 0x0A) {
                if (nlc == 0)
                    result += '\n';
                nlc++;
            } else if (value[i] == '\t') {
                //do nothing
            } else if (value[i] == '-' || value[i] == 0x2D) {
                //do nothing
                //TODO: Put significant '-' into result. strip rest
                if (dac == 0)
                    result += '-';
                dac++;
            } else {
                wsc = 0;
                nlc = 0;
                dac = 0;
                result += value[i];
            }
        }
        return result.Trim();
    }
    public static string[] Arrayify(this string str) {
        var tmp_str = str.Replace("\n", "°\n°").Replace(" ", "°").Split('°').ToList();
        tmp_str.RemoveAll(x => x == "");
        return tmp_str.ToArray();
    }
    public static string ToString(this object obj) {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }
    public static bool ContainsOnlyNumbers(this string str) {
        return !string.IsNullOrEmpty(str) && str.All(c => char.IsDigit(c));
    }
    public static bool ContainsNumbers(this string str) {
        return !string.IsNullOrEmpty(str) && str.Any(c => char.IsDigit(c));
    }
    public static bool ContainsOnlyLetters(this string str) {
        return !string.IsNullOrEmpty(str) && str.All(c => char.IsLetter(c));
    }
    public static bool ContainsLetters(this string str) {
        return !string.IsNullOrEmpty(str) && str.Any(c => char.IsLetter(c));
    }
}
using Newtonsoft.Json;
namespace parser;
public static class Helper {
    public static string Dump(this object obj) {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}


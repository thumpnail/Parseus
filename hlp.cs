using Newtonsoft.Json;
namespace parser;
public static class hlp {
    public static string dmp(object obj) {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}


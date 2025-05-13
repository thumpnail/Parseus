using System.Reflection;
namespace Parseus.Util;

public static class ToStringUtil
{
    public static string ToFormattedString<T>(T obj)
    {
        if (obj == null) return "null";

        Type type = obj.GetType();
        var properties = type.GetProperties();
        var fields = type.GetFields();
        var members = type.GetMembers();
        
        var values = new List<(string name, string value)>();
        

        string propertiesString = string.Join(", ", 
            properties.Select(p => $"{p.Name} = {p.GetValue(obj)}")) 
                                  + fields.Select(p => $"{p.Name} = {p.GetValue(obj)}") 
                                  + members.Select(p => $"{p.Name}");

        return $"{type.Name} {{ {propertiesString} }}";
    }
}
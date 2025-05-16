using System.Globalization;
using Parseus.Parser.Common;
namespace Parseus.Util;

public static class LogFormater {
    public static string FormatLog(
        string message, 
        string caller,
        LogLevel logLevel = LogLevel.none,
        Exception exception = null) 
    {
        return $"[{logLevel.ToString()}][{caller}][{DateTime.Now}] {message}";
    }
}
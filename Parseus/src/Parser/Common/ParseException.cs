<<<<<<< Updated upstream
﻿using Parseus.Util;
namespace Parseus.Parser.Common;
public enum LogLevel : int {
    none = 0,
    error = 1,
    warning = 2,
    info = 3
}
public class ParseException : Exception {
    public ParseException(string message, string caller, StreamWriter logWriter = null) : base(message) {
        if (logWriter is not null) {
            Console.WriteLine(LogFormater.FormatLog(message,caller));
            logWriter.WriteLine(LogFormater.FormatLog(message,caller));
        }
    }
    public ParseException(string message, string caller, Exception innerException, StreamWriter logWriter = null) : base(message, innerException) {
        if (logWriter is not null) {
            Console.WriteLine(LogFormater.FormatLog(message,caller));
            logWriter.WriteLine(LogFormater.FormatLog(message,caller));
        }
    }
=======
﻿using Parseus.Util;
namespace Parseus.Parser.Common;
public enum LogLevel : int {
    none = 0,
    error = 1,
    warning = 2,
    info = 3
}
public class ParseException : Exception {
    public ParseException(string message, string caller, StreamWriter logWriter = null) : base(message) {
        if (logWriter is not null) {
            Console.WriteLine(LogFormater.FormatLog(message,caller));
            logWriter.WriteLine(LogFormater.FormatLog(message,caller));
        }
    }
    public ParseException(string message, string caller, Exception innerException, StreamWriter logWriter = null) : base(message, innerException) {
        if (logWriter is not null) {
            Console.WriteLine(LogFormater.FormatLog(message,caller));
            logWriter.WriteLine(LogFormater.FormatLog(message,caller));
        }
    }
>>>>>>> Stashed changes
}
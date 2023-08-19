using System.Collections;
using ParseKit.Lexer;
using ParseKit.Util;

namespace ParseKit.Parser;

public static partial class ParserModule {
    public class ParserException : Exception  {
        public override string Message { get; }
        public override IDictionary Data { get; }
        public override string? Source { get; set; }
        public ParserException() {
            Message = "Error while parsing";
            Source = "";
        }
    }
}
<<<<<<< Updated upstream
﻿using System.Text;
namespace Parseus.Parser.BasicParser.SBNF;

public partial class SBNFParserGenerator {
    public string ParsableClass = 
        """
        public abstract class Parsable {
            protected IParserContext Context;
            public Parsable() {
                Context = new ParserContext([]);
            }
            public Parsable(IParserContext context) {
                Context = context;
            }
        
            public Parsable? Parse() {
                return null;
            }
            
            internal void ParseAlt(params Action[] actions) {
                foreach (var action in actions) {
                    try {
                        action();
                        return;
                    } catch (ParseException) {
                        // Ignore exception and try the next alternative
                    }
                }
                throw new ParseException("Failed to parse alternative options");
            }
            
            internal void ParseLiteral(string literal, out bool matched) {
                if (Context.MatchToken(literal)) {
                    Context.NextToken();
                    matched = true;
                } else {
                    matched = false;
                }
            }
            
            internal void ParseToken(string tokenType, out string value) {
                if (!Context.HasMoreTokens()) throw new ParseException($"Expected token of type {tokenType}, but got end of input");
                var token = Context.NextToken();
                if (token.Type != tokenType) throw new ParseException($"Expected token of type {tokenType}, but got {token.Type}");
                value = token.Value;
            }
            
            internal void ParseOpt(Action action) {
                try {
                    action();
                } catch (ParseException) {
                    // Optional parsing means failure is okay, ignore
                }
            }
            internal void ParseRepeat(params Action[] actions) {
                try {
                    foreach (var action in actions) {
                        try {
                            action();
                        }
                        catch (ParseException e) {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
                catch (ParseException e) {
                    Console.WriteLine(e);
                }
            }
            
            internal void ParseNode<T>(ref T? node) where T : Parsable?, new() {
                node = new T { Context = Context } as T;
                node.Parse();
                if (node == null) throw new ParseException("Failed to parse node");
            }
        }                 
        """;
    public string ParserExceptionClass =
        """
        public class ParseException : Exception {
            public ParseException(string message) : base(message) {}
            public ParseException(string message, Exception innerException) : base(message, innerException) {}
        }
        """;
    public string ParserContextClass =
        """
        public interface IParserContext {
            Token NextToken();
            Token PeekToken(int offset = 0);
            bool MatchToken(string expectedValue);
            bool HasMoreTokens();
        }
        public class ParserContext : IParserContext {
            private readonly List<Token> _tokens;
            private int _position;
        
            public ParserContext(List<Token> tokens) {
                _tokens = tokens;
                _position = 0;
            }
        
            public Token NextToken() {
                if (!HasMoreTokens())
                    throw new ParseException("Unexpected end of token stream");
                return _tokens[_position++];
            }
        
            public Token PeekToken(int offset = 0) {
                if (_position + offset >= _tokens.Count)
                    throw new ParseException("Unexpected end of token stream while peeking");
                return _tokens[_position + offset];
            }
        
            public bool MatchToken(string expectedValue) {
                if (!HasMoreTokens()) return false;
                return PeekToken().Value == expectedValue;
            }
        
            public bool HasMoreTokens() {
                return _position < _tokens.Count;
            }
        }
        """;
    public string ParserAstNodeClass =
        """
        public class AstNode
        {
            public string Type { get; }
            public string Value { get; }
            public List<AstNode> Children { get; }
        
            public AstNode(string type, string value = null)
            {
                Type = type;
                Value = value;
                Children = new List<AstNode>();
            }
        
            public AstNode(string type, List<AstNode> children, string value = null)
            {
                Type = type;
                Children = children;
                Value = value;
            }
        
            public override string ToString()
            {
                return Value != null ? $"{Type}({Value})" : $"{Type}[{string.Join(", ", Children)}]";
            }
        }
        """;
    public string ParserTokenClass =
        """
        public class Token {
            public string Type { get; }
            public string Value { get; }
        
            public Token(string type, string value) {
                Type = type;
                Value = value;
            }
        }
        """;
=======
﻿using System.Text;
namespace Parseus.Parser.BasicParser.SBNF;

public partial class SBNFParserGenerator {
    public string ParsableClass = 
        """
        public abstract class Parsable {
            protected IParserContext Context;
            public Parsable() {
                Context = new ParserContext([]);
            }
            public Parsable(IParserContext context) {
                Context = context;
            }
        
            public Parsable? Parse() {
                return null;
            }
            
            internal void ParseAlt(params Action[] actions) {
                foreach (var action in actions) {
                    try {
                        action();
                        return;
                    } catch (ParseException) {
                        // Ignore exception and try the next alternative
                    }
                }
                throw new ParseException("Failed to parse alternative options");
            }
            
            internal void ParseLiteral(string literal, out bool matched) {
                if (Context.MatchToken(literal)) {
                    Context.NextToken();
                    matched = true;
                } else {
                    matched = false;
                }
            }
            
            internal void ParseToken(string tokenType, out string value) {
                if (!Context.HasMoreTokens()) throw new ParseException($"Expected token of type {tokenType}, but got end of input");
                var token = Context.NextToken();
                if (token.Type != tokenType) throw new ParseException($"Expected token of type {tokenType}, but got {token.Type}");
                value = token.Value;
            }
            
            internal void ParseOpt(Action action) {
                try {
                    action();
                } catch (ParseException) {
                    // Optional parsing means failure is okay, ignore
                }
            }
            internal void ParseRepeat(params Action[] actions) {
                try {
                    foreach (var action in actions) {
                        try {
                            action();
                        }
                        catch (ParseException e) {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
                catch (ParseException e) {
                    Console.WriteLine(e);
                }
            }
            
            internal void ParseNode<T>(ref T? node) where T : Parsable?, new() {
                node = new T { Context = Context } as T;
                node.Parse();
                if (node == null) throw new ParseException("Failed to parse node");
            }
        }                 
        """;
    public string ParserExceptionClass =
        """
        public class ParseException : Exception {
            public ParseException(string message) : base(message) {}
            public ParseException(string message, Exception innerException) : base(message, innerException) {}
        }
        """;
    public string ParserContextClass =
        """
        public interface IParserContext {
            Token NextToken();
            Token PeekToken(int offset = 0);
            bool MatchToken(string expectedValue);
            bool HasMoreTokens();
        }
        public class ParserContext : IParserContext {
            private readonly List<Token> _tokens;
            private int _position;
        
            public ParserContext(List<Token> tokens) {
                _tokens = tokens;
                _position = 0;
            }
        
            public Token NextToken() {
                if (!HasMoreTokens())
                    throw new ParseException("Unexpected end of token stream");
                return _tokens[_position++];
            }
        
            public Token PeekToken(int offset = 0) {
                if (_position + offset >= _tokens.Count)
                    throw new ParseException("Unexpected end of token stream while peeking");
                return _tokens[_position + offset];
            }
        
            public bool MatchToken(string expectedValue) {
                if (!HasMoreTokens()) return false;
                return PeekToken().Value == expectedValue;
            }
        
            public bool HasMoreTokens() {
                return _position < _tokens.Count;
            }
        }
        """;
    public string ParserAstNodeClass =
        """
        public class AstNode
        {
            public string Type { get; }
            public string Value { get; }
            public List<AstNode> Children { get; }
        
            public AstNode(string type, string value = null)
            {
                Type = type;
                Value = value;
                Children = new List<AstNode>();
            }
        
            public AstNode(string type, List<AstNode> children, string value = null)
            {
                Type = type;
                Children = children;
                Value = value;
            }
        
            public override string ToString()
            {
                return Value != null ? $"{Type}({Value})" : $"{Type}[{string.Join(", ", Children)}]";
            }
        }
        """;
    public string ParserTokenClass =
        """
        public class Token {
            public string Type { get; }
            public string Value { get; }
        
            public Token(string type, string value) {
                Type = type;
                Value = value;
            }
        }
        """;
>>>>>>> Stashed changes
}
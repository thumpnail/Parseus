<<<<<<< Updated upstream
﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Parseus.Parser.Common;
using Parseus.Util;
namespace Parseus.Parser.Implicit.V1;
#if false
public abstract class BaseParser {
    internal bool DEBUG = false;
    internal LogLevel LogLevel = LogLevel.none;
    internal static StreamWriter LogWriter = new StreamWriter("./log.txt");
    internal IParserContext context;
    public BaseParser() { this.context = new BasicParserContext();}
    public BaseParser(IParserContext context) {
        this.context = context;
    }
    internal static LiteralClass Literal(string literal) => new LiteralClass(literal);
    internal static TokenClass Token(string token) => new TokenClass(token);
    internal static AltClass Alt(params IParseNodeClass[] nodes) => new AltClass(nodes);
    internal static OptionalClass Opt(params IParseNodeClass[] nodes) => new OptionalClass(nodes);
    internal static RepeatClass Repeat(params IParseNodeClass[] nodes) => new RepeatClass(nodes);
    internal static NodeClass<T> Node<T>(Parser<T> IdentifierParser) where T : new() => new NodeClass<T>(IdentifierParser);
    //-----------------------------------------------------------------------
    internal interface IParseNodeClass {
        void Parse();
    }
    internal class LiteralClass : IParseNodeClass {
        private string literal;
        public LiteralClass(string literal) {
            this.literal = literal;
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class TokenClass : IParseNodeClass {
        private string token;
        public TokenClass(string token) {
            this.token = token;
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class AltClass : IParseNodeClass {
        public List<IParseNodeClass> alternatives = new();
        public AltClass(params IParseNodeClass[] nodes) {
            
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class RepeatClass : IParseNodeClass {
        public List<IParseNodeClass> childs = new();
        public RepeatClass(params IParseNodeClass[] nodes) {
            
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class OptionalClass : IParseNodeClass {
        public List<IParseNodeClass> childs = new();
        public OptionalClass(params IParseNodeClass[] nodes) {
            
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }

    internal class NodeClass<T> : IParseNodeClass where T : new() {
        private readonly Parser<T> refParser;
        public NodeClass(Parser<T> refParser) {
            this.refParser = refParser;
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    //-----------------------------------------------------------------------
    internal class Parser<T> where T : new() {
        // add a fild that returns the default get from this class so it returns the T value
        public List<IParseNodeClass> Nodes;
        public T self { get; private set; }
        public Parser(params IParseNodeClass[] nodes) {
            this.Nodes = nodes.ToList();
        }
    
        public T Parse(IParserContext context) {
            self = new();
            //Nodes(context, self);
            //parse and get the Ast Type
            return self;
        }
    }
}
=======
﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Parseus.Parser.Common;
using Parseus.Util;
namespace Parseus.Parser.Implicit.V1;
#if false
public abstract class BaseParser {
    internal bool DEBUG = false;
    internal LogLevel LogLevel = LogLevel.none;
    internal static StreamWriter LogWriter = new StreamWriter("./log.txt");
    internal IParserContext context;
    public BaseParser() { this.context = new BasicParserContext();}
    public BaseParser(IParserContext context) {
        this.context = context;
    }
    internal static LiteralClass Literal(string literal) => new LiteralClass(literal);
    internal static TokenClass Token(string token) => new TokenClass(token);
    internal static AltClass Alt(params IParseNodeClass[] nodes) => new AltClass(nodes);
    internal static OptionalClass Opt(params IParseNodeClass[] nodes) => new OptionalClass(nodes);
    internal static RepeatClass Repeat(params IParseNodeClass[] nodes) => new RepeatClass(nodes);
    internal static NodeClass<T> Node<T>(Parser<T> IdentifierParser) where T : new() => new NodeClass<T>(IdentifierParser);
    //-----------------------------------------------------------------------
    internal interface IParseNodeClass {
        void Parse();
    }
    internal class LiteralClass : IParseNodeClass {
        private string literal;
        public LiteralClass(string literal) {
            this.literal = literal;
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class TokenClass : IParseNodeClass {
        private string token;
        public TokenClass(string token) {
            this.token = token;
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class AltClass : IParseNodeClass {
        public List<IParseNodeClass> alternatives = new();
        public AltClass(params IParseNodeClass[] nodes) {
            
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class RepeatClass : IParseNodeClass {
        public List<IParseNodeClass> childs = new();
        public RepeatClass(params IParseNodeClass[] nodes) {
            
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    internal class OptionalClass : IParseNodeClass {
        public List<IParseNodeClass> childs = new();
        public OptionalClass(params IParseNodeClass[] nodes) {
            
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }

    internal class NodeClass<T> : IParseNodeClass where T : new() {
        private readonly Parser<T> refParser;
        public NodeClass(Parser<T> refParser) {
            this.refParser = refParser;
        }
        public void Parse() {
            throw new NotImplementedException();
        }
    }
    //-----------------------------------------------------------------------
    internal class Parser<T> where T : new() {
        // add a fild that returns the default get from this class so it returns the T value
        public List<IParseNodeClass> Nodes;
        public T self { get; private set; }
        public Parser(params IParseNodeClass[] nodes) {
            this.Nodes = nodes.ToList();
        }
    
        public T Parse(IParserContext context) {
            self = new();
            //Nodes(context, self);
            //parse and get the Ast Type
            return self;
        }
    }
}
>>>>>>> Stashed changes
#endif
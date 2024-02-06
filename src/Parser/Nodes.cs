using System.Globalization;
using Token = System.Int32;
using InternalType = System.Int32;

namespace Parseus.Parser;
public static partial class ParserModule {
    const byte INTERNALTYPE_UNKOWN = 0, INTERNALTYPE_LITERAL = 1, INTERNALTYPE_REGEXEXPRESSION = 2;

    public interface IRuleChild {
        public void Parse(AbstractSyntaxTree ctx);
    }

    public struct Rule_t : IRuleChild {
        string name;
        Token nodeType;
        Alt_t[] childs;
        public Rule_t(string name, int tk, Alt_t[] elements) {
            this.name = name;
            this.nodeType = tk;
        }
        public void Parse(AbstractSyntaxTree ctx) {
            foreach (var item in childs) {
                //TODO: just Parse Alt node
                item.Parse(ctx);
            }
        }
    }
    public static Rule_t Rule(string name, Token NodeType, params Alt_t[] elements) { return new Rule_t(name, NodeType, elements); }
    public static Rule_t Rule(string name, params Alt_t[] elements) { return new Rule_t(name, -1, elements); }

    public struct Alt_t : IRuleChild {
        Group_t[] childs;
        public Alt_t(Group_t[] groups) { this.childs = childs; }
        public void Parse(AbstractSyntaxTree ctx) {
            foreach (var item in childs) {
                //TODO: check inside each Group node if it is parseable, then parse group
                item.Parse(ctx);
            }
        }
    }
    public static Alt_t Alt(params Group_t[] groups) { return new Alt_t(groups); }

    public struct Group_t : IRuleChild {
        IRuleChild[] childs;
        public Group_t(IRuleChild[] childs) { this.childs = childs; }
        public void Parse(AbstractSyntaxTree ctx) {
            foreach (var item in childs) {
                //TODO: parse each element, prob need another function to check if it is parseable for alts
                item.Parse(ctx);
            }
        }
    }
    public static Group_t Group(params IRuleChild[] childs) { return new Group_t(childs); }

    public struct Opt_t : IRuleChild {
        IRuleChild[] childs;
        public Opt_t(IRuleChild[] childs) { this.childs = childs; }
        public void Parse(AbstractSyntaxTree ctx) {
            foreach (var item in childs) {
                //TODO: check if it parses, if not ignore, if parsable, parse it
                item.Parse(ctx);
            }
        }
    }
    public static Opt_t Opt(params IRuleChild[] childs) { return new Opt_t(childs); }

    public struct Repeat_t : IRuleChild {
        IRuleChild[] childs;
        public Repeat_t(IRuleChild[] childs) { this.childs = childs; }
        public void Parse(AbstractSyntaxTree ctx) {
            foreach (var item in childs) {
                //TODO: check if parseable, if so, parse it, if it repeats, parse the repeat
                //TODO: if nothing fits just ignore, we need information for the position after the repeat node, to overtake
                item.Parse(ctx);
            }
        }
    }
    public static Repeat_t Repeat(params IRuleChild[] childs) { return new Repeat_t(childs); }

    public struct Literal_t : IRuleChild {
        InternalType type;
        Token token;
        string value;
        public Literal_t(int tk, string value, InternalType type) {
            this.token = tk;
            this.value = value;
            this.type = type;
        }
        public void Parse(AbstractSyntaxTree ctx) {
            //TODO: Parse the literal, or token and add information to the Current Working Node
        }
    }
    public static Literal_t Lit(Token tk, string value) { return new Literal_t(tk, value, INTERNALTYPE_LITERAL); }
    public static Literal_t Lit(string value) { return new Literal_t(-1, value, INTERNALTYPE_LITERAL); }
    public static Literal_t Lit(Token tk) { return new Literal_t(tk, null, INTERNALTYPE_LITERAL); }
    public static Literal_t RExp(Token tk, string value) { return new Literal_t(tk, value, INTERNALTYPE_REGEXEXPRESSION); }

    public struct RefRule_t : IRuleChild {
        InternalType it;
        string value;
        public RefRule_t(string value) { this.value = value; }
        public void Parse(AbstractSyntaxTree ctx) {
            //TODO: Search for the Rule and try parse it
        }
    }
    public static RefRule_t RefRule(string value) { return new RefRule_t(value); }
}
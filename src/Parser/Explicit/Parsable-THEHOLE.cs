using Parseus.Parser.Common;
namespace Parseus.Parser.Explicit;

public abstract class Parsable {
    protected AParserContext Context;
    public Parsable() {
        throw new NotImplementedException();
        Context = new BasicAParserContext();
    }
    public Parsable(AParserContext context) {
        throw new NotImplementedException();
        Context = context;
    }

    public abstract Parsable? Parse();

    internal void ParseAlt(params Action[] actions) {
        throw new NotImplementedException();
    }

    internal void ParseLiteral(string literal, out bool matched) {
        throw new NotImplementedException();
    }


    internal void ParseToken(string tokenType, out string value) {
        throw new NotImplementedException();
    }

    internal void ParseOpt(Action action) {
        throw new NotImplementedException();
    }
    internal void ParseRepeat(params Action[] actions) {
        throw new NotImplementedException();
    }
    //parse subnodes
    internal void ParseNode<T>(ref T? node) where T : Parsable?, new() {
        throw new NotImplementedException();
    }
}
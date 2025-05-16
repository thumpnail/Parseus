<<<<<<< Updated upstream
﻿using Parseus.Parser.Common;
namespace Parseus.Parser.Explicit;

public abstract class Parsable {
    protected IParserContext Context;
    public Parsable() {
        throw new NotImplementedException();
        Context = new BasicParserContext();
    }
    public Parsable(IParserContext context) {
        throw new NotImplementedException();
        Context = context;
    }

    public abstract Parsable? Parse();

    internal void ParseAlt(params Action[] actions) {
        foreach (var action in actions) {
            try {
                action();
                return;
            }
            catch (ParseException) {
                // Ignore exception and try the next alternative
            }
        }
        throw new ParseException("Failed to parse alternative options", $"{typeof(Parsable)}.ParseAlt");
    }

    internal void ParseLiteral(string literal, out bool matched) {
        if (Context.MatchToken(x=>x.Value == literal)) {
            Context.Consume();
            matched = true;
        } else {
            matched = false;
            throw new ParseException($"Expected '{literal}' {new Func<string>(() => {
                try {
                    return $"but got '{Context.PeekToken().Value}' of type '{Context.PeekToken().Type}'";
                } catch (ParseException e) {}
                return "";
            }).Invoke()}", $"{typeof(Parsable)}.ParseLiteral");
        }
    }


    internal void ParseToken(string tokenType, out string value) {
        if (!Context.HasMoreTokens()) throw new ParseException($"Expected token of type {tokenType}, but got end of input",$"{typeof(Parsable)}.ParseToken");
        var token = Context.Consume();
        if (token.Type != tokenType) throw new ParseException($"Expected token of type {tokenType}, but got {token.Type}",$"{typeof(Parsable)}.ParseToken");
        value = token.Value;
    }

    internal void ParseOpt(Action action) {
        try {
            action();
        }
        catch (ParseException) {
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
                    throw new ParseException("Failed to parse repeat options",$"{typeof(Parsable)}.ParseRepeat");
                }
            }
        }
        catch (ParseException e) {
            Console.WriteLine(e);
        }
    }
    //parse subnodes
    internal void ParseNode<T>(ref T? node) where T : Parsable?, new() {
        node = new T { Context = Context } as T;
        node.Parse();
        if (node == null) throw new ParseException("Failed to parse node",$"{typeof(Parsable)}.ParseNode");
    }
=======
﻿using Parseus.Parser.Common;
namespace Parseus.Parser.Explicit;

public abstract class Parsable {
    protected IParserContext Context;
    public Parsable() {
        throw new NotImplementedException();
        Context = new BasicParserContext();
    }
    public Parsable(IParserContext context) {
        throw new NotImplementedException();
        Context = context;
    }

    public abstract Parsable? Parse();

    internal void ParseAlt(params Action[] actions) {
        foreach (var action in actions) {
            try {
                action();
                return;
            }
            catch (ParseException) {
                // Ignore exception and try the next alternative
            }
        }
        throw new ParseException("Failed to parse alternative options", $"{typeof(Parsable)}.ParseAlt");
    }

    internal void ParseLiteral(string literal, out bool matched) {
        if (Context.MatchToken(x=>x.Value == literal)) {
            Context.Consume();
            matched = true;
        } else {
            matched = false;
            throw new ParseException($"Expected '{literal}' {new Func<string>(() => {
                try {
                    return $"but got '{Context.PeekToken().Value}' of type '{Context.PeekToken().Type}'";
                } catch (ParseException e) {}
                return "";
            }).Invoke()}", $"{typeof(Parsable)}.ParseLiteral");
        }
    }


    internal void ParseToken(string tokenType, out string value) {
        if (!Context.HasMoreTokens()) throw new ParseException($"Expected token of type {tokenType}, but got end of input",$"{typeof(Parsable)}.ParseToken");
        var token = Context.Consume();
        if (token.Type != tokenType) throw new ParseException($"Expected token of type {tokenType}, but got {token.Type}",$"{typeof(Parsable)}.ParseToken");
        value = token.Value;
    }

    internal void ParseOpt(Action action) {
        try {
            action();
        }
        catch (ParseException) {
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
                    throw new ParseException("Failed to parse repeat options",$"{typeof(Parsable)}.ParseRepeat");
                }
            }
        }
        catch (ParseException e) {
            Console.WriteLine(e);
        }
    }
    //parse subnodes
    internal void ParseNode<T>(ref T? node) where T : Parsable?, new() {
        node = new T { Context = Context } as T;
        node.Parse();
        if (node == null) throw new ParseException("Failed to parse node",$"{typeof(Parsable)}.ParseNode");
    }
>>>>>>> Stashed changes
}
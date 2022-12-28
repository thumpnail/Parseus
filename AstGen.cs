using Microsoft.VisualBasic.CompilerServices;
using L = parser.Lexer;

namespace parser;
public struct AstContext {
    public Chunk head;
    public List<Tuple<Lexer.TokenType, string>> tokens;
    public Queue<Tuple<Lexer.TokenType, string>> qtokens;
    public int cidx;

    public AstContext(List<Tuple<Lexer.TokenType, string>> tokens) {
        cidx = 0;
        this.tokens = tokens;
        head = new();
    }
    public Tuple<Lexer.TokenType, string> Current {
        get { return tokens[cidx]; }
    }
    public Tuple<Lexer.TokenType, string> Peek {
        get { return tokens[cidx + 1]; }
    }
    public Tuple<Lexer.TokenType, string> Pop {
        get { return tokens[cidx++]; }
    }
    public bool Incr {
        get {
            if (cidx + 1 >= tokens.Count) {
                return false;
            }
            cidx++;
            return true;
        }
    }
}
public static class AstGen {
    public static Chunk GenerateAst(List<Tuple<Lexer.TokenType, string>> tokens) {
        AstContext ctx = new(tokens);
        //Assign Statement
        while(ctx.cidx < ctx.tokens.Count) {
            if (ctx.Current.Item1 == L.TokenType.k_identifier && ctx.Peek.Item1 == L.TokenType.o_adveq) {
                DiscoverAssign(ref ctx);
            }
            if (ctx.Current.Item1 == L.TokenType.EOL)
                ctx.cidx++;
            if (ctx.Current.Item1 == L.TokenType.EOF)
                break;
        }
        return ctx.head;
    }
    // Assign statement
    public static void DiscoverAssign(ref AstContext ctx) {
        string varname = ctx.Pop.Item2;
        ctx.cidx++;
        ctx.head.Add(new AssignStat(varname, DiscoverExpressions(ref ctx).Item2));
    }

    public static (AstContext, IExpression) DiscoverExpressions(ref AstContext ctx) {
        var operandStack = new Stack<IExpression>();
        var operatorStack = new Stack<Operator>();

        var tokens = new List<Tuple<Lexer.TokenType, string>>();
        for (ctx.cidx = ctx.cidx; ctx.tokens[ctx.cidx].Item1 != L.TokenType.EOL; ctx.cidx++) { // TODO: not all expressions are ended with EOL
            tokens.Add(ctx.tokens[ctx.cidx]);
        }

        foreach (var token in tokens) {
            if (double.TryParse(token.Item2, out var operand)) {
                operandStack.Push(new Value().setValue(operand));
            } else if (token.Item1 == L.TokenType.t_string) {
                operandStack.Push(new Value().setValue(token.Item2));
            } else if (token.Item1 == L.TokenType.k_identifier) {
                operandStack.Push(new VarExp(token.Item2));
            } else if (Ast.getOp(token.Item2) != Operator.nil) { // TODO: usage of operator dictonary in lexer
                operatorStack.Push(Ast.getOp(token.Item2));
            } else if (token.Item2 == "(") {
                operatorStack.Push(Operator.ORB); // TODO: usage of tokentypes?
            } else if (token.Item2 == ")") {
                while (operatorStack.Peek() != Operator.ORB) {
                    var rightOperand = operandStack.Pop();
                    var leftOperand = operandStack.Pop();
                    var op = operatorStack.Pop();
                    operandStack.Push(ApplyOperation(leftOperand, rightOperand, op, true));
                }
                operatorStack.Pop();
            }
        }

        while (operatorStack.Count > 0) {
            var rightOperand = operandStack.Pop();
            var leftOperand = operandStack.Pop();
            var op = operatorStack.Pop();
            operandStack.Push(ApplyOperation(leftOperand, rightOperand, op));
        }

        return (ctx, operandStack.Pop());
    }

    public static IExpression ApplyOperation(IExpression leftOperand, IExpression rightOperand, Operator op, bool brackets = false) {
        switch (op) {
            case Operator.ADD:
            case Operator.SUB:
                if(brackets)
                    return new MathExp(leftOperand, op, rightOperand).SetParenties(ParenType.round);
                else
                    return new MathExp(leftOperand, op, rightOperand);
            case Operator.MUL:
            case Operator.DIV:
                if (brackets)
                    return new MathExp(leftOperand, op, rightOperand).SetParenties(ParenType.round);
                else
                    return new MathExp(leftOperand, op, rightOperand);
            case Operator.AND:
            case Operator.OR:
            case Operator.BIG:
            case Operator.BIGEQL:
            case Operator.SML:
            case Operator.SMLEQL:
            case Operator.NOTEQL:
            case Operator.EQL:
                if (brackets)
                    return new LogicExp(leftOperand, op, rightOperand).SetParenties(ParenType.round);
                else
                    return new LogicExp(leftOperand, op, rightOperand);
            case Operator.INCR:
            case Operator.DECR:
                throw new NotImplementedException();
            case Operator.LSHIFT:
            case Operator.RSHIFT:
                throw new NotImplementedException();
            default:
                throw new ArgumentException("Invalid operator: " + op);
        }
    }
}
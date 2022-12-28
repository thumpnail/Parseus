using System;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.CodeDom;
using System.Linq;
using System.Security.Principal;

namespace parser;
public enum Operator : byte {
    ADD, // +
    SUB, // -
    MUL, // *
    DIV, // /
    AND, // &&
    OR, // ||

    SML, // <
    SMLEQL, // <=
    BIG, // >
    BIGEQL, // >=
    EQL, // ==
    NOTEQL, // !=
    RSHIFT, // >>
    LSHIFT, // <<
    INCR, // ++
    DECR, // --

    ORB, // (
    CRB, // )

    nil
}
public enum Bit_op {
    shift_left, // <<
    shift_right,// >>
}
public enum VType : uint {
    @null,
    number,
    @true,
    @false,
    @string,
    @table,
    @array
}
public enum ParenType {
    round,
    box,
    brackets
}
public static class Ast {
    public static string getOp(Operator op) {
        switch (op) {
            case Operator.ADD:
                return "+";
            case Operator.SUB:
                return "-";
            case Operator.MUL:
                return "*";
            case Operator.DIV:
                return "/";
            case Operator.AND:
                return "&&";
            case Operator.OR:
                return "||";
            case Operator.SML:
                return "<";
            case Operator.SMLEQL:
                return "<=";
            case Operator.BIG:
                return ">";
            case Operator.BIGEQL:
                return ">=";
            case Operator.EQL:
                return "==";
            case Operator.NOTEQL:
                return "!=";
            case Operator.LSHIFT:
                return "<<";
            case Operator.RSHIFT:
                return ">>";
            default:
                return "-NI-";
        }
    }
    public static Operator getOp(string op) {
        switch (op) {
            case "+":
                return Operator.ADD;
            case "-":
                return Operator.SUB;
            case "*":
                return Operator.MUL;
            case "/":
                return Operator.DIV;
            case "&&":
            case "&":
                return Operator.AND;
            case "||":
            case "|":
                return Operator.OR;
            case "<":
                return Operator.SML;
            case "<=":
                return Operator.SMLEQL;
            case ">":
                return Operator.BIG;
            case ">=":
                return Operator.BIGEQL;
            case "==":
                return Operator.EQL;
            case "!=":
                return Operator.NOTEQL;
            case "<<":
                return Operator.LSHIFT;
            case ">>":
                return Operator.RSHIFT;
            default:
                return Operator.nil;
        }
    }
}
public interface INode {
    public string GenerateSrcCode();
    public List<int> GenerateByteCode();
}
public interface IExpression : INode {
    public static IExpression operator +(IExpression exp1, IExpression exp2) {
        return new MathExp(exp1, Operator.ADD, exp2);
    }
}
public interface IStatement : INode { }

[StructLayout(LayoutKind.Explicit)]
public struct Value : IExpression {
    [FieldOffset(4)] VType type;
    [FieldOffset(0)] double @double;
    [FieldOffset(8)] Dictionary<Value, Value> @table;
    [FieldOffset(8)] List<Value> @array;
    [FieldOffset(8)] string @string;
    public Value setValue() {
        type = VType.@null;
        return this;
    }
    public Value setValue(double @double) {
        type = VType.number;
        this.@double = @double;
        return this;
    }
    public Value setValue(string @string) {
        this.@string = @string;
        type = VType.@string;
        return this;
    }
    public Value setValue(Dictionary<Value, Value> @table) {
        this.@table = @table;
        type = VType.@table;
        return this;
    }
    public Value setValue(List<Value> @array) {
        this.@array = @array;
        type = VType.@array;
        return this;
    }
    public string GenerateSrcCode() {
        switch (type) {
            case VType.number:
                return $"{@double}";
            case VType.@string:
                return $"\"{@string}\"";
            case VType.@table:
                return $"{@table.ToString()}";
            case VType.@array:
                return $"{@array.ToString()}";
            case VType.@true:
                return $"{true}";
            case VType.@false:
                return $"{false}";
            case VType.@null:
                return "null";
            default:
                if (!Double.IsNaN(@double))
                    return $"{@double}";
                return "--error--";
        }
    }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct VarExp : IExpression {
    string varname = "";
    public VarExp(string varname) { this.varname = varname; }
    public string GenerateSrcCode() { return $"{varname}"; }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct Exp : IExpression {
    string varname;
    public Exp(string varname) {
        this.varname = varname;
    }
    public string GenerateSrcCode() { return ""; }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct MathExp : IExpression {
    (IExpression, IExpression) vals;
    char? left;
    char? right;
    Operator math_op;
    public MathExp(IExpression v1, Operator op, IExpression v2) {
        vals = new(v1, v2);
        math_op = op;
        left = null;
        right = null;
    }
    public MathExp SetParenties(ParenType parentype) {
        switch (parentype) {
            case ParenType.round:
                left = '(';
                right = ')';
                break;
            case ParenType.box:
                left = '[';
                right = ']';
                break;
            case ParenType.brackets:
                left = '{';
                right = '}';
                break;
            default:
                left = null;
                right = null;
                break;
        }
        return this;
    }
    public string GenerateSrcCode() { return $"{left}{vals.Item1.GenerateSrcCode()} {Ast.getOp(math_op)} {vals.Item2.GenerateSrcCode()}{right}"; }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct LogicExp : IExpression {
    (IExpression, IExpression) vals;
    Operator logic_op;
    char? left;
    char? right;
    public LogicExp(IExpression v1, Operator op, IExpression v2) {
        vals = new(v1, v2);
        logic_op = op;
        left = null;
        right = null;
    }
    public LogicExp SetParenties(ParenType parentype) {
        switch (parentype) {
            case ParenType.round:
                left = '(';
                right = ')';
                break;
            case ParenType.box:
                left = '[';
                right = ']';
                break;
            case ParenType.brackets:
                left = '{';
                right = '}';
                break;
            default:
                left = null;
                right = null;
                break;
        }
        return this;
    }
    public string GenerateSrcCode() { return $"{left}{vals.Item1.GenerateSrcCode()} {Ast.getOp(logic_op)} {vals.Item2.GenerateSrcCode()}{right}"; }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct TypeExp : IExpression {
    string typename = "";
    public TypeExp(string typename) {
        this.typename = typename;
    }
    public string GenerateSrcCode() { return $": {typename}"; }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct SemiStat : IStatement {
    public SemiStat() { }
    public string GenerateSrcCode() { return ";\n"; }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct AssignStat : IStatement {
    //let <var> : <typeexp> = <exp <<value * value> + value>
    string varname;
    TypeExp? type;
    IExpression exp;
    public AssignStat(string varname, IExpression expression) {
        this.varname = varname;
        this.exp = expression;
        this.type = null;
    }
    public AssignStat(string varname, TypeExp typeinfo, IExpression expression) {
        this.type = typeinfo;
        this.varname = varname;
        this.exp = expression;
    }
    public string GenerateSrcCode() { return $"{varname}{type?.GenerateSrcCode() + " "}:= {exp.GenerateSrcCode()};\n"; }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
//funcDecl = "fnc" Name "(" { paramList } ")" { ":"  type } { block };
public struct FunctionStatement : IStatement {
    string functionName;
    List<IExpression> ParameterList;
    List<IStatement> Body;
    public string GenerateSrcCode() {
        string paratmp = "";
        foreach (var item in ParameterList) {
            paratmp += item.GenerateSrcCode();
        }
        string bodytemp = "";
        foreach (var item in Body) {
            bodytemp += item.GenerateSrcCode();
        }
        return $"fnc {functionName}({paratmp}) {'{'}\n{bodytemp}\n{'}'}";
    }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
public struct Chunk : IStatement {
    List<IStatement> statements;
    public Chunk(List<IStatement> statements) {
        if (statements is null)
            this.statements = new List<IStatement>();
        else
            this.statements = statements;
    }
    public Chunk() {
        this.statements = new List<IStatement>();
    }
    public void Add(IStatement statement) { statements.Add(statement); }
    public string GenerateSrcCode() {
        string tmp = "";
        foreach (var i in statements)
            tmp += i.GenerateSrcCode();
        return $"{tmp}";
    }
    public List<int> GenerateByteCode() { return new List<int>(); }
}
using System.Runtime.CompilerServices;
using Parseus.Parser.Common;
using Parseus.Util;
namespace Parseus;

public struct Result<T> {
    public T? Value;
    public bool Success;
    public Error? error;
    public Result(T? value) {
        this.Value = value;
        this.Success = true;
        this.error = null;
    }
    public Result(string errorMsg) {
        this.error = new Error(errorMsg);
        this.Success = false;
    }
    public T GetValue() {
        return Value ?? ThrowError();
    }
    public void GetValue(out T value) {
        value = Value ?? ThrowError();
    }
    public void GetValue(Action<T> action) {
        action(Value ?? ThrowError());
    }
    public void SetValue(T? value) {
        this.Value = value;
        this.Success = value is not null;
    }
    public void SetValue(T value, Action<T> action) {
        action(value);
    }
    public T ThrowError() {
        error?.Throw();
        return default;
    }
}

public class Result {
    public object? Value;
    public Type? type;
    public bool Success;
    public Error? error;
    public Result() {}
    public Result(object value) {
        this.Value = value;
        this.type = value.GetType();
        this.Success = true;
    }
    public Result(string errorMsg) {
        this.error = new Error(errorMsg);
        this.Success = false;
    }
    public static Result<T> AsGeneric<T>(Result result) where T : new() {
        if (typeof(T) != result.type) {
            throw new Exception("Type missmatch");
        }
        return new() {
            error = result.error,
            Success = result.Success,
            Value = (T)result.Value
        };
    }
    public static Result FromGeneric<T>(Result<T> result) where T : new() {
        return new Result {
            error = result.error,
            type = typeof(T),
            Success = result.Success,
            Value = result.Value
        };
    }
    public object GetValue() {
        return Value;
    }
    public void GetValue(out (object value,Type type) value) {
        value = (Value,type);
    }
    public void GetValue(Action<object> action) {
        action(Value);
    }
    public void SetValue(object? value) {
        this.Value = value;
        this.type = value.GetType();
        this.Success = value is not null;
    }
    public void SetValue(object value, Action<object> action) {
        action(value);
    }
    public void ThrowError() {
        error?.Throw();
    }
}

public class ResultExt {
    public static bool MCheck(params bool[] input) {
        var result = true;
        foreach (var chkvalue in input) {
            if (!chkvalue) {
                return false;
            }
        }
        return result;
    }
}

public struct Error {
    private Queue<string> errors = new();
    private string msg;
    public Error(string msg, LogLevel level = LogLevel.none, string origin = "unkown") {
        this.msg = msg;
        errors.Enqueue(LogFormater.FormatLog(msg, $"{origin}", level));
    }
    public void Throw(string origin = "unkown") {
        var exept = new ParseException(msg, $"{origin}.Throw") {
            Data = {
                ["Message"] = msg
            }
        };
        throw exept;
    }
}

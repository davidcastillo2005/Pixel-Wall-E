using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PixelWallE.Parser.src;

public class Result : IParsable<Result>
{
    object? Value { get; set; }
    Type Type { get; set; }

    private Result(object? value)
    {
        Value = value;
        Type = value?.GetType() ?? typeof(object);
    }

    private Result(object? value, Type type)
    {
        Value = value;
        Type = type;
    }

    // private static Result ExecuteOp(string op, object?[] values, Type[] types)
    // {
    //     var flags = BindingFlags.Public | BindingFlags.Static;
    //     MethodInfo? method = types
    //         .SelectMany(m => m.GetMethods(flags))
    //         .FirstOrDefault(m =>
    //             m.Name == op &&
    //             m.GetParameters()
    //                 .Select(p => p.ParameterType)
    //                 .SequenceEqual(types)
    //         );
    //     var result = method!.Invoke(null, values);
    //     return new Result(result, method.ReturnType);
    // }

    // public static Result ExecuteOp(string op, params Result[] values)
    //     => ExecuteOp(op, [.. values.Select(x => x.Value)], [.. values.Select(x => x.Type)]);

    public static Result operator +(Result a, Result b) => new ((dynamic)a.Value! + (dynamic)b.Value!);
    public static Result operator -(Result a, Result b) => new ((dynamic)a.Value! - (dynamic)b.Value!);
    public static Result operator *(Result a, Result b) => new ((dynamic)a.Value! * (dynamic)b.Value!);
    public static Result operator /(Result a, Result b) => new ((dynamic)a.Value! / (dynamic)b.Value!);
    public static Result operator %(Result a, Result b) => new ((dynamic)a.Value! % (dynamic)b.Value!);
    public static Result operator &(Result a, Result b) => new ((dynamic)a.Value! & (dynamic)b.Value!);
    public static Result operator |(Result a, Result b) => new ((dynamic)a.Value! | (dynamic)b.Value!);
    public static Result operator ^(Result a, Result b)
    {
        if (a.Value is double castedA && b.Value is double castedB)
            return new Result(Math.Pow(castedA, castedB), a.Type);
        throw new Exception();
    }
    public static Result operator ==(Result a, Result b) => new ((dynamic)a.Value! == (dynamic)b.Value!);
    public static Result operator !=(Result a, Result b) => new ((dynamic)a.Value! != (dynamic)b.Value!);
    public static Result operator <(Result a, Result b) => new ((dynamic)a.Value! < (dynamic)b.Value!);
    public static Result operator <=(Result a, Result b) => new ((dynamic)a.Value! <= (dynamic)b.Value!);
    public static Result operator >(Result a, Result b) => new ((dynamic)a.Value! > (dynamic)b.Value!);
    public static Result operator >=(Result a, Result b) => new ((dynamic)a.Value! >= (dynamic)b.Value!);
    public static Result operator -(Result a) => new (-(dynamic)a.Value!);
    public static Result operator !(Result a) => new (!(dynamic)a.Value!);

    public override bool Equals(object? obj)
        => ReferenceEquals(obj, this)
        && obj is Result value
        && Value!.Equals(value.Value);

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : Type.GetHashCode();

    public static Result Parse(string s, IFormatProvider? provider)
    {
        if (int.TryParse(s, provider, out int num))
            return new Result(num);
        if (bool.TryParse(s, out bool b))
            return new Result(b);
        return new Result(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Result result)
    {
        if (int.TryParse(s, provider, out int num))
            result = new Result(num);
        else if (bool.TryParse(s, out bool b))
            result = new Result(b);
        else
            result = new Result(s);
        return true;
    }
}
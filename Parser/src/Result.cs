using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PixelWallE.Parser.src;

public class Result : IParsable<Result>
{
    public object? Value { get; set; }
    public Type Type { get; set; }

    public Result(object? value)
    {
        Value = value;
        Type = value?.GetType() ?? typeof(object);
    }

    public Result(object? value, Type type)
    {
        Value = value;
        Type = type;
    }

    public bool ToBoolean()
    {
        if (Value is not null && Value is bool bValue)
            return bValue;
        throw new Exception();
    }

    public int ToInterger()
    {
        if (Value is not null && Value is int iValue)
            return iValue;
        throw new Exception();
    }

    public override string ToString()
    {
        if (Value is not null && Value is string sValue)
            return sValue;
        throw new Exception();
    }

    public static Result operator +(Result a, Result b) => new((dynamic)a.Value! + (dynamic)b.Value!);
    public static Result operator -(Result a, Result b) => new((dynamic)a.Value! - (dynamic)b.Value!);
    public static Result operator *(Result a, Result b) => new((dynamic)a.Value! * (dynamic)b.Value!);
    public static Result operator /(Result a, Result b) => new((dynamic)a.Value! / (dynamic)b.Value!);
    public static Result operator %(Result a, Result b) => new((dynamic)a.Value! % (dynamic)b.Value!);
    public static Result operator &(Result a, Result b) => new((dynamic)a.Value! & (dynamic)b.Value!);
    public static Result operator |(Result a, Result b) => new((dynamic)a.Value! | (dynamic)b.Value!);
    public static Result operator ^(Result a, Result b)
    {
        if (a.Value is double castedA && b.Value is double castedB)
            return new Result(Math.Pow(castedA, castedB), a.Type);
        throw new Exception();
    }
    public static Result operator ==(Result a, Result b) => new((dynamic)a.Value! == (dynamic)b.Value!);
    public static Result operator !=(Result a, Result b) => new((dynamic)a.Value! != (dynamic)b.Value!);
    public static Result operator <(Result a, Result b) => new((dynamic)a.Value! < (dynamic)b.Value!);
    public static Result operator <=(Result a, Result b) => new((dynamic)a.Value! <= (dynamic)b.Value!);
    public static Result operator >(Result a, Result b) => new((dynamic)a.Value! > (dynamic)b.Value!);
    public static Result operator >=(Result a, Result b) => new((dynamic)a.Value! >= (dynamic)b.Value!);
    public static Result operator -(Result a) => new(-(dynamic)a.Value!);
    public static Result operator !(Result a) => new(!(dynamic)a.Value!);

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
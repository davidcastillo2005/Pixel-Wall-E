using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PixelWallE.Parser.src;

public class ValueType(object? value, Type type) : IParsable<ValueType>
{
    object? Value { get; set; } = value;
    Type Type { get; set; } = type;

    private static ValueType ExecuteOp(string op, object?[] values, Type[] types)
    {
        MethodInfo? method = types
            .SelectMany(m => m.GetMethods())
            .FirstOrDefault(m =>
                m.IsStatic &&
                m.Name == op &&
                m.GetParameters()
                    .Select(p => p.ParameterType)
                    .SequenceEqual(types)
            );
        var result = method!.Invoke(null, values);
        return new ValueType(result, method.ReturnType);
    }

    public static ValueType ExecuteOp(string op, params ValueType[] values)
        => ExecuteOp(op, [.. values.Select(x => x.Value)], [.. values.Select(x => x.Type)]);

    public static ValueType operator +(ValueType a, ValueType b)
    {
        return ExecuteOp("op_Addition", a, b);
    }
    public static ValueType operator -(ValueType a, ValueType b)
    {
        return ExecuteOp("op_Subtraction", a, b);
    }
    public static ValueType operator *(ValueType a, ValueType b)
    {
        return ExecuteOp("op_Multiply", a, b);
    }
    public static ValueType operator /(ValueType a, ValueType b)
    {
        return ExecuteOp("op_Division", a, b);
    }
    public static ValueType operator %(ValueType a, ValueType b)
    {
        return ExecuteOp("op_Modulus", a, b);
    }
    public static ValueType operator &(ValueType a, ValueType b)
    {
        return ExecuteOp("op_BitwiseAnd", a, b);
    }
    public static ValueType operator |(ValueType a, ValueType b)
    {
        return ExecuteOp("op_BitwiseOr", a, b);
    }
    public static ValueType operator ^(ValueType a, ValueType b)
    {
        if (a.Value is double castedA && b.Value is double castedB)
        {
            return new ValueType(Math.Pow(castedA, castedB), a.Type);
        }
        throw new Exception();
    }
    public static ValueType operator ==(ValueType a, ValueType b)
    {
        return ExecuteOp("op_Equality", a, b);
    }
    public static ValueType operator !=(ValueType a, ValueType b)
    {
        return ExecuteOp("op_Inequality", a, b);
    }
    public static ValueType operator <(ValueType a, ValueType b)
    {
        return ExecuteOp("op_LessThan", a, b);
    }
    public static ValueType operator <=(ValueType a, ValueType b)
    {
        return ExecuteOp("op_LessThanOrEqual", a, b);
    }
    public static ValueType operator >(ValueType a, ValueType b)
    {
        return ExecuteOp("op_GreaterThan", a, b);
    }
    public static ValueType operator >=(ValueType a, ValueType b)
    {
        return ExecuteOp("op_GreaterThanOrEqual", a, b);
    }
    
    public static ValueType operator -(ValueType a) => ExecuteOp("op_Subtraction", a);

    public static ValueType operator !(ValueType a) => ExecuteOp("op_UnaryNegation", a);

    public override bool Equals(object? obj)
        => ReferenceEquals(obj, this)
        && obj is ValueType value
        && Value!.Equals(value.Value);

    public override int GetHashCode() => Value != null ? Value.GetHashCode() : Type.GetHashCode();

    public static ValueType Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ValueType result)
    {
        throw new NotImplementedException();
    }
}
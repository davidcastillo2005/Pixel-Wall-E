using PixelWallE.Parser.src.Enums;
using Type = PixelWallE.Lexer.src.Type;

namespace PixelWallE.Parser.src.Extensions;

public static class OperatorExtension
{
    public static BinaryType ToBinaryType(this Type type) => type switch
    {
        Type.Addition => BinaryType.Add,
        Type.Subtraction => BinaryType.Subtract,
        Type.Multiplication => BinaryType.Multiply,
        Type.Division => BinaryType.Divide,
        Type.Exponentiation => BinaryType.Power,
        Type.Remainder => BinaryType.Modulus,
        Type.InclusiveLess => BinaryType.LessOrEqualThan,
        Type.InclusiveGreater => BinaryType.GreaterOrEqualThan,
        Type.ExclusiveLess => BinaryType.LessThan,
        Type.ExclusiveGreater => BinaryType.GreaterThan,
        Type.Equal => BinaryType.Equal,
        Type.NotEqual => BinaryType.NotEqual,
        Type.And => BinaryType.And,
        Type.Or => BinaryType.Or,
        _ => throw new Exception(),
    };

    public static UnaryType ToUnaryType(this Type type) => type switch
    {
        Type.Subtraction => UnaryType.Negative,
        Type.Complement => UnaryType.Not,
        _ => throw new Exception(),
    };
}
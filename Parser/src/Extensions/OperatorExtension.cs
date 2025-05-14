using PixelWallE.Parser.src.Enums;

namespace PixelWallE.Parser.src.Extensions;

public static class OperatorExtension
{
    public static BinaryType ToBinaryType(this TokenType type) => type switch
    {
        TokenType.Plus => BinaryType.Add,
        TokenType.Minus => BinaryType.Subtract,
        TokenType.Multiplication => BinaryType.Multiply,
        TokenType.Division => BinaryType.Divide,
        TokenType.Exponentiation => BinaryType.Power,
        TokenType.Modulus => BinaryType.Modulus,
        TokenType.LessOrEqual => BinaryType.LessOrEqualThan,
        TokenType.GreaterOrEqual => BinaryType.GreaterOrEqualThan,
        TokenType.Less => BinaryType.LessThan,
        TokenType.Greater => BinaryType.GreaterThan,
        TokenType.Equal => BinaryType.Equal,
        TokenType.NotEqual => BinaryType.NotEqual,
        TokenType.And => BinaryType.And,
        TokenType.Or => BinaryType.Or,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this BinaryType op) => op switch
    {
        BinaryType.Add => TokenType.Plus,
        BinaryType.Subtract => TokenType.Minus,
        BinaryType.Multiply => TokenType.Multiplication,
        BinaryType.Divide => TokenType.Division,
        BinaryType.Modulus => TokenType.Modulus,
        BinaryType.Power => TokenType.Exponentiation,
        BinaryType.LessOrEqualThan => TokenType.LessOrEqual,
        BinaryType.LessThan => TokenType.Less,
        BinaryType.GreaterOrEqualThan => TokenType.GreaterOrEqual,
        BinaryType.GreaterThan => TokenType.Greater,
        BinaryType.Equal => TokenType.Equal,
        BinaryType.NotEqual => TokenType.NotEqual,
        BinaryType.And => TokenType.And,
        BinaryType.Or => TokenType.Or,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this UnaryType op) => op switch
    {
        UnaryType.Negative => TokenType.Minus,
        UnaryType.Not => TokenType.Not,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this LiteralType literal) => literal switch
    {
        LiteralType.Boolean => TokenType.Boolean,
        LiteralType.String => TokenType.String,
        LiteralType.Integer => TokenType.Interger,
        _ => throw new Exception(),
    };

    public static UnaryType ToUnaryType(this TokenType type) => type switch
    {
        TokenType.Minus => UnaryType.Negative,
        TokenType.Not => UnaryType.Not,
        _ => throw new Exception(),
    };
}
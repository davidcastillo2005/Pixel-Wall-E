using PixelWallE.Parser.src.Enums;
using TokenType = PixelWallE.Lexer.src.Type;

namespace PixelWallE.Parser.src.Extensions;

public static class OperatorExtension
{
    public static BinaryOps ToBinaryType(this TokenType type) => type switch
    {
        TokenType.Plus => BinaryOps.Add,
        TokenType.Minus => BinaryOps.Subtract,
        TokenType.Multiplication => BinaryOps.Multiply,
        TokenType.Division => BinaryOps.Divide,
        TokenType.Exponentiation => BinaryOps.Power,
        TokenType.Modulus => BinaryOps.Modulus,
        TokenType.LessOrEqual => BinaryOps.LessOrEqualThan,
        TokenType.GreaterOrEqual => BinaryOps.GreaterOrEqualThan,
        TokenType.Less => BinaryOps.LessThan,
        TokenType.Greater => BinaryOps.GreaterThan,
        TokenType.Equal => BinaryOps.Equal,
        TokenType.NotEqual => BinaryOps.NotEqual,
        TokenType.And => BinaryOps.And,
        TokenType.Or => BinaryOps.Or,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this BinaryOps op) => op switch
    {
        BinaryOps.Add => TokenType.Plus,
        BinaryOps.Subtract => TokenType.Minus,
        BinaryOps.Multiply => TokenType.Multiplication,
        BinaryOps.Divide => TokenType.Division,
        BinaryOps.Modulus => TokenType.Modulus,
        BinaryOps.Power => TokenType.Exponentiation,
        BinaryOps.LessOrEqualThan => TokenType.LessOrEqual,
        BinaryOps.LessThan => TokenType.Less,
        BinaryOps.GreaterOrEqualThan => TokenType.GreaterOrEqual,
        BinaryOps.GreaterThan => TokenType.Greater,
        BinaryOps.Equal => TokenType.Equal,
        BinaryOps.NotEqual => TokenType.NotEqual,
        BinaryOps.And => TokenType.And,
        BinaryOps.Or => TokenType.Or,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this UnaryOps op) => op switch
    {
        UnaryOps.Negative => TokenType.Minus,
        UnaryOps.Not => TokenType.Not,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this Literals literal) => literal switch
    {
        Literals.Boolean => TokenType.Boolean,
        Literals.String => TokenType.String,
        Literals.Integer => TokenType.Interger,
        _ => throw new Exception(),
    };

    public static UnaryOps ToUnaryType(this TokenType type) => type switch
    {
        TokenType.Minus => UnaryOps.Negative,
        TokenType.Not => UnaryOps.Not,
        _ => throw new Exception(),
    };
}
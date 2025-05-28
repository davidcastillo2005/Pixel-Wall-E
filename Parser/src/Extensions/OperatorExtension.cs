using PixelWallE.Parser.src.Enums;
using TokenType = PixelWallE.Lexer.src.Type;

namespace PixelWallE.Parser.src.Extensions;

public static class OperatorExtension
{
    public static BinaryOperation ToBinaryType(this TokenType type) => type switch
    {
        TokenType.Plus => BinaryOperation.Add,
        TokenType.Minus => BinaryOperation.Subtract,
        TokenType.Multiplication => BinaryOperation.Multiply,
        TokenType.Division => BinaryOperation.Divide,
        TokenType.Exponentiation => BinaryOperation.Power,
        TokenType.Modulus => BinaryOperation.Modulus,
        TokenType.LessOrEqual => BinaryOperation.LessOrEqualThan,
        TokenType.GreaterOrEqual => BinaryOperation.GreaterOrEqualThan,
        TokenType.Less => BinaryOperation.LessThan,
        TokenType.Greater => BinaryOperation.GreaterThan,
        TokenType.Equal => BinaryOperation.Equal,
        TokenType.NotEqual => BinaryOperation.NotEqual,
        TokenType.And => BinaryOperation.And,
        TokenType.Or => BinaryOperation.Or,
        _ => throw new Exception(),
    };

    public static TokenType ToTokenType(this BinaryOperation op) => op switch
    {
        BinaryOperation.Add => TokenType.Plus,
        BinaryOperation.Subtract => TokenType.Minus,
        BinaryOperation.Multiply => TokenType.Multiplication,
        BinaryOperation.Divide => TokenType.Division,
        BinaryOperation.Modulus => TokenType.Modulus,
        BinaryOperation.Power => TokenType.Exponentiation,
        BinaryOperation.LessOrEqualThan => TokenType.LessOrEqual,
        BinaryOperation.LessThan => TokenType.Less,
        BinaryOperation.GreaterOrEqualThan => TokenType.GreaterOrEqual,
        BinaryOperation.GreaterThan => TokenType.Greater,
        BinaryOperation.Equal => TokenType.Equal,
        BinaryOperation.NotEqual => TokenType.NotEqual,
        BinaryOperation.And => TokenType.And,
        BinaryOperation.Or => TokenType.Or,
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
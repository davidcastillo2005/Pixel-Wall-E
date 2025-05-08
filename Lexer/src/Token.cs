namespace PixelWallE.Lexer.src;

public class Token(Type type, string value)
{
    public Type Type { get; } = type;
    public string Value = value;
}

public enum Type
{
    Identifier, Keyword, Addition, Subtraction, Multiplication, Division,
    Exponentiation, Modulus, LeftCurly, RightCurly, LeftBracket, RightBracket,
    LessOrEqual, GreaterOrEqual, Less, Greater, Equal, Number, Boolean, Assign,
    Comma, Color, NewLine, NotEqual, And, Or, Complement, EOF, Unknown
}
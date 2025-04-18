namespace PixelWall_E.Lexer.src;

public class Token(Type type, string value)
{
    public Type Type { get; } = type;
    public string Value = value;
}
public enum Type
{
    Identifier, Keyword, Addition, Subtraction, Multiplication, Division,
    Exponentiation, Remainder, LeftBracket, RightBracket, LeftSquareBracket,
    RightSquareBracketThan, LessOrEqualThan, GreaterOrEqualThan, LessThan,
    GreaterThan, Equal, Number, Assign, Comma, Color, NewLine, NotEqual, And, Or, Complement, EOF,
    Unknown
}
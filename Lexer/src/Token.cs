namespace PixelWallE.Lexer.src;

public class Token(Type type, string value)
{
    public Type Type { get; } = type;
    public string Value = value;
}

public enum Type
{
    Identifier, Keyword, Addition, Subtraction, Multiplication, Division,
    Exponentiation, Remainder, LeftBracket, RightBracket, LeftSquareBracket,
    RightSquareBracketThan, InclusiveLess, InclusiveGreater, ExclusiveLess,
    ExclusiveGreater, Equal, Number, Boolean, Assign, Comma, Color, NewLine, NotEqual, And, Or, Complement, EOF,
    Unknown
}
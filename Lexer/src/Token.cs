namespace PixelWallE.Lexer.src;

public class Token(Type type, string value)
{
    public Type Type { get; } = type;
    public string Value = value;

    public override string ToString()
    {
        return $"{Type}" + " : " + $"{Value}";
    }
}

public enum Type
{
    Identifier, Keyword, Plus, Minus, Multiplication, Division,
    Exponentiation, Modulus, String
    , LeftCurly, RightCurly, LeftBracket, RightBracket,
    LessOrEqual, GreaterOrEqual, Less, Greater, Equal, Interger, Boolean, Assign,
    Comma, Color, NewLine, NotEqual, And, Or, Not, EOF, Unknown, GoTo, False, True
}
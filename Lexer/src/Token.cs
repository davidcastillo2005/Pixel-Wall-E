namespace PixelWallE.Lexer.src;

public class Token(TokenType type, string value)
{
    public TokenType Type { get; } = type;
    public string Value = value;

    public override string ToString()
    {
        return $"{Type}" + " : " + $"{Value}";
    }
}

public enum TokenType
{
    Identifier, Keyword, Plus, Minus, Dot, Division,
    Exponentiation, Modulus, String
    , LeftCurly, RightCurly, LeftBracket, RightBracket,
    LessOrEqual, GreaterOrEqual, Less, Greater, Equal, Interger, Boolean, Assign,
    Comma, Color, NewLine, NotEqual, And, Or, Not, EOF, Unknown, GoTo, False, True
}
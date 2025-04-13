namespace PixelWall_E.Lexer.src;
public class Token(Type type, string value)
{
    public Type Type { get; } = type;
    public string Value = value;
}
public enum Type
{
    Identifier, Keyword, Addition, Subtraction, Multiplication, Division,
    Exponentiation, remainder, LeftBracket, RightBracket, LeftSquareBracket,
    RightSquareBracketThan, LessOrEqualThan, GreaterOrEqualThan, LessThan,
    GreaterThan, Equal, Number, Assign, Comma, Color, NewLine, NotEqual
}
public class Lexer
{
    private static bool IsPartOfIdentifierOrNumber(char c)
    {
        return char.IsLetterOrDigit(c) || c == '-';
    }
    private static void ProcessTemp(ref string temp, List<Token> tokens)
    {
        switch (temp)
        {
            case "false" or "true" or "Goto" or "GoTo" or "Spawn":
                tokens.Add(new(Type.Keyword, temp));
                break;
            case "Blue" or "Green" or "Red" or "Yellow" or "Black" or "White":
                tokens.Add(new(Type.Color, temp));
                break;
            default:
                tokens.Add(char.IsDigit(temp[0]) ? new(Type.Number, temp) : new(Type.Identifier, temp));
                break;
        }
        temp = "";
    }
    private static bool PeekNext(string input, int i, char c)
    {
        return i < input.Length - 1 && input[i + 1] == c;
    }

    public static Token[] Tokenize(string input)
    {
        string temp = "";
        List<Token> tokens = [];
        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];
            if (IsPartOfIdentifierOrNumber(currentChar))
            {
                temp += currentChar;
            }
            if (i == input.Length - 1 || temp != "")
            {
                ProcessTemp(ref temp, tokens);
            }
            switch (currentChar)
            {
                case '\n':
                    tokens.Add(new(Type.NewLine, "\\n"));
                    i++;
                    break;
                case '+':
                    tokens.Add(new(Type.Addition, currentChar.ToString()));
                    break;
                case '-' when temp == "":
                    tokens.Add(new(Type.Subtraction, currentChar.ToString()));
                    break;
                case '/':
                    tokens.Add(new(Type.Division, currentChar.ToString()));
                    break;
                case '*':
                    if (PeekNext(input, i, '*'))
                    {
                        tokens.Add(new(Type.Exponentiation, "**"));
                        i++;
                    }
                    else
                    {
                        tokens.Add(new(Type.Multiplication, currentChar.ToString()));
                    }
                    break;
                case '<':
                    if (PeekNext(input, i, '-'))
                    {
                        tokens.Add(new(Type.Assign, "<-"));
                        i++;
                    }
                    else
                    {
                        tokens.Add(new(Type.LessThan, currentChar.ToString()));
                    }
                    break;
                case '>':
                    if (PeekNext(input, i, '='))
                    {
                        tokens.Add(new(Type.GreaterOrEqualThan, ">="));
                        i++;
                    }
                    else
                    {
                        tokens.Add(new(Type.GreaterThan, currentChar.ToString()));
                    }
                    break;
                case '=':
                    if (PeekNext(input, i, '='))
                    {
                        tokens.Add(new(Type.Equal, "=="));
                        i++;
                    }
                    else
                    {
                        tokens.Add(new(Type.Assign, currentChar.ToString()));
                    }
                    break;
                case '!':
                    if (PeekNext(input, i, '='))
                    {
                        tokens.Add(new(Type.NotEqual, "!="));
                        i++;
                    }
                    break;
                case '(':
                    tokens.Add(new(Type.LeftBracket, currentChar.ToString()));
                    break;
                case ')':
                    tokens.Add(new(Type.RightBracket, currentChar.ToString()));
                    break;
                case '[':
                    tokens.Add(new(Type.LeftSquareBracket, currentChar.ToString()));
                    break;
                case ']':
                    tokens.Add(new(Type.RightSquareBracketThan, currentChar.ToString()));
                    break;
                case ',':
                    tokens.Add(new(Type.Comma, currentChar.ToString()));
                    break;
                case '%':
                    tokens.Add(new(Type.remainder, currentChar.ToString()));
                    break;
            }
        }
        return [.. tokens];
    }
}
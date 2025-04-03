namespace Lexer.src;

public class Token(Type type, string lexeme)
{
    public Type Type { get; } = type;
    public string Lexeme { get; set; } = lexeme;
}

public enum Type 
{ 
    Identifier, Keyword, Addition, Subtraction, Multiplication, Division, Exponentiation, Remainder, 
    LeftBracket, RightBracket, LeftSquareBracket, RightSquareBracket, LessOrEqual, GreaterOrEqual, 
    Less, Greater, Equal, Number, Assign, Comma, Color, NewLine 
}

public class Lexer
{
    public static Token[] Tokenize(string input)
    {
        List<Token> tokens = [];
        string temp = "";

        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];

            if (IsPartOfIdentifierOrNumber(currentChar))
            {
                temp += currentChar;
            }
            else
            {
                ProcessTemp(ref temp, tokens);

                if (currentChar == '\n')
                {
                    tokens.Add(new Token(Type.NewLine, "\\n"));
                }
                else if (TryProcessOperator(input, ref i, currentChar, tokens))
                {
                    continue;
                }
            }
        }

        // Process any remaining temp value
        ProcessTemp(ref temp, tokens);

        return tokens.ToArray();
    }

    private static bool IsPartOfIdentifierOrNumber(char c)
    {
        return char.IsLetterOrDigit(c) || c == '-';
    }

    private static void ProcessTemp(ref string temp, List<Token> tokens)
    {
        if (string.IsNullOrEmpty(temp)) return;

        if (IsKeyword(temp))
        {
            tokens.Add(new Token(Type.Keyword, temp));
        }
        else if (IsColor(temp))
        {
            tokens.Add(new Token(Type.Color, temp));
        }
        else
        {
            var type = char.IsDigit(temp[0]) ? Type.Number : Type.Identifier;
            tokens.Add(new Token(type, temp));
        }

        temp = "";
    }

    private static bool IsKeyword(string value)
    {
        return value is "false" or "true" or "Goto" or "GoTo";
    }

    private static bool IsColor(string value)
    {
        return value is "Blue" or "Green" or "Red" or "Yellow" or "Black" or "White";
    }

    private static bool TryProcessOperator(string input, ref int i, char currentChar, List<Token> tokens)
    {
        switch (currentChar)
        {
            case '+':
                tokens.Add(new Token(Type.Addition, "+"));
                break;
            case '-':
                tokens.Add(new Token(Type.Subtraction, "-"));
                break;
            case '/':
                tokens.Add(new Token(Type.Division, "/"));
                break;
            case '*':
                if (PeekNext(input, i) == '*')
                {
                    tokens.Add(new Token(Type.Exponentiation, "**"));
                    i++;
                }
                else
                {
                    tokens.Add(new Token(Type.Multiplication, "*"));
                }
                break;
            case '<':
                if (PeekNext(input, i) == '-')
                {
                    tokens.Add(new Token(Type.Assign, "<-"));
                    i++;
                }
                else if (PeekNext(input, i) == '=')
                {
                    tokens.Add(new Token(Type.LessOrEqual, "<="));
                    i++;
                }
                else
                {
                    tokens.Add(new Token(Type.Less, "<"));
                }
                break;
            case '>':
                if (PeekNext(input, i) == '=')
                {
                    tokens.Add(new Token(Type.GreaterOrEqual, ">="));
                    i++;
                }
                else
                {
                    tokens.Add(new Token(Type.Greater, ">"));
                }
                break;
            case '=':
                if (PeekNext(input, i) == '=')
                {
                    tokens.Add(new Token(Type.Equal, "=="));
                    i++;
                }
                else
                {
                    tokens.Add(new Token(Type.Assign, "="));
                }
                break;
            case '!':
                if (PeekNext(input, i) == '=')
                {
                    tokens.Add(new Token(Type.Equal, "!="));
                    i++;
                }
                break;
            case '(':
                tokens.Add(new Token(Type.LeftBracket, "("));
                break;
            case ')':
                tokens.Add(new Token(Type.RightBracket, ")"));
                break;
            case '[':
                tokens.Add(new Token(Type.LeftSquareBracket, "["));
                break;
            case ']':
                tokens.Add(new Token(Type.RightSquareBracket, "]"));
                break;
            case ',':
                tokens.Add(new Token(Type.Comma, ","));
                break;
            case '%':
                tokens.Add(new Token(Type.Remainder, "%"));
                break;
            default:
                return false;
        }

        return true;
    }

    private static char PeekNext(string input, int index)
    {
        return index + 1 < input.Length ? input[index + 1] : '\0';
    }
}
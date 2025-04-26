using System.Text.RegularExpressions;
namespace PixelWallE.Lexer.src;

public class Lexer
{
    public static Token[] ScanInput(string input)
    {
        string temp = "";
        List<Token> tokens = [];
        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];
            if (currentChar == ' ')
                continue;
            if (temp != "" && !IsPartOfIdentifierOrNumber(currentChar, temp))
            {
                ProcessTemp(ref temp, tokens);
            }
            var token = ProcessChar(input, currentChar, ref i);
            if (token != null)
            {
                tokens.Add(token);
                continue;
            }
            temp += currentChar;
        }

        if (temp != "")
        {
            ProcessTemp(ref temp, tokens);
        }

        tokens.Add(new Token(Type.EOF, "$"));
        return [.. tokens];
    }

    private static bool IsPartOfIdentifierOrNumber(char c, string temp)
    {
        return char.IsLetterOrDigit(c);
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
                tokens.Add(int.TryParse(temp, out _) ? new(Type.Number, temp) : new(Type.Identifier, temp));
                break;
        }
        temp = "";
    }

    private static Token? ProcessChar(string input, char currentChar, ref int i)
    {
        switch (currentChar)
        {
            case '\n':
                i++;
                return new(Type.NewLine, "\\n");
            case '+':
                return new(Type.Addition, currentChar.ToString());
            case '-':
                return new(Type.Subtraction, currentChar.ToString());
            case '/':
                return new(Type.Division, currentChar.ToString());
            case '*':
                if (PeekNext(input, i, '*'))
                {
                    i++;
                    return new(Type.Exponentiation, "**");
                }
                else
                {
                    return new(Type.Multiplication, currentChar.ToString());
                }
            case '<':
                if (PeekNext(input, i, '-'))
                {
                    i++;
                    return new(Type.Assign, "<-");
                }
                else
                {
                    return new(Type.ExclusiveLess, currentChar.ToString());
                }
            case '>':
                if (PeekNext(input, i, '='))
                {
                    i++;
                    return new(Type.InclusiveGreater, ">=");
                }
                else
                {
                    return new(Type.ExclusiveGreater, currentChar.ToString());
                }
            case '=':
                if (PeekNext(input, i, '='))
                {
                    i++;
                    return new(Type.Equal, "==");
                }
                return new(Type.Assign, currentChar.ToString());
            case '!':
                if (PeekNext(input, i, '='))
                {
                    i++;
                    return new(Type.NotEqual, "!=");
                }
                return new(Type.Complement, "!");
            case '(':
                return new(Type.LeftBracket, currentChar.ToString());
            case ')':
                return new(Type.RightBracket, currentChar.ToString());
            case '[':
                return new(Type.LeftSquareBracket, currentChar.ToString());
            case ']':
                return new(Type.RightSquareBracketThan, currentChar.ToString());
            case ',':
                return new(Type.Comma, currentChar.ToString());
            case '%':
                return new(Type.Remainder, currentChar.ToString());
            default:
                return null;
        }
    }

    private static bool PeekNext(string input, int i, char c)
    {
        return i < input.Length - 1 && input[i + 1] == c;
    }
}
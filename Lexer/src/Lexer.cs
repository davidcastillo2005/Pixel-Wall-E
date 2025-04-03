namespace Lexer.src;
public class Token(Type type, string lexeme)
{
    public Type Type { get; } = type;
    public string Lexeme { get; set; } = lexeme;
}
public enum Type
{
    Identifier, Keyword, Addition, Subtraction, Multiplication, Division,
    Exponentiation, Remainder, LeftBracket, RightBracket, LeftSquareBracket,
    RightSquareBracket, LessOrEqual, GreaterOrEqual, Less, Greater, Equal,
    Number, Assign, Comma, Color, NewLine
}
public class Lexer()
{
    public static Token[] Tokenize(string input)
    {
        List<Token> tokens = [];
        string temp = "";
        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];
            Token token;
            if (currentChar == '-' || char.IsDigit(currentChar) || char.IsLetter(currentChar))
            {
                temp += currentChar;
            }
            else if (i == input.Length - 1 || temp != "")
            {
                if (temp == "false" || temp == "true" || temp == "Goto" || temp == "GoTo")
                {
                    token = new(Type.Keyword, temp);
                    tokens.Add(token);
                    temp = "";
                }
                else if (temp == "Blue" || temp == "Green" || temp == "Red" || temp == "Yellow" || temp == "Black" || temp == "White")
                {
                    token = new(Type.Color, temp);
                    tokens.Add(token);
                    temp = "";
                }
                else
                {
                    token = char.IsDigit(temp[0]) ? new(Type.Number, temp) : new(Type.Identifier, temp);
                    tokens.Add(token);
                    temp = "";
                }
            }
            switch (currentChar)
            {
                case '\n':
                    token = new(Type.NewLine, "\\n");
                    tokens.Add(token);
                    i++;
                    break;
                case '+':
                    token = new(Type.Addition, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '-' when temp == "":
                    token = new(Type.Subtraction, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '/':
                    token = new(Type.Division, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '*':
                    if (input[i + 1] == '*')
                    {
                        token = new(Type.Exponentiation, "**");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(Type.Multiplication, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '<':
                    if (input[i + 1] == '-')
                    {
                        token = new(Type.Assign, "<-");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(Type.Less, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '>':
                    if (input[i + 1] == '=')
                    {
                        token = new(Type.GreaterOrEqual, ">=");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(Type.Greater, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '=':
                    if (input[i + 1] == '=')
                    {
                        token = new(Type.Equal, "==");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(Type.Assign, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '!':
                    if (input[i + 1] == '=')
                    {
                        token = new(Type.Equal, "!=");
                        tokens.Add(token);
                        i++;
                    }
                    break;
                case '(':
                    token = new(Type.LeftBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case ')':
                    token = new(Type.RightBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '[':
                    token = new(Type.LeftSquareBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case ']':
                    token = new(Type.RightSquareBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case ',':
                    token = new(Type.Comma, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '%':
                    token = new(Type.Remainder, currentChar.ToString());
                    tokens.Add(token);
                    break;
            }
        }
        return [.. tokens];
    }
}
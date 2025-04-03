namespace Lexer.src;
public class Token(TokenType type, string value)
{
    public TokenType Type { get; } = type;
    public string Value { get; set; } = value;
}
public enum TokenType { Identifier, Keyword, Addition, Subtraction, Multiplication, Division, Exponentiation, Remainder, LeftBracket, RightBracket, LeftSquareBracket, RightSquareBracket, LessOrEqual, GreaterOrEqual, Less, Greater, Equal, Number, Assign, Comma, Color, NewLine }
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
                    token = new(TokenType.Keyword, temp);
                    tokens.Add(token);
                    temp = "";
                }
                else if (temp == "Blue" || temp == "Green" || temp == "Red" || temp == "Yellow" || temp == "Black" || temp == "White")
                {
                    token = new(TokenType.Color, temp);
                    tokens.Add(token);
                    temp = "";
                }
                else
                {
                    token = char.IsDigit(temp[0]) ? new(TokenType.Number, temp) : new(TokenType.Identifier, temp);
                    tokens.Add(token);
                    temp = "";
                }
            }
            switch (currentChar)
            {
                case '\n':
                    token = new(TokenType.NewLine, "\\n");
                    tokens.Add(token);
                    i++;
                    break;
                case '+':
                    token = new(TokenType.Addition, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '-' when temp == "":
                    token = new(TokenType.Subtraction, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '/':
                    token = new(TokenType.Division, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '*':
                    if (input[i + 1] == '*')
                    {
                        token = new(TokenType.Exponentiation, "**");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(TokenType.Multiplication, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '<':
                    if (input[i + 1] == '-')
                    {
                        token = new(TokenType.Assign, "<-");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(TokenType.Less, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '>':
                    if (input[i + 1] == '=')
                    {
                        token = new(TokenType.GreaterOrEqual, ">=");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(TokenType.Greater, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '=':
                    if (input[i + 1] == '=')
                    {
                        token = new(TokenType.Equal, "==");
                        tokens.Add(token);
                        i++;
                    }
                    else
                    {
                        token = new(TokenType.Assign, currentChar.ToString());
                        tokens.Add(token);
                    }
                    break;
                case '!':
                    if (input[i + 1] == '=')
                    {
                        token = new(TokenType.Equal, "!=");
                        tokens.Add(token);
                        i++;
                    }
                    break;
                case '(':
                    token = new(TokenType.LeftBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case ')':
                    token = new(TokenType.RightBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '[':
                    token = new(TokenType.LeftSquareBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case ']':
                    token = new(TokenType.RightSquareBracket, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case ',':
                    token = new(TokenType.Comma, currentChar.ToString());
                    tokens.Add(token);
                    break;
                case '%':
                    token = new(TokenType.Remainder, currentChar.ToString());
                    tokens.Add(token);
                    break;
            }
        }
        return [.. tokens];
    }
}
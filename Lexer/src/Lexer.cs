using System.Text.RegularExpressions;
namespace Lexer.src;
public class Token(TokenType type, string value)
{
    public TokenType Type { get; } = type;
    public string Value { get; set; } = value;

    public static Token Sum(Token token1, Token token2)
    {
        if (token1.Type != token2.Type)
        {
            throw new Exception();
        }
        return new Token(token1.Type, token1.Value + token2.Value);
    }
}
public enum TokenType { Identifier, Keyword, Addition, Subtraction, Multiplication, Division, Exponentiation, Remainder, LeftBracket, RightBracket, LeftSquareBracket, RightSquareBracket, Number, AssignOp, Comma, CBlue, CRed, CGreen }
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
                switch (temp)
                {
                    case "GoTo":
                        token = new(TokenType.Keyword, temp);
                        tokens.Add(token);
                        temp = "";
                        break;
                    case "Blue":
                        token = new(TokenType.CBlue, temp);
                        tokens.Add(token);
                        temp = "";
                        break;
                    case "Green":
                        token = new(TokenType.CGreen, temp);
                        tokens.Add(token);
                        temp = "";
                        break;
                    case "Red":
                        token = new(TokenType.CRed, temp);
                        tokens.Add(token);
                        temp = "";
                        break;
                    default:
                        token = char.IsDigit(temp[0]) == true ? new(TokenType.Number, temp) : new(TokenType.Identifier, temp);
                        tokens.Add(token);
                        temp = "";
                        break;
                }
            }
            switch (currentChar)
            {
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
                        token = new(TokenType.AssignOp, "<-");
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
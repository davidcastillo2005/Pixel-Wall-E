using System.Text.RegularExpressions;
namespace Lexer.src;
public class Token
{
    public TokenType Type { get; }
    public string Value { get; set; }
    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }
    public static Token Sum(Token token1, Token token2)
    {
        if (token1.Type != token2.Type)
        {
            throw new Exception();
        }
        return new Token(token1.Type, token1.Value + token2.Value);
    }
}
public enum TokenType { Identifier, BinaryOperator, Dots, Number, AssingOperator }
public class Lexer()
{
    public Token[] Tokenize(string input)
    {
        List<Token> tokens = [];
        string temp = "";
        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];
            if (char.IsDigit(currentChar))
            {
                temp += currentChar;
            }
            if (char.IsLetter(currentChar))
            {
                temp += currentChar;
            }
            if ((i == input.Length - 1 || !char.IsDigit(currentChar) && !char.IsLetter(currentChar)) && temp != "")
            {
                Token token = char.IsDigit(temp[0]) == true ? new(TokenType.Number, temp) : new(TokenType.Identifier, temp);
                tokens.Add(token);
                temp = "";
            }
            if (currentChar == '+' || currentChar == '-' || currentChar == '/' || currentChar == '*')
            {
                Token token = new(TokenType.BinaryOperator, currentChar.ToString());
                tokens.Add(token);
            }
            if (currentChar == '=')
            {
                Token token = new(TokenType.AssingOperator, currentChar.ToString());
                tokens.Add(token);
            }
            if (currentChar == '(' || currentChar == ')')
            {
                Token token = new(TokenType.Dots, currentChar.ToString());
                tokens.Add(token);
            }
        }
        if (temp != "")
        {
            Token token = new(TokenType.Dots, temp);
        }
        return tokens.ToArray();
    }
}
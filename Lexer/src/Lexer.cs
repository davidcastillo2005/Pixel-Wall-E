namespace PixelWallE.Lexer.src;

public class Lexer
{
    private int SourceIndex = 0;

    private List<Token> tokens = [];

    private delegate bool IsIntOrId(string source, int startIndex);

    private readonly Dictionary<string, TokenType> keyword = new Dictionary<string, TokenType>
    {
        {"GoTo", TokenType.GoTo},
        {"false", TokenType.Boolean},
        {"true", TokenType.Boolean},
    };

    public Token[] Tokenize(string input)
    {
        bool ReadChar;
        do
        {
            if (!(ReadChar = ReadWhiteSpace(input))
                && (ReadChar = TryGetNewLineToken(input, out Token? token)
                || TryGetStrToken(input, out token)
                || TryGetIntToken(input, out token)
                || TryGetSymToken(input, out token)
                || TryGetIdentifier(input, out token)))
            {
                tokens.Add(token!);
            }
        } while (ReadChar);

        tokens.Add(new Token(TokenType.EOF, "$"));
        return [.. tokens];
    }

    private bool TryGetNewLineToken(string input, out Token? token)
    {
        int startIndex = SourceIndex;
        if (TryMatchPattern(input, "\n") || TryMatchPattern(input, "\r\n"))
        {
            token = new Token(TokenType.NewLine, "\n");
            return true;
        }
        return ResetSourceIndex(startIndex, out token);
    }

    private bool ReadWhiteSpace(string input)
    {
        var space = false;
        while (TryMatchPattern(input, " "))
            space = true;
        return space;
    }

    #region Symbols

    private bool TryGetSymToken(string source, out Token? token)
    {
        int startIndex = SourceIndex;
        int nextIndex = startIndex++;
        if (source[SourceIndex] == '+')
        {
            return GetDefaultToken(new Token(TokenType.Plus, "+"), out token);
        }
        else if (source[SourceIndex] == '-')
        {
            return GetDefaultToken(new Token(TokenType.Minus, "-"), out token);
        }
        else if (source[SourceIndex] == '*')
        {
            if (source[nextIndex] == '*')
            {
                return GetDefaultToken(new Token(TokenType.Exponentiation, "**"), out token);
            }
            else
            {
                return GetDefaultToken(new Token(TokenType.Dot, "*"), out token);
            }
        }
        else if (source[SourceIndex] == '/')
        {
            return GetDefaultToken(new Token(TokenType.Division, "/"), out token);
        }
        else if (source[SourceIndex] == '%')
        {
            return GetDefaultToken(new Token(TokenType.Modulus, "%"), out token);
        }
        else if (source[SourceIndex] == '(')
        {
            return GetDefaultToken(new Token(TokenType.LeftCurly, "("), out token);
        }
        else if (source[SourceIndex] == ')')
        {
            return GetDefaultToken(new Token(TokenType.RightCurly, ")"), out token);
        }
        else if (source[SourceIndex] == '[')
        {
            return GetDefaultToken(new Token(TokenType.LeftBracket, "["), out token);
        }
        else if (source[SourceIndex] == ']')
        {
            return GetDefaultToken(new Token(TokenType.RightBracket, "]"), out token);
        }
        else if (source[SourceIndex] == '<')
        {
            if (source[SourceIndex] == '=')
            {
                return GetDefaultToken(new Token(TokenType.LessOrEqual, "<="), out token);
            }
            else if (source[SourceIndex] == '-')
            {
                return GetDefaultToken(new Token(TokenType.Assign, "<-"), out token);
            }
            else
            {
                return GetDefaultToken(new Token(TokenType.Less, "<"), out token);
            }
        }
        else if (source[SourceIndex] == '>')
        {
            if (source[SourceIndex] == '=')
            {
                return GetDefaultToken(new Token(TokenType.GreaterOrEqual, ">="), out token);
            }
            else
            {
                return GetDefaultToken(new Token(TokenType.Greater, ">"), out token);
            }
        }
        else if (source[SourceIndex] == '=' && source[nextIndex] == '=')
        {
            return GetDefaultToken(new Token(TokenType.Equal, "=="), out token);
        }
        return ResetSourceIndex(startIndex, out token);
    }

    #endregion

    #region Interger

    private bool TryGetIntToken(string input, out Token? token)
    {
        if (TryGetIntOrIdOrKey(input, IsInterger, out string? i))
        {
            return GetDefaultToken(new Token(TokenType.Interger, i!), out token);
        }
        return ResetSourceIndex(SourceIndex, out token);
    }

    private bool IsInterger(string input, int startIndex)
        => SourceIndex < input.Length && char.IsDigit(input[SourceIndex]);

    #endregion

    #region Identifier

    private bool TryGetIdentifier(string input, out Token? token)
    {
        if (TryGetIntOrIdOrKey(input, IsIdentifier, out string? value))
        {
            var isKeyword = keyword.TryGetValue(value!, out TokenType keywordType);
            var type = isKeyword ? keywordType : TokenType.Identifier;
            return GetDefaultToken(new Token(type, value!), out token);
        }
        return ResetSourceIndex(SourceIndex, out token);
    }

    private bool IsIdentifier(string input, int startIndex)
        => SourceIndex < input.Length && !(char.IsDigit(input[SourceIndex]) && startIndex == SourceIndex)
        && char.IsLetterOrDigit(input[SourceIndex]);

    #endregion

    #region String

    private bool TryGetStrToken(string input, out Token? token)
    {
        if (TryMatchPattern(input, "\"") && TryGetStrValue(input, out string? value) && TryMatchPattern(input, "\""))
        {
            return GetDefaultToken(new Token(TokenType.String, value!), out token);
        }
        token = null;
        return false;
    }

    private bool TryGetStrValue(string input, out string? tokenValue)
    {
        int startIndex = SourceIndex;
        string temp = "";
        while (input[SourceIndex] != '\"')
        {
            temp += input[SourceIndex++];
        }

        if (temp != "")
        {
            tokenValue = temp;
            return true;
        }
        return ResetSourceIndex(startIndex, out tokenValue);
    }

    #endregion

    #region Tools

    private bool TryGetIntOrIdOrKey(string input, IsIntOrId isIntOrId, out string? tokenValue)
    {
        int startIndex = SourceIndex;
        string temp = "";

        while (isIntOrId(input, startIndex))
        {
            temp += input[SourceIndex++];
        }

        if (temp != "")
        {
            tokenValue = temp;
            return true;
        }
        return ResetSourceIndex(startIndex, out tokenValue);
    }

    private bool TryMatchPattern(string input, string pattern)
    {
        int startIndex = SourceIndex;

        for (int i = 0; i < pattern.Length; i++)
        {
            if (i + startIndex > input.Length - 1 || pattern[i] != input[i + startIndex])
            {
                SourceIndex = startIndex;
                return false;
            }
            SourceIndex++;
        }
        return true;
    }

    private bool ResetSourceIndex<T>(int startIndex, out T? token)
    {
        SourceIndex = startIndex;
        token = default;
        return false;
    }

    private bool GetDefaultToken<T>(T value, out T? output)
    {
        output = value;
        return true;
    }

    #endregion
}
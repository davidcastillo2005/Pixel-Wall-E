namespace PixelWallE.Lexer.src;

public class Lexer
{
    private int SourceIndex = 0;

    private List<Token> tokens = [];

    private delegate bool IsIntOrId(string source, int startIndex);

    private readonly Dictionary<string, Type> keyword = new Dictionary<string, Type>
    {
        {"GoTo", Type.GoTo},
        {"false", Type.Boolean},
        {"true", Type.Boolean},
    };

    private readonly List<(string symbol, Type token)> symbols =
    [
        ("==", Type.Equal), ("!=", Type.NotEqual), ("<=", Type.LessOrEqual), (">=", Type.GreaterOrEqual),
        ("&&", Type.And), ("||", Type.Or), ("!", Type.Not), ("<-", Type.Assign), ("+", Type.Plus), ("-", Type.Minus),
        ("^", Type.Exponentiation), ("*", Type.Multiplication), (">", Type.Greater), ("<", Type.Less),
        ("/", Type.Division), ("%", Type.Modulus), ("(", Type.LeftCurly), (")", Type.RightCurly),
        ("[", Type.LeftBracket), ("]", Type.RightBracket), (",", Type.Comma)
    ];

    public Token[] Tokenize(string input)
    {
        bool ReadChar;
        do
        {
            if (!(ReadChar = ReadWhiteSpace(input)))
            {
                if (ReadChar = TryGetNewLineToken(input, out Token? token)
                || TryGetStrToken(input, out token)
                || TryGetIntToken(input, out token)
                || TryGetSymToken(input, out token)
                || TryGetIdentifier(input, out token))
                {
                    tokens.Add(token!);
                }
            }
        } while (ReadChar);

        tokens.Add(new Token(Type.EOF, "$"));
        return [.. tokens];
    }

    private bool TryGetNewLineToken(string input, out Token? token)
    {
        int startIndex = SourceIndex;
        if (MatchPattern(input, "\n") || MatchPattern(input, "\r\n"))
        {
            token = new Token(Type.NewLine, "\n");
            return true;
        }
        return ResetToken(startIndex, out token);
    }

    private bool ReadWhiteSpace(string input)
    {
        var space = false;
        while (MatchPattern(input, " "))
            space = true;
        return space;
    }

    #region Symbols

    private bool TryGetSymToken(string input, out Token? token)
    {
        int startIndex = SourceIndex;
        if (input)
        return ResetToken(startIndex, out token);
    }

    #endregion

    #region Interger

    private bool TryGetIntToken(string input, out Token? token)
    {
        if (TryGetIntOrIdOrKey(input, IsInterger, out string? i))
        {
            token = new Token(Type.Interger, i!);
            return true;
        }
        return ResetToken(SourceIndex, out token);
    }

    private bool IsInterger(string input, int startIndex)
        => SourceIndex < input.Length && char.IsDigit(input[SourceIndex]);

    #endregion

    #region Identifier

    private bool TryGetIdentifier(string input, out Token? token)
    {
        if (TryGetIntOrIdOrKey(input, IsIdentifier, out string? value))
        {
            var isKeyword = keyword.TryGetValue(value!, out Type keywordType);
            var type = isKeyword ? keywordType : Type.Identifier;
            token = new Token(type, value!);
            return true;
        }
        return ResetToken(SourceIndex, out token);
    }

    private bool IsIdentifier(string input, int startIndex)
        => SourceIndex < input.Length && !(char.IsDigit(input[SourceIndex]) && startIndex == SourceIndex)
        && char.IsLetterOrDigit(input[SourceIndex]);

    #endregion

    #region String

    private bool TryGetStrToken(string input, out Token? token)
    {
        if (MatchPattern(input, "\"") && TryGetStrValue(input, out string? value) && MatchPattern(input, "\""))
        {
            token = new Token(Type.String, value!);
            return true;
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
        return ResetToken(startIndex, out tokenValue);
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
        return ResetToken(startIndex, out tokenValue);
    }

    private bool MatchPattern(string input, string pattern)
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

    private bool ResetToken<T>(int startIndex, out T? token)
    {
        SourceIndex = startIndex;
        token = default;
        return false;
    }

    #endregion
}
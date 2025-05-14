namespace PixelWallE.Lexer.src;

public class Lexer
{
    private int InputIndex = 0;

    private List<Token> tokens = [];

    private delegate bool IsIntOrId(string input, int startIndex);

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
        int startIndex = InputIndex;
        if (MatchPattern(input, "\n") || MatchPattern(input, "\\n"))
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
        int startIndex = InputIndex;
        foreach (var item in symbols)
        {
            if (MatchPattern(input, item.symbol))
            {
                token = new Token(item.token, item.symbol);
                return true;
            }
        }
        return ResetToken(startIndex, out token);
    }

    #endregion

    #region Interger

    private bool TryGetIntToken(string input, out Token? token)
    {
        if (TryGetIntOrId(input, IsInterger, out string? i))
        {
            token = new Token(Type.Interger, i!);
            return true;
        }
        return ResetToken(InputIndex, out token);
    }

    private bool IsInterger(string input, int startIndex)
        => InputIndex < input.Length && char.IsDigit(input[InputIndex]);

    #endregion

    #region Identifier

    private bool TryGetIdentifier(string input, out Token? token)
    {
        if (TryGetIntOrId(input, IsIdentifier, out string? name))
        {
            token = new Token(Type.Identifier, name!);
            return true;
        }
        return ResetToken(InputIndex, out token);
    }

    private bool IsIdentifier(string input, int startIndex)
        => InputIndex < input.Length && !(char.IsDigit(input[InputIndex]) && startIndex == InputIndex)
        && char.IsLetterOrDigit(input[InputIndex]);

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
        int startIndex = InputIndex;
        string temp = "";
        while (input[InputIndex] != '\"')
        {
            temp += input[InputIndex++];
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

    private bool TryGetIntOrId(string input, IsIntOrId isIntOrId, out string? tokenValue)
    {
        int startIndex = InputIndex;
        string temp = "";

        while (isIntOrId(input, startIndex))
        {
            temp += input[InputIndex++];
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
        int startIndex = InputIndex;

        for (int i = 0; i < pattern.Length; i++)
        {
            if (i + startIndex > input.Length - 1 || pattern[i] != input[i + startIndex])
            {
                InputIndex = startIndex;
                return false;
            }
            InputIndex++;
        }
        return true;
    }

    private bool ResetToken<T>(int startIndex, out T? token)
    {
        InputIndex = startIndex;
        token = default;
        return false;
    }

    #endregion
}
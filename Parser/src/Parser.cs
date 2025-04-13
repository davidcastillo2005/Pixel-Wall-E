using System.Data;
using PixelWall_E.Parser.src.Expressions;
using PixelWall_E.Lexer.src;
using Type = PixelWall_E.Lexer.src.Type;
namespace PixelWall_E.Parser.src;

public class Parser
{
    int tokenIndex;

    public IExpression Parse(Token[] tokens)
    {
        return TryGetBlockExpre(tokens, out IExpression? expre) ? expre! : throw new InvalidExpressionException();
    }

    public bool TryGetBlockExpre(Token[] tokens, out IExpression? expre)
    {
        List<IExpression> expressions = [];
        bool ReadLine;

        do
        {
            if (ReadLine = TryGetAssignExpre(tokens, out IExpression? lineExpre))
            {
                expressions.Add(lineExpre!);
            }
            else if (ReadLine = TryGetFunctionExpre(tokens, out lineExpre))
            {
                expressions.Add(lineExpre);
            }
        } while (ReadLine);

        var node = new BlockExpression([.. expressions]);
        return GetDefaultExpre(node, out expre);
    }

    private bool TryGetAssignExpre(Token[] tokens, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string name = tokens[tokenIndex].Value;
        if (tokens[tokenIndex++].Type != Type.Identifier || tokens[tokenIndex++].Type != Type.Assign)
        {
            return ResetDefaultExpre(startIndex, out expre);
        }

        if (TryParseArithExpre(tokens, out IExpression<int>? num))
        {
            return TryAssignExpre(name, num!, out expre);
        }
        if (TryParseBooleanExpre(tokens, out IExpression<bool>? b))
        {
            return TryAssignExpre(name, b, out expre);
        }
        if (TryParseStringExpre(tokens, out IExpression<string>? str))
        {
            return TryAssignExpre(name, str, out expre);
        }

        return ResetDefaultExpre(startIndex, out expre);
    }

    private bool TryAssignExpre<T>(string name, IExpression<T> value, out IExpression expre)
    {
        var node = new AssignExpre<T>(name, value);
        return GetDefaultExpre(node, out expre);
    }

    private bool TryParseArithExpre(Token[] tokens, out IExpression<int>? expression)
    {
        return TryGetSummation(tokens, out expression);
    }

    private bool TryGetSummation(Token[] tokens, out IExpression<int>? expre)
    {
        int startIndex = tokenIndex;
        if (!TryGetProduct(tokens, out IExpression<int>? left))
        {
            return ResetDefaultExpre(startIndex, out expre);
        }

        if (tokens[tokenIndex++].Type == Type.Addition
            && TryGetSummation(tokens, out IExpression<int>? right))
        {
            var node = new AdditionExpre(left!, right!);
            return GetDefaultExpre(node, out expre);
        }

        return GetDefaultExpre(left!, out expre);
    }

    private bool TryGetProduct(Token[] tokens, out IExpression<int>? expre)
    {
        int startIndex = tokenIndex;
        if (!TryGetPow(tokens, out IExpression<int>? left))
        {
            return ResetDefaultExpre(startIndex, out expre);
        }

        if (tokens[tokenIndex++].Type == Type.Multiplication
            && TryGetProduct(tokens, out IExpression<int>? right))
        {
            var node = new MultiplicationExpre(left!, right!);
            return GetDefaultExpre(node, out expre);
        }

        return GetDefaultExpre(left!, out expre);
    }

    private bool TryGetPow(Token[] tokens, out IExpression<int>? expre)
    {
        int startIndex = tokenIndex;
        if (!TryGetNum(tokens, out IExpression<int>? left))
        {
            return ResetDefaultExpre(startIndex, out expre);
        }

        if (tokens[tokenIndex++].Type == Type.Exponentiation
            && TryGetPow(tokens, out IExpression<int>? right))
        {
            var node = new ExponentiationExpre(left!, right!);
            return GetDefaultExpre(node, out expre);
        }

        return GetDefaultExpre(left!, out expre);
    }

    private bool TryGetNum(Token[] tokens, out IExpression<int>? expre)
    {
        int startIndex = tokenIndex;
        Token token = tokens[tokenIndex++];
        if (token.Type == Type.Number)
        {
            var node = new NumberExpre(int.Parse(token.Value));
            return GetDefaultExpre(node, out expre);
        }
        if (token.Type == Type.Identifier
            && TryParseFunction(tokens, out IExpression<int>? result))
        {
            return GetDefaultExpre(result!, out expre);
        }
        if (token.Type == Type.LeftBracket
            && TryParseArithExpre(tokens, out result)
            && token.Type == Type.RightBracket)
        {
            return GetDefaultExpre(result!, out expre);
        }
        return ResetDefaultExpre(startIndex, out expre);
    }

    private bool TryParseFunction<T>(Token[] tokens, out IExpression<T>? result)
    {
        throw new NotImplementedException();
    }

    private bool TryParseStringExpre(Token[] tokens, out IExpression<string> str)
    {
        throw new NotImplementedException();
    }

    private bool TryParseBooleanExpre(Token[] tokens, out IExpression<bool> b)
    {
        throw new NotImplementedException();
    }

    private bool TryGetFunctionExpre(Token[] tokens, out IExpression lineExpre)
    {
        throw new NotImplementedException();
    }

    private bool ResetDefaultExpre(int index, out IExpression? expre)
    {
        tokenIndex = index;
        expre = null;
        return false;
    }

    private bool ResetDefaultExpre<T>(int index, out IExpression<T>? expre)
    {

        tokenIndex = index;
        expre = null;
        return false;
    }

    private bool GetDefaultExpre(IExpression value, out IExpression expre)
    {
        expre = value;
        return true;
    }

    private bool GetDefaultExpre<T>(IExpression<T> value, out IExpression<T> expre)
    {
        expre = value;
        return true;
    }
}
using System.Data;
using PixelWall_E.Parser.src.Expressions;
using PixelWall_E.Lexer.src;
using Type = PixelWall_E.Lexer.src.Type;
using PixelWall_E.Parser.src.Expressions.Arithmetic;
using PixelWall_E.Parser.src.Expressions.Boolean;

namespace PixelWall_E.Parser.src.Parser;

public enum OperatorState { Shift, Reduce }

public class Parser
{
    int tokenIndex;
    readonly Dictionary<Type, OperatorState> ShiftOrReduceOperators = new(){
        { Type.Addition, OperatorState.Shift },
        { Type.Subtraction, OperatorState.Reduce },
        { Type.Multiplication, OperatorState.Shift },
        { Type.Division, OperatorState.Reduce },
        { Type.Remainder, OperatorState.Reduce },
        { Type.Exponentiation, OperatorState.Shift },
    };

    delegate bool TryGetFunc<T>(Token[] tokens, out IExpression<T>? expre);

    public IExpression Parse(Token[] tokens)
    {
        return TryGetBlockExpre(tokens, out IExpression? expre) ? expre! : throw new InvalidExpressionException();
    }


    private bool TryGetBlockExpre(Token[] tokens, out IExpression? expre)
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
                expressions.Add(lineExpre!);
            }
        } while (ReadLine);

        BlockExpression node = new([.. expressions]);
        return GetExpre(node, out expre);
    }

    private bool TryGetAssignExpre(Token[] tokens, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string name = tokens[tokenIndex].Value;
        if (!MatchToken(tokens, Type.Identifier) || !MatchToken(tokens, Type.Assign))
        {
            return ResetExpre(startIndex, out expre);
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

        return ResetExpre(startIndex, out expre);
    }

    private bool TryParseFunction<T>(Token[] tokens, out IExpression<T>? result)
    {
        throw new NotImplementedException();
    }

    private bool TryGetFunctionExpre(Token[] tokens, out IExpression? lineExpre)
    {
        return ResetExpre(tokenIndex, out lineExpre);
    }

    #region Arithmetic

    private bool TryParseArithExpre(Token[] tokens, out IExpression<int>? expression)
        => TryGetAddExpre(tokens, out expression);
    private bool TryGetAddExpre(Token[] tokens, out IExpression<int>? expre)
        => ShiftExpre(tokens, TryGetProduct, out expre, [Type.Addition, Type.Subtraction]);
    private bool TryGetProduct(Token[] tokens, out IExpression<int>? expre)
        => ShiftExpre(tokens, TryGetPow, out expre, [Type.Multiplication, Type.Division, Type.Remainder]);
    private bool TryGetPow(Token[] tokens, out IExpression<int>? expre)
        => ShiftExpre(tokens, TryGetNum, out expre, [Type.Exponentiation]);
    private bool TryGetNum(Token[] tokens, out IExpression<int>? expre)
        => TryGetLiteral(tokens, Type.Number, TryParseArithExpre, out expre);

    private bool TryGetLiteral<T>(Token[] tokens, Type literalType, TryGetFunc<T> tryGetFunc, out IExpression<T>? expre) where T : IParsable<T>
    {
        int startIndex = tokenIndex;
        if (MatchToken(tokens, literalType)
            && T.TryParse(tokens[startIndex].Value, null, out T? literal))
        {
            return GetDefaultExpre(CreateIntance(literal), out expre);
        }
        else if (MatchToken(tokens, Type.Identifier)
            && TryParseFunction(tokens, out IExpression<T>? result))
        {
            return GetDefaultExpre(result!, out expre);
        }
        else if (MatchToken(tokens, Type.LeftBracket)
            && tryGetFunc(tokens, out result)
            && MatchToken(tokens, Type.RightBracket))
        {
            return GetDefaultExpre(result!, out expre);
        }
        return ResetExpre(startIndex, out expre);
    }
    #endregion

    #region Boolean

    private bool TryParseBooleanExpre(Token[] tokens, out IExpression<bool> b)
    {
        return TryGetConcatenationOr(tokens, out b!);
    }

    private bool TryGetBool(Token[] tokens, out IExpression<bool>? expre)
    {
        int startIndex = tokenIndex;
        if (MatchToken(tokens, Type.Number))
        {
            BooleanLiteralExpre node = new(bool.Parse(tokens[tokenIndex - 1].Value));
            return GetDefaultExpre(node, out expre);
        }
        if (MatchToken(tokens, Type.Identifier)
            && TryParseFunction(tokens, out IExpression<bool>? result))
        {
            return GetDefaultExpre(result!, out expre);
        }
        if (MatchToken(tokens, Type.LeftBracket)
            && TryParseBooleanExpre(tokens, out result)
            && MatchToken(tokens, Type.RightBracket))
        {
            return GetDefaultExpre(result!, out expre);
        }
        return ResetExpre(startIndex, out expre);
    }

    private bool TryGetConcatenationOr(Token[] tokens, out IExpression<bool>? expre)
    {
        int startIndex = tokenIndex;
        if (!TryGetConcatenationAnd(tokens, out IExpression<bool>? left))
        {
            return ResetExpre(startIndex, out expre);
        }

        if (MatchToken(tokens, Type.Or)
            && TryGetConcatenationOr(tokens, out IExpression<bool>? right))
        {
            OrExpre node = new(left!, right!);
            return GetDefaultExpre(node, out expre);
        }

        return GetDefaultExpre(left!, out expre);
    }

    private bool TryGetConcatenationAnd(Token[] tokens, out IExpression<bool>? expre)
    {
        int startIndex = tokenIndex;
        if (!TryGetComplement(tokens, out IExpression<bool> left))
        {
            return ResetExpre(startIndex, out expre);
        }

        if (MatchToken(tokens, Type.And)
            && TryGetConcatenationAnd(tokens, out IExpression<bool>? right))
        {
            AndExpre node = new(left!, right!);
            return GetDefaultExpre(node, out expre);
        }

        return GetDefaultExpre(left!, out expre);
    }

    private bool TryGetComplement(Token[] tokens, out IExpression<bool> expre)
    {
        int startIndex = tokenIndex;
        if (MatchToken(tokens, Type.Complement)
            && TryGetComplement(tokens, out IExpression<bool> argument))
        {
            NotExpre node = new(argument);
            return GetDefaultExpre(node, out expre);
        }
        if (TryGetBool(tokens, out expre!))
        {
            return true;
        }
        return ResetExpre(startIndex, out expre!);
    }

    #endregion

    private bool TryParseStringExpre(Token[] tokens, out IExpression<string> str)
    {
        throw new NotImplementedException();
    }

    #region Tools

    private bool ShiftExpre<T>(Token[] tokens, TryGetFunc<T> tryGetFunc, out IExpression<T>? expre, Type[] types)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression<T>? left))
            return ResetExpre(startIndex, out expre);
        if (!ReduceExpre(tokens, tryGetFunc, left, out expre, types))
            return GetDefaultExpre(left!, out expre);
        return true;
    }

    private bool TryMatchOperator(Token[] tokens, Type[] types, out Type type)
    {
        foreach (var item in types)
        {
            if (!MatchToken(tokens, item))
                continue;
            type = item;
            return true;
        }
        type = Type.Unknown;
        return false;
    }

    private bool ReduceExpre<T>(Token[] tokens, TryGetFunc<T> tryGetFunc, IExpression<T>? left, out IExpression<T>? expre, Type[] types)
    {
        int startIndex = tokenIndex;
        if (!TryMatchOperator(tokens, types, out Type type))
            return ResetExpre(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && ShiftExpre(tokens, tryGetFunc, out IExpression<T>? right, types))
        {
            var node = CreateIntance(type, left, right);
            return GetDefaultExpre(node, out expre);
        }
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out right))
        {
            var node = CreateIntance(type, left, right);
            if (ReduceExpre(tokens, tryGetFunc, node, out IExpression<T>? result, types))
                return GetDefaultExpre(result!, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetExpre(startIndex, out expre);
    }

    private IExpression<T> CreateIntance<T>(Type type, IExpression<T>? left, IExpression<T>? right)
    {
        if (left is IExpression<int> num1 && right is IExpression<int> num2)
            return (CreateIntance(type, num1, num2) as IExpression<T>)!;
        throw new InvalidCastException();
    }

    private IExpression<T> CreateIntance<T>(Type type, IExpression<T>? argument)
    {
        if (argument is IExpression<int> num)
            return (CreateIntance(type, num) as IExpression<T>)!;
        throw new InvalidCastException();
    }

    private IExpression<T> CreateIntance<T>(T? literal)
    {

        return literal switch
        {
            int num => (new IntegerExpre(num) as IExpression<T>)!,
            _ => throw new Exception(),
        };
    }

    private IExpression<int> CreateIntance(Type typeOperator, IExpression<int>? left, IExpression<int> right)
    {
        return typeOperator switch
        {
            Type.Addition => new AdditionExpre(left!, right!),
            Type.Subtraction => new SubtractionExpre(left!, right!),
            Type.Multiplication => new MultiplicationExpre(left!, right!),
            Type.Division => new DivisionExpre(left!, right!),
            Type.Exponentiation => new ExponentiationExpre(left!, right!),
            Type.Remainder => new SubtractionExpre(left!, right!),
            _ => throw new InvalidCastException(),
        };
    }

    private bool TryAssignExpre<T>(string name, IExpression<T> value, out IExpression expre)
    {
        AssignExpre<T> node = new(name, value);
        return GetExpre(node, out expre);
    }

    private bool MatchToken(Token[] tokens, Type type)
    {
        if (tokens[tokenIndex].Type != type)
            return false;
        tokenIndex++;
        return true;
    }

    private bool ResetExpre(int index, out IExpression? expre)
    {
        tokenIndex = index;
        expre = null;
        return false;
    }

    private bool ResetExpre<T>(int index, out IExpression<T>? expre)
    {

        tokenIndex = index;
        expre = null;
        return false;
    }

    private bool GetExpre(IExpression value, out IExpression expre)
    {
        expre = value;
        return true;
    }

    private bool GetDefaultExpre<T>(IExpression<T> value, out IExpression<T> expre)
    {
        expre = value;
        return true;
    }

    #endregion
}
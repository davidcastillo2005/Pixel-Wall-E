using System.Data;
using PixelWallE.Parser.src.Expressions;
using PixelWallE.Lexer.src;
using Type = PixelWallE.Lexer.src.Type;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Extensions;

namespace PixelWallE.Parser.src.Parser;

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

    delegate bool TryGetFunc(Token[] tokens, out IExpression? expre);

    public IInstruction Parse(Token[] tokens)
    {
        return TryGetBlockExpre(tokens, out IInstruction? expre) ? expre! : throw new InvalidExpressionException();
    }


    private bool TryGetBlockExpre(Token[] tokens, out IInstruction? expre)
    {
        List<IInstruction> expressions = [];
        bool ReadLine;

        do
        {
            if (ReadLine = TryGetAssignExpre(tokens, out IInstruction? lineExpre))
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

    private bool TryGetAssignExpre(Token[] tokens, out IInstruction? expre)
    {
        int startIndex = tokenIndex;
        string name = tokens[tokenIndex].Value;
        if (!MatchToken(tokens, Type.Identifier) || !MatchToken(tokens, Type.Assign))
        {
            return ResetExpre(startIndex, out expre);
        }

        if (TryParseArithExpre(tokens, out IExpression? num))
        {
            return TryAssignExpre(name, num!, out expre);
        }
        if (TryParseBooleanExpre(tokens, out IExpression? b))
        {
            return TryAssignExpre(name, b, out expre);
        }
        if (TryParseStringExpre(tokens, out IExpression? str))
        {
            return TryAssignExpre(name, str, out expre);
        }

        return ResetExpre(startIndex, out expre);
    }

    private bool TryParseFunction(Token[] tokens, out IExpression? result)
    {
        throw new NotImplementedException();
    }

    private bool TryGetFunctionExpre(Token[] tokens, out IInstruction? lineExpre)
    {
        return ResetExpre(tokenIndex, out lineExpre);
    }

    #region Arithmetic

    private bool TryParseArithExpre(Token[] tokens, out IExpression? expression)
        => TryGetAddExpre(tokens, out expression);
    private bool TryGetAddExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetProduct, out expre, [Type.Addition, Type.Subtraction]);
    private bool TryGetProduct(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetPow, out expre, [Type.Multiplication, Type.Division, Type.Remainder]);
    private bool TryGetPow(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetNum, out expre, [Type.Exponentiation]);
    private bool TryGetNum(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, Type.Number, TryParseArithExpre, out expre);

    #endregion

    #region Boolean

    private bool TryParseBooleanExpre(Token[] tokens, out IExpression b) => TryGetOrExpre(tokens, out b!);

    private bool TryGetOrExpre(Token[] tokens, out IExpression? expre) => TryShiftBinaryExpre(tokens, TryGetAndExpre, out expre, [Type.Or]);

    private bool TryGetAndExpre(Token[] tokens, out IExpression? expre) => TryShiftBinaryExpre(tokens, TryGetComplement, out expre, [Type.And]);

    private bool TryGetComplement(Token[] tokens, out IExpression? expre) => TryShifttUnaryExpre(tokens, TryGetBool, out expre, [Type.Complement]);

    private bool TryGetBool(Token[] tokens, out IExpression? expre) =>
        TryGetLiteral(tokens, Type.Boolean, TryParseBooleanExpre, out expre);

    #endregion

    private bool TryParseStringExpre(Token[] tokens, out IExpression str)
    {
        throw new NotImplementedException();
    }

    #region Tools

    private bool TryShiftBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, Type[] types)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? left))
            return ResetExpre(startIndex, out expre);
        if (!TryReduceBinaryExpre(tokens, tryGetFunc, left, out expre, types))
            return GetDefaultExpre(left!, out expre);
        return true;
    }

    private bool TryShifttUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, Type[] types)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? argument))
            return ResetExpre(startIndex, out expre);
        if (!TryReduceUnaryExpre(tokens, tryGetFunc, argument, out expre, types))
            return GetDefaultExpre(argument!, out expre);
        return true;
    }

    private bool TryReduceBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? left, out IExpression? expre, Type[] types)
    {
        int startIndex = tokenIndex;
        if (!TryMatchTokenType(tokens, types, out Type type))
            return ResetExpre(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShiftBinaryExpre(tokens, tryGetFunc, out IExpression? right, types))
            return GetDefaultExpre(new BinaryExpre(left!, right!, type.ToBinaryType()), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out right))
        {
            IExpression node = new BinaryExpre(left!, right!, type.ToBinaryType());
            if (TryReduceBinaryExpre(tokens, tryGetFunc, node, out IExpression? result, types))
                return GetDefaultExpre(result!, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetExpre(startIndex, out expre);
    }

    private bool TryReduceUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? argument, out IExpression? expre, Type[] types)
    {
        int startIndex = tokenIndex;
        if (!TryMatchTokenType(tokens, types, out Type type))
            return ResetExpre(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShifttUnaryExpre(tokens, tryGetFunc, out argument, types))
            return GetDefaultExpre(new UnaryExpre(argument!, type.ToUnaryType()), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out argument))
        {
            IExpression node = new UnaryExpre(argument!, type.ToUnaryType());
            if (TryReduceUnaryExpre(tokens, tryGetFunc, node, out IExpression? result, types))
                return GetDefaultExpre(node, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetExpre(startIndex, out expre);
    }

    private bool TryGetLiteral(Token[] tokens, Type literalType, TryGetFunc tryGetFunc, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        if (MatchToken(tokens, literalType)
            && ValueType.TryParse(tokens[startIndex].Value, null, out ValueType? literal))
        {
            return GetDefaultExpre(new LiteralExpre(literal), out expre);
        }
        else if (MatchToken(tokens, Type.Identifier)
            && TryParseFunction(tokens, out IExpression? result))
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


    private bool TryMatchTokenType(Token[] tokens, Type[] types, out Type type)
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

    private bool TryAssignExpre(string name, IExpression value, out IInstruction expre)
    {
        AssignExpre node = new(name, value);
        return GetExpre(node, out expre);
    }

    private bool MatchToken(Token[] tokens, Type type)
    {
        if (tokens[tokenIndex].Type != type)
            return false;
        tokenIndex++;
        return true;
    }

    private bool ResetExpre(int index, out IInstruction? expre)
    {
        tokenIndex = index;
        expre = null;
        return false;
    }

    private bool ResetExpre(int index, out IExpression? expre)
    {

        tokenIndex = index;
        expre = null;
        return false;
    }

    private bool GetExpre(IInstruction value, out IInstruction expre)
    {
        expre = value;
        return true;
    }

    private bool GetDefaultExpre(IExpression value, out IExpression expre)
    {
        expre = value;
        return true;
    }

    #endregion
}
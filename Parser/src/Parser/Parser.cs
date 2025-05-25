using System.Data;
using PixelWallE.Parser.src.Expressions;
using PixelWallE.Lexer.src;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Extensions;

namespace PixelWallE.Parser.src.Parser;

public enum OperatorState { Shift, Reduce }

public class Parser
{
    private int tokenIndex;
    private readonly Dictionary<TokenType, OperatorState> ShiftOrReduceOperators = new()
    {
        { TokenType.Plus, OperatorState.Shift }, { TokenType.Minus, OperatorState.Reduce },
        { TokenType.Multiplication, OperatorState.Shift }, { TokenType.Division, OperatorState.Reduce },
        { TokenType.Modulus, OperatorState.Reduce }, { TokenType.Exponentiation, OperatorState.Shift },
        { TokenType.Equal, OperatorState.Reduce }, { TokenType.NotEqual, OperatorState.Reduce },
        { TokenType.Greater, OperatorState.Reduce }, { TokenType.GreaterOrEqual, OperatorState.Reduce },
        { TokenType.Less, OperatorState.Reduce }, { TokenType.LessOrEqual, OperatorState.Reduce },
    };

    delegate bool TryGetFunc(Token[] tokens, out IExpression? expre);

    public IInstruction Parse(Token[] tokens) => TryGetBlockExpre(tokens, out IInstruction? expre) ? expre! : throw new InvalidExpressionException();

    private bool TryGetBlockExpre(Token[] tokens, out IInstruction? expre)
    {
        List<IInstruction> expressions = [];
        bool ReadLine;
        do
        {
            //TODO Cambiar el nombre a todos loa AssignExpre a AssignInst.
            //TODO Cambiar todas los nombres expression a instruction.

            if (ReadLine = TryGetAssignExpre(tokens, out IInstruction? lineExpre)
                || TryGetGoToInstr(tokens, out lineExpre)
                || TryGetFunctionExpre(tokens, out lineExpre)
                || TryGetLabelInst(expressions.Count, tokens, out lineExpre))
            {
                expressions.Add(lineExpre!);
                continue;
            }
            ReadLine = MatchToken(tokens, TokenType.NewLine);
        } while (ReadLine);

        BlockExpression node = new([.. expressions]);
        return GetDefaultExpre(node, out expre);
    }

    private bool TryGetGoToInstr(Token[] tokens, out IInstruction? lineExpre)
    {
        int startIndex = tokenIndex;
        //TODO Crear MatchToken para listas de token.
        if (MatchToken(tokens, TokenType.GoTo)
            && MatchToken(tokens, TokenType.LeftBracket)
            && MatchToken(tokens, TokenType.Identifier)
            && MatchToken(tokens, TokenType.RightBracket))
        {
            var valueIndex = tokenIndex - 2;
            string targetLabel = tokens[valueIndex].Value;
            Instruction instr;
            if (MatchToken(tokens, TokenType.LeftCurly)
                && TryParseBooleanExpre(tokens, out IExpression cond)
                && MatchToken(tokens, TokenType.RightCurly))
            {
                instr = new GoToInst(targetLabel, cond);
            }
            else
            {
                instr = new GoToInst(targetLabel, null);
            }
            return GetDefaultExpre(instr, out lineExpre);
        }
        return ResetExpre(startIndex, out lineExpre);
    }

    private bool TryGetLabelInst(int lineIndex, Token[] tokens, out IInstruction? lineExpre)
    {
        int startIndex = tokenIndex;
        string value = tokens[tokenIndex].Value;
        if (!MatchToken(tokens, TokenType.Identifier) || !MatchToken(tokens, TokenType.NewLine))
            return ResetExpre(startIndex, out lineExpre);
        Instruction instr = new LabelInst(value, lineIndex);
        return GetDefaultExpre(instr, out lineExpre);
    }

    private bool TryGetAssignExpre(Token[] tokens, out IInstruction? expre)
    {
        int startIndex = tokenIndex;
        if (!MatchToken(tokens, TokenType.Identifier) || !MatchToken(tokens, TokenType.Assign) || !TryParseExpre(tokens, out IExpression? value))
            return ResetExpre(startIndex, out expre);
        return TryAssignExpre(tokens[startIndex].Value, value!, out expre);
    }

    private bool TryParseExpre(Token[] tokens, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string name = tokens[tokenIndex].Value;
        List<TryGetFunc> list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];

        foreach (var func in list)
        {
            if (func(tokens, out IExpression? value) && MatchToken(tokens, TokenType.NewLine))
                return GetDefaultExpre(value!, out expre);
            tokenIndex = startIndex;
        }

        return ResetExpre(startIndex, out expre);
    }

    #region Methods

    private bool TryParseFunction(Token[] tokens, out IExpression? expre, string name)
    {
        int startIndex = tokenIndex;
        if (MatchToken(tokens, TokenType.Identifier)
            && MatchToken(tokens, TokenType.LeftCurly)
            && TryGetParams(tokens, out IExpression[]? parameters))
        {
            expre = new CallableExpre(name, parameters!);
            return true;
        }
        return ResetExpre(startIndex, out expre);
    }

    private bool TryGetParams(Token[] tokens, out IExpression[]? parameters)
    {
        int startIndex = tokenIndex;
        List<TryGetFunc> list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];
        List<IExpression> paramList = [];
        do
        {
            int paramIndex = tokenIndex;
            foreach (var func in list)
            {
                if (paramList.Count == 0 && func(tokens, out IExpression? value))
                {
                    paramList.Add(value!);
                    break;
                }
                else if (MatchToken(tokens, TokenType.Comma) && func(tokens, out value))
                {
                    paramList.Add(value!);
                    break;
                }
                tokenIndex = paramIndex;
            }
            if (paramIndex == tokenIndex)
            {
                return ResetExpre(startIndex, out parameters);
            }
        } while (!MatchToken(tokens, TokenType.RightCurly));

        return GetDefaultExpre([.. paramList], out parameters);
    }

    private bool TryGetFunctionExpre(Token[] tokens, out IInstruction? lineExpre)
        => ResetExpre(tokenIndex, out lineExpre);

    #endregion

    #region Arithmetic

    private bool TryParseArithExpre(Token[] tokens, out IExpression? expression)
        => TryGetAddExpre(tokens, out expression);
    private bool TryGetAddExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetProduct, out expre, [BinaryType.Add, BinaryType.Subtract]);
    private bool TryGetProduct(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetPow, out expre, [BinaryType.Multiply, BinaryType.Divide, BinaryType.Modulus]);
    private bool TryGetPow(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetNum, out expre, [BinaryType.Power]);
    private bool TryGetNum(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, LiteralType.Integer, TryParseArithExpre, out expre);

    #endregion

    #region Boolean

    private bool TryParseBooleanExpre(Token[] tokens, out IExpression b) => TryGetOrExpre(tokens, out b!);

    private bool TryGetOrExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetAndExpre, out expre, [BinaryType.Or]);

    private bool TryGetAndExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetComparator, out expre, [BinaryType.And]);

    private bool TryGetComparator(Token[] tokens, out IExpression? expre) => TryShiftBinaryExpre(tokens, TryMatchComp, out expre,
    [
        BinaryType.Equal,
        BinaryType.NotEqual,
        BinaryType.GreaterThan,
        BinaryType.GreaterOrEqualThan,
        BinaryType.LessThan,
        BinaryType.LessOrEqualThan
    ]);

    private bool TryMatchComp(Token[] tokens, out IExpression? expre) =>
        TryParseArithExpre(tokens, out expre) || TryGetComplement(tokens, out expre)
        || TryParseStringExpre(tokens, out expre);

    private bool TryGetComplement(Token[] tokens, out IExpression? expre)
        => TryShifttUnaryExpre(tokens, TryGetBool, out expre, [TokenType.Not]);

    private bool TryGetBool(Token[] tokens, out IExpression? expre) =>
        TryGetLiteral(tokens, LiteralType.Boolean, TryParseBooleanExpre, out expre);

    #endregion

    #region Strings

    private bool TryParseStringExpre(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, LiteralType.String, null, out expre);

    #endregion

    #region Tools

    private bool TryShiftBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, BinaryType[] op)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? left))
            return ResetExpre(startIndex, out expre);
        if (!TryReduceBinaryExpre(tokens, tryGetFunc, left, out expre, op))
            return GetDefaultExpre(left!, out expre);
        return true;
    }

    private bool TryShifttUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] types)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? argument))
            return ResetExpre(startIndex, out expre);
        if (!TryReduceUnaryExpre(tokens, tryGetFunc, argument, out expre, types))
            return GetDefaultExpre(argument!, out expre);
        return true;
    }

    private bool TryReduceBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? left, out IExpression? expre, BinaryType[] op)
    {
        int startIndex = tokenIndex;
        if (!TryMatchTokenType(tokens, op, out TokenType type))
            return ResetExpre(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShiftBinaryExpre(tokens, tryGetFunc, out IExpression? right, op))
            return GetDefaultExpre(new BinaryExpre(left!, right!, type.ToBinaryType()), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out right))
        {
            IExpression node = new BinaryExpre(left!, right!, type.ToBinaryType());
            if (TryReduceBinaryExpre(tokens, tryGetFunc, node, out IExpression? result, op))
                return GetDefaultExpre(result!, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetExpre(startIndex, out expre);
    }

    private bool TryReduceUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? argument, out IExpression? expre, TokenType[] types)
    {
        int startIndex = tokenIndex;

        if (!TryMatchTokenType(tokens, types, out TokenType type))
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

    private bool TryGetLiteral(Token[] tokens, LiteralType literalType, TryGetFunc? tryGetFunc, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        IExpression? result;
        if (MatchToken(tokens, literalType)
            && Result.TryParse(tokens[startIndex].Value, null, out Result? literal))
            return GetDefaultExpre(new LiteralExpre(literal), out expre);
        else if (MatchToken(tokens, TokenType.LeftCurly)
            && tryGetFunc is not null && tryGetFunc(tokens, out result)
            && MatchToken(tokens, TokenType.RightCurly))
            return GetDefaultExpre(result!, out expre);
        else if (TryParseFunction(tokens, out result, tokens[startIndex].Value))
            return GetDefaultExpre(result!, out expre);
        else if (MatchToken(tokens, TokenType.Identifier))
            return GetDefaultExpre(new VariableExpre(tokens[startIndex].Value), out expre);
        return ResetExpre(startIndex, out expre);
    }

    private bool TryMatchTokenType(Token[] tokens, TokenType[] types, out TokenType type)
    {
        foreach (var item in types)
        {
            if (!MatchToken(tokens, item))
                continue;
            type = item;
            return true;
        }
        type = TokenType.Unknown;
        return false;
    }

    private bool TryMatchTokenType(Token[] tokens, BinaryType[] op, out TokenType type)
    {
        foreach (BinaryType item in op)
        {
            if (!MatchToken(tokens, item))
                continue;
            type = item.ToTokenType();
            return true;
        }
        type = TokenType.Unknown;
        return false;
    }

    private static bool TryAssignExpre(string name, IExpression value, out IInstruction expre)
        => GetDefaultExpre(new AssignExpre(name, value), out expre);

    private bool MatchToken(Token[] tokens, TokenType[] types, out TokenType? matchedType)
    {
        foreach (var item in types)
        {
            if (tokens[tokenIndex].Type == item)
            {
                matchedType = item;
                return true;
            }
        }
        matchedType = null;
        return false;
    }

    private bool MatchToken<T>(Token[] tokens, T type)
    {
        switch (type)
        {
            case TokenType token:
                if (tokens[tokenIndex].Type != token)
                    return false;
                break;
            case BinaryType binOp:
                if (tokens[tokenIndex].Type != binOp.ToTokenType())
                    return false;
                break;
            case UnaryType unaOp:
                if (tokens[tokenIndex].Type != unaOp.ToTokenType())
                    return false;
                break;
            case LiteralType lit:
                if (tokens[tokenIndex].Type != lit.ToTokenType())
                    return false;
                break;
            default:
                throw new Exception();
        }
        tokenIndex++;
        return true;
    }

    private bool ResetExpre<T>(int index, out T? expre)
    {
        tokenIndex = index;
        expre = default;
        return false;
    }

    private static bool GetDefaultExpre<T>(T value, out T expre)
    {
        expre = value;
        return true;
    }

    #endregion
}
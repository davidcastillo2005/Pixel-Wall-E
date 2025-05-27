using System.Data;
using PixelWallE.Lexer.src;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Extensions;
using PixelWallE.Parser.src.AST;

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

    public IStatement Parse(Token[] tokens) => TryGetBlockExpre(tokens, out IStatement? expre) ? expre! : throw new InvalidExpressionException();

    private bool TryGetBlockExpre(Token[] tokens, out IStatement? expre)
    {
        List<IStatement> expressions = [];
        bool ReadLine;
        do
        {
            //TODO Cambiar el nombre a todos loa AssignExpre a AssignInst.
            //TODO Cambiar todas los nombres expression a instruction.

            if (ReadLine = TryGetAssignExpre(tokens, out IStatement? lineExpre)
                || TryGetGoToInstr(tokens, out lineExpre)
                || TryGetFunctionExpre(tokens, out lineExpre)
                || TryGetLabelInst(expressions.Count, tokens, out lineExpre))
            {
                expressions.Add(lineExpre!);
                continue;
            }
            ReadLine = TryMatchToken(tokens, TokenType.NewLine);
        } while (ReadLine);

        Block node = new([.. expressions]);
        return GetDefaultExpre(node, out expre);
    }

    private bool TryGetGoToInstr(Token[] tokens, out IStatement? lineExpre)
    {
        int startIndex = tokenIndex;
        if (TryMatchAllTokens(tokens,
            [
                TokenType.GoTo,
                TokenType.LeftBracket,
                TokenType.Identifier,
                TokenType.RightBracket
            ]))
        {
            var valueIndex = tokenIndex - 2;
            string targetLabel = tokens[valueIndex].Value;
            StatementNode instr;
            if (TryMatchToken(tokens, TokenType.LeftCurly)
                && TryParseBooleanExpre(tokens, out IExpression cond)
                && TryMatchToken(tokens, TokenType.RightCurly))
            {
                instr = new GoToStmntNode(targetLabel, cond);
            }
            else
            {
                instr = new GoToStmntNode(targetLabel, null);
            }
            return GetDefaultExpre(instr, out lineExpre);
        }
        return ResetTokenIndex(startIndex, out lineExpre);
    }

    private bool TryGetLabelInst(int lineIndex, Token[] tokens, out IStatement? lineExpre)
    {
        int startIndex = tokenIndex;
        string value = tokens[tokenIndex].Value;
        if (!TryMatchAllTokens(tokens, [TokenType.Identifier, TokenType.NewLine]))
            return ResetTokenIndex(startIndex, out lineExpre);
        StatementNode instr = new LabelStmntNode(value, lineIndex);
        return GetDefaultExpre(instr, out lineExpre);
    }

    private bool TryGetAssignExpre(Token[] tokens, out IStatement? expre)
    {
        int startIndex = tokenIndex;
        if (!(TryMatchAllTokens(tokens, [TokenType.Identifier, TokenType.Assign])
            && TryParseExpre(tokens, out IExpression? value)))
            return ResetTokenIndex(startIndex, out expre);
        return TryAssignExpre(tokens[startIndex].Value, value!, out expre);
    }

    private bool TryParseExpre(Token[] tokens, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        string name = tokens[tokenIndex].Value;
        List<TryGetFunc> list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];

        foreach (var func in list)
        {
            if (func(tokens, out IExpression? value) && TryMatchToken(tokens, TokenType.NewLine))
                return GetDefaultExpre(value!, out expre);
            tokenIndex = startIndex;
        }

        return ResetTokenIndex(startIndex, out expre);
    }

    #region Methods

    private bool TryParseFunction(Token[] tokens, out IExpression? expre, string name)
    {
        int startIndex = tokenIndex;
        if (TryMatchAllTokens(tokens, [TokenType.Identifier, TokenType.LeftCurly])
            && TryGetParams(tokens, out IExpression[]? parameters))
        {
            expre = new CallableExpreNode(name, parameters!);
            return true;
        }
        return ResetTokenIndex(startIndex, out expre);
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
                else if (TryMatchToken(tokens, TokenType.Comma) && func(tokens, out value))
                {
                    paramList.Add(value!);
                    break;
                }
                tokenIndex = paramIndex;
            }
            if (paramIndex == tokenIndex)
            {
                return ResetTokenIndex(startIndex, out parameters);
            }
        } while (!TryMatchToken(tokens, TokenType.RightCurly));

        return GetDefaultExpre([.. paramList], out parameters);
    }

    private bool TryGetFunctionExpre(Token[] tokens, out IStatement? lineExpre)
        => ResetTokenIndex(tokenIndex, out lineExpre);

    #endregion

    #region Arithmetic

    private bool TryParseArithExpre(Token[] tokens, out IExpression? expression)
        => TryGetAddExpre(tokens, out expression);
    private bool TryGetAddExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetProduct, out expre, [BinaryOps.Add, BinaryOps.Subtract]);
    private bool TryGetProduct(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetPow, out expre, [BinaryOps.Multiply, BinaryOps.Divide, BinaryOps.Modulus]);
    private bool TryGetPow(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetNum, out expre, [BinaryOps.Power]);
    private bool TryGetNum(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, Literals.Integer, TryParseArithExpre, out expre);

    #endregion

    #region Boolean

    private bool TryParseBooleanExpre(Token[] tokens, out IExpression b) => TryGetOrExpre(tokens, out b!);

    private bool TryGetOrExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetAndExpre, out expre, [BinaryOps.Or]);

    private bool TryGetAndExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetComparator, out expre, [BinaryOps.And]);

    private bool TryGetComparator(Token[] tokens, out IExpression? expre) => TryShiftBinaryExpre(tokens, TryMatchComp, out expre,
    [
        BinaryOps.Equal,
        BinaryOps.NotEqual,
        BinaryOps.GreaterThan,
        BinaryOps.GreaterOrEqualThan,
        BinaryOps.LessThan,
        BinaryOps.LessOrEqualThan
    ]);

    private bool TryMatchComp(Token[] tokens, out IExpression? expre) =>
        TryParseArithExpre(tokens, out expre) || TryGetComplement(tokens, out expre)
        || TryParseStringExpre(tokens, out expre);

    private bool TryGetComplement(Token[] tokens, out IExpression? expre)
        => TryShifttUnaryExpre(tokens, TryGetBool, out expre, [TokenType.Not]);

    private bool TryGetBool(Token[] tokens, out IExpression? expre) =>
        TryGetLiteral(tokens, Literals.Boolean, TryParseBooleanExpre, out expre);

    #endregion

    #region Strings

    private bool TryParseStringExpre(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, Literals.String, null, out expre);

    #endregion

    #region Tools

    private bool TryShiftBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, BinaryOps[] op)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? left))
            return ResetTokenIndex(startIndex, out expre);
        if (!TryReduceBinaryExpre(tokens, tryGetFunc, left, out expre, op))
            return GetDefaultExpre(left!, out expre);
        return true;
    }

    private bool TryShifttUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] types)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? argument))
            return ResetTokenIndex(startIndex, out expre);
        if (!TryReduceUnaryExpre(tokens, tryGetFunc, argument, out expre, types))
            return GetDefaultExpre(argument!, out expre);
        return true;
    }

    private bool TryReduceBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? left, out IExpression? expre, BinaryOps[] op)
    {
        int startIndex = tokenIndex;
        if (!TryGetMatchToken(tokens, op, out TokenType type))
            return ResetTokenIndex(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShiftBinaryExpre(tokens, tryGetFunc, out IExpression? right, op))
            return GetDefaultExpre(new BinaryExpreNode(left!, right!, type.ToBinaryType()), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out right))
        {
            IExpression node = new BinaryExpreNode(left!, right!, type.ToBinaryType());
            if (TryReduceBinaryExpre(tokens, tryGetFunc, node, out IExpression? result, op))
                return GetDefaultExpre(result!, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryReduceUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? argument, out IExpression? expre, TokenType[] types)
    {
        int startIndex = tokenIndex;

        if (!TryGetMatchToken(tokens, types, out TokenType type))
            return ResetTokenIndex(startIndex, out expre);

        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShifttUnaryExpre(tokens, tryGetFunc, out argument, types))
            return GetDefaultExpre(new UnaryExpreNode(argument!, type.ToUnaryType()), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out argument))
        {
            IExpression node = new UnaryExpreNode(argument!, type.ToUnaryType());
            if (TryReduceUnaryExpre(tokens, tryGetFunc, node, out IExpression? result, types))
                return GetDefaultExpre(node, out expre);
            return GetDefaultExpre(node, out expre);
        }

        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryGetLiteral(Token[] tokens, Literals literalType, TryGetFunc? tryGetFunc, out IExpression? expre)
    {
        int startIndex = tokenIndex;
        IExpression? result;
        if (TryMatchToken(tokens, literalType)
            && Result.TryParse(tokens[startIndex].Value, null, out Result? literal))
            return GetDefaultExpre(new LiteralExpre(literal), out expre);
        else if (TryMatchToken(tokens, TokenType.LeftCurly)
            && tryGetFunc is not null && tryGetFunc(tokens, out result)
            && TryMatchToken(tokens, TokenType.RightCurly))
            return GetDefaultExpre(result!, out expre);
        else if (TryParseFunction(tokens, out result, tokens[startIndex].Value))
            return GetDefaultExpre(result!, out expre);
        else if (TryMatchToken(tokens, TokenType.Identifier))
            return GetDefaultExpre(new VariableExpreNode(tokens[startIndex].Value), out expre);
        return ResetTokenIndex(startIndex, out expre);
    }

    //TODO Buscar la manera de unir los dos metodos TryGetMatchToken ya que son muy parecidos.
    private bool TryGetMatchToken(Token[] tokens, TokenType[] types, out TokenType type)
    {
        foreach (TokenType item in types)
        {
            if (!TryMatchToken(tokens, item))
                continue;
            type = item;
            return true;
        }
        type = TokenType.Unknown;
        return false;
    }

    private bool TryGetMatchToken(Token[] tokens, BinaryOps[] op, out TokenType type)
    {
        foreach (BinaryOps item in op)
        {
            if (!TryMatchToken(tokens, item))
                continue;
            type = item.ToTokenType();
            return true;
        }
        type = TokenType.Unknown;
        return false;
    }
    // private bool TryGetMatchToken<T>(Token[] tokens, T[] types, out TokenType matchedType) where T : IConvertibleTokenType
    // {
    //     if (typeof(T) == typeof(BinaryOps))
    //     {
    //         var binOpsArr = types as BinaryOps[];
    //         if (binOpsArr is not null)
    //         {
    //             foreach (var item in binOpsArr)
    //             {
    //                 var typeToCompare = item.ToTokenType();
    //                 if (tokens[tokenIndex].Type == typeToCompare)
    //                 {
    //                     matchedType = typeToCompare;
    //                     return true;
    //                 }
    //             }
    //         }
    //     }
    //     matchedType = TokenType.Unknown;
    //     return false;
    // }

    private static bool TryAssignExpre(string name, IExpression value, out IStatement expre)
        => GetDefaultExpre(new AssignExpre(name, value), out expre);

    private bool TryMatchAllTokens(Token[] tokens, TokenType[] types)
    {
        int startIndex = tokenIndex;
        foreach (var item in types)
        {
            if (tokens[tokenIndex].Type != item)
            {
                tokenIndex = startIndex;
                return false;
            }
            tokenIndex++;
        }
        return true;
    }

    private bool TryMatchToken<T>(Token[] tokens, T type)
    {
        switch (type)
        {
            case TokenType token:
                if (tokens[tokenIndex].Type != token)
                    return false;
                break;
            case BinaryOps binOp:
                if (tokens[tokenIndex].Type != binOp.ToTokenType())
                    return false;
                break;
            case UnaryOps unaOp:
                if (tokens[tokenIndex].Type != unaOp.ToTokenType())
                    return false;
                break;
            case Literals lit:
                if (tokens[tokenIndex].Type != lit.ToTokenType())
                    return false;
                break;
            default:
                throw new Exception();
        }
        tokenIndex++;
        return true;
    }

    private bool ResetTokenIndex<T>(int index, out T? type)
    {
        tokenIndex = index;
        type = default;
        return false;
    }

    private static bool GetDefaultExpre<T>(T value, out T expre)
    {
        expre = value;
        return true;
    }

    #endregion
}
using System.Data;
using PixelWallE.Lexer.src;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Extensions;
using PixelWallE.Parser.src.AST;
using TokenType = PixelWallE.Lexer.src.Type;
using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src;

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

    public IStatement Parse(Token[] tokens) => TryGetCodeBlock(tokens, out IStatement? expre) ? expre! : throw new InvalidExpressionException();

    private bool TryGetCodeBlock(Token[] tokens, out IStatement? expre)
    {
        List<IStatement> statements = [];
        bool ReadLine;
        do
        {
            if (ReadLine = TryGetAssignExpre(tokens, out IStatement? line)
                || TryGetGoToStmnt(tokens, out line)
                || TryGetFunctionExpre(tokens, out line)
                || TryGetLabelInst(statements.Count, tokens, out line))
            {
                statements.Add(line!);
                continue;
            }
            ReadLine = TryMatchToken(tokens, TokenType.NewLine);
        } while (ReadLine);

        CodeBlock node = new([.. statements]);
        return GetDefaultExpre(node, out expre);
    }
    private bool TryGetGoToStmnt(Token[] tokens, out IStatement? lineExpre)
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
            IStatement instr;
            if (TryMatchToken(tokens, TokenType.LeftCurly)
                && TryParseBooleanExpre(tokens, out IExpression cond)
                && TryMatchToken(tokens, TokenType.RightCurly))
            {
                instr = new GoToStmnt(targetLabel, cond);
            }
            else
            {
                instr = new GoToStmnt(targetLabel, null);
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
        IStatement instr = new LabelStmnt(value, lineIndex);
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
        TryGetFunc[] list = [TryParseBooleanExpre, TryParseArithExpre, TryParseStringExpre];

        foreach (var func in list)
        {
            if (func(tokens, out IExpression? value) && TryMatchToken(tokens, TokenType.NewLine))
                return GetDefaultExpre(value!, out expre);
            tokenIndex = startIndex;
        }

        return ResetTokenIndex(startIndex, out expre);
    }

    #region Methods

    private bool TryParseMethod(Token[] tokens, out IExpression[]? parameters)
    {
        int startIndex = tokenIndex;
        if (!TryMatchAllTokens(tokens, [TokenType.Identifier, TokenType.LeftCurly])
            || !TryGetParams(tokens, out parameters))
        {
            return ResetTokenIndex(startIndex, out parameters);
        }
        return true;
    }

    private bool TryGetAction(Token[] tokens, out IStatement? statement)
    {
        string identifier = tokens[tokenIndex].Value;
        if (TryParseMethod(tokens, out IExpression[]? parameters))
        {
            statement = new AST.Action(identifier, parameters!);
            return true;
        }
        return ResetTokenIndex(tokenIndex, out statement);
    }

    private bool TryGetFunction(Token[] tokens, out IExpression? expre)
    {
        string identifier = tokens[tokenIndex].Value;
        if (TryParseMethod(tokens, out IExpression[]? parameters))
        {
            expre = new Function(identifier, parameters!);
            return true;
        }
        return ResetTokenIndex(tokenIndex, out expre);
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
        => TryShiftBinaryExpre(tokens, TryGetProduct, out expre, [TokenType.Plus, TokenType.Minus]);
    private bool TryGetProduct(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetPow, out expre, [TokenType.Multiplication, TokenType.Division, TokenType.Modulus]);
    private bool TryGetPow(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetNum, out expre, [TokenType.Exponentiation]);
    private bool TryGetNum(Token[] tokens, out IExpression? expre)
        => TryGetLiteral(tokens, Literals.Integer, TryParseArithExpre, out expre);

    #endregion

    #region Boolean

    private bool TryParseBooleanExpre(Token[] tokens, out IExpression b) => TryGetOrExpre(tokens, out b!);

    private bool TryGetOrExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetAndExpre, out expre, [TokenType.Or]);

    private bool TryGetAndExpre(Token[] tokens, out IExpression? expre)
        => TryShiftBinaryExpre(tokens, TryGetComparator, out expre, [TokenType.And]);

    private bool TryGetComparator(Token[] tokens, out IExpression? expre) => TryShiftBinaryExpre(tokens, TryMatchComp, out expre,
    [
        TokenType.Equal,
        TokenType.NotEqual,
        TokenType.GreaterOrEqual,
        TokenType.Greater,
        TokenType.Less,
        TokenType.LessOrEqual
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

    private bool TryShiftBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? left))
            return ResetTokenIndex(startIndex, out expre);
        if (!TryReduceBinaryExpre(tokens, tryGetFunc, left, out expre, tokenTypes))
            return GetDefaultExpre(left!, out expre);
        return true;
    }

    private bool TryShifttUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!tryGetFunc(tokens, out IExpression? argument))
            return ResetTokenIndex(startIndex, out expre);
        if (!TryReduceUnaryExpre(tokens, tryGetFunc, argument, out expre, tokenTypes))
            return GetDefaultExpre(argument!, out expre);
        return true;
    }

    private bool TryReduceBinaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? left, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;
        if (!TryGetMatchToken(tokens, tokenTypes, out TokenType type))
            return ResetTokenIndex(startIndex, out expre);
        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShiftBinaryExpre(tokens, tryGetFunc, out IExpression? right, tokenTypes))
            return GetDefaultExpre(new BinaryExpreNode(left!, right!, type.ToBinaryType()), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out right))
        {
            IExpression node = new BinaryExpreNode(left!, right!, type.ToBinaryType());
            if (TryReduceBinaryExpre(tokens, tryGetFunc, node, out IExpression? result, tokenTypes))
                return GetDefaultExpre(result, out expre);
            return GetDefaultExpre(node, out expre);
        }
        return ResetTokenIndex(startIndex, out expre);
    }

    private bool TryReduceUnaryExpre(Token[] tokens, TryGetFunc tryGetFunc, IExpression? argument, out IExpression? expre, TokenType[] tokenTypes)
    {
        int startIndex = tokenIndex;

        if (!TryGetMatchToken(tokens, tokenTypes, out TokenType type))
            return ResetTokenIndex(startIndex, out expre);

        OperatorState operatorState = ShiftOrReduceOperators[type];
        if (operatorState == OperatorState.Shift && TryShifttUnaryExpre(tokens, tryGetFunc, out argument, tokenTypes))
            return GetDefaultExpre(new UnaryExpreNode(argument!, type.ToUnaryType()), out expre);
        if (operatorState == OperatorState.Reduce && tryGetFunc(tokens, out argument))
        {
            IExpression node = new UnaryExpreNode(argument!, type.ToUnaryType());
            if (TryReduceUnaryExpre(tokens, tryGetFunc, node, out IExpression? result, tokenTypes))
                return GetDefaultExpre(result, out expre);
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
        else if (TryGetFunction(tokens, out result))
            return GetDefaultExpre(result!, out expre);
        else if (TryMatchToken(tokens, TokenType.Identifier))
            return GetDefaultExpre(new VariableExpre(tokens[startIndex].Value), out expre);
        return ResetTokenIndex(startIndex, out expre);
    }

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

    private static bool TryAssignExpre(string name, IExpression value, out IStatement expre)
        => GetDefaultExpre(new AssignStmnt(name, value), out expre);

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
            case BinaryOperation binOp:
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
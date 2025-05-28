using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;
namespace PixelWallE.Parser.src.AST;

public class BinaryExpreNode(IExpression left, IExpression right, BinaryOperation opType) : IExpression
{
    public IExpression LeftArg { get; set; } = left;
    public IExpression RightArg { get; set; } = right;
    public BinaryOperation OperatorType { get; set; } = opType;

    public Result Accept(IVisitor visitor)
    {
        return visitor.BinaryVisit(LeftArg.Accept(visitor), OperatorType, RightArg.Accept(visitor));
    }
}

public class UnaryExpreNode(IExpression argument, UnaryOperation opType) : IExpression
{
    public IExpression Argument { get; set; } = argument;
    public UnaryOperation OperatorType { get; set; } = opType;

    public Result Accept(IVisitor visitor)
    {
        return visitor.UnaryVisit(Argument.Accept(visitor), OperatorType);
    }
}

public class VariableExpre(string identifier) : IExpression
{
    string Identifier { get; set; } = identifier;

    public Result Accept(IVisitor visitor)
    {
        return visitor.VariableVisit(Identifier);
    }
}

public class LiteralExpre(Result value) : IExpression
{
    public Result Value { get; set; } = value;
    public Result Accept(IVisitor visitor)
    {
        return visitor.LiteralVisit(Value);
    }
}
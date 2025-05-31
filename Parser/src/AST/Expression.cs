using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;
namespace PixelWallE.Parser.src.AST;

public class BinaryExpreNode(IExpression left, IExpression right, BinaryOperationType opType) : IExpression
{
    public IExpression LeftArg { get; set; } = left;
    public IExpression RightArg { get; set; } = right;
    public BinaryOperationType OperatorType { get; set; } = opType;

    public Result Accept(IVisitor visitor)
        => visitor.BinaryVisit(LeftArg.Accept(visitor), OperatorType, RightArg.Accept(visitor));
}

public class UnaryExpreNode(IExpression argument, UnaryOperationType opType) : IExpression
{
    public IExpression Argument { get; set; } = argument;
    public UnaryOperationType OperatorType { get; set; } = opType;

    public Result Accept(IVisitor visitor)
        => visitor.UnaryVisit(Argument.Accept(visitor), OperatorType);
}

public class VariableExpre(string identifier) : IExpression
{
    public string Identifier { get; set; } = identifier;

    public Result Accept(IVisitor visitor) => visitor.VariableVisit(Identifier);
}

public class LiteralExpre(Result value) : IExpression
{
    public Result Value { get; set; } = value;
    public Result Accept(IVisitor visitor) => visitor.LiteralVisit(Value);
}
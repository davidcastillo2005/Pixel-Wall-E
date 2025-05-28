using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;
namespace PixelWallE.Parser.src.AST;

public abstract class Expression : IExpression
{
    public abstract Result Accept(IVisitor visitor);
}

public class BinaryExpreNode(IExpression left, IExpression right, BinaryOps opType) : Expression
{
    public IExpression LeftArg { get; set; } = left;
    public IExpression RightArg { get; set; } = right;
    public BinaryOps OperatorType { get; set; } = opType;

    public override Result Accept(IVisitor visitor)
    {
        return visitor.BinaryVisit(LeftArg.Accept(visitor), OperatorType, RightArg.Accept(visitor));
    }
}

public class UnaryExpreNode(IExpression argument, UnaryOps opType) : Expression
{
    public IExpression Argument { get; set; } = argument;
    public UnaryOps OperatorType { get; set; } = opType;

    public override Result Accept(IVisitor visitor)
    {
        return visitor.UnaryVisit(Argument.Accept(visitor), OperatorType);
    }
}

public class VariableExpre(string identifier) : Expression
{
    string Identifier { get; set; } = identifier;

    public override Result Accept(IVisitor visitor)
    {
        return visitor.VariableVisit(Identifier);
    }
}

public class Function(string identifier, IExpression[] arguments) : Expression
{
    string Identifier { get; set; } = identifier;
    IExpression[] Arguments { get; set; } = arguments;

    public override Result Accept(IVisitor visitor)
    {
        return visitor.FuncVisit(Identifier, visitor.ParamsVisit(Arguments));
    }
}

public class LiteralExpre(Result value) : Expression
{
    public Result Value { get; set; } = value;
    public override Result Accept(IVisitor visitor)
    {
        return visitor.LiteralVisit(Value);
    }
}
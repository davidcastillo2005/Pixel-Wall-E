using PixelWallE.Lexer.src;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;
namespace PixelWallE.Parser.src.AST;

public class BinaryExpreNode(IExpression left, IExpression right, BinaryOperationType opType, Coord coord) : IExpression
{
    public IExpression LeftArg { get; set; } = left;
    public IExpression RightArg { get; set; } = right;
    public BinaryOperationType OperatorType { get; set; } = opType;
    public Coord Coord { get; set; } = coord;

    public Result Accept(IVisitor visitor)
        => visitor.BinaryVisit(LeftArg.Accept(visitor), OperatorType, RightArg.Accept(visitor), Coord);
}
public class UnaryExpreNode : IExpression
{
    public IExpression Argument { get; set; }
    public UnaryOperationType OperatorType { get; set; }
    public Coord Coord { get; set; }

    public UnaryExpreNode(IExpression argument, UnaryOperationType opType, Coord coord)
    {
        Argument = argument;
        OperatorType = opType;
        Coord = coord;
    }

    public Result Accept(IVisitor visitor)
        => visitor.UnaryVisit(Argument.Accept(visitor), OperatorType, Coord);
}

public class VariableExpre(string identifier, Coord coord) : IExpression
{
    public string Identifier { get; set; } = identifier;
    public Coord Coord { get; set; } = coord;

    public Result Accept(IVisitor visitor) => visitor.VariableVisit(Identifier, Coord);
}

public class LiteralExpre(Result value, Coord coord) : IExpression
{
    public Result Value { get; set; } = value;
    public Coord Coord { get; set; } = coord;
    public Result Accept(IVisitor visitor) => visitor.LiteralVisit(Value, Coord);
}
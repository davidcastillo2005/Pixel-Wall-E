using PixelWallE.Parser.src.Enums;

namespace PixelWallE.Parser.src.Expressions;

public interface IInstruction
{
    void Accept();
}

public interface IExpression : IInstruction
{
    new ValueType Accept();
}

public abstract class Instruction : IInstruction
{
    public abstract void Accept();
}

public abstract class Expression : IExpression
{
    public abstract ValueType Accept();

    void IInstruction.Accept() => Accept();
}

public class BinaryExpre(IExpression left, IExpression right, BinaryType opType) : Expression
{
    public IExpression LeftArgument { get; set; } = left;
    public IExpression RightArgument { get; set; } = right;
    public BinaryType OperatorType { get; set; } = opType;

    public override ValueType Accept() => OperatorType switch
    {
        BinaryType.Add => LeftArgument.Accept()! + RightArgument.Accept()!,
        BinaryType.Subtract => LeftArgument.Accept()! - RightArgument.Accept()!,
        BinaryType.Multiply => LeftArgument.Accept()! * RightArgument.Accept()!,
        BinaryType.Divide => LeftArgument.Accept()! / RightArgument.Accept()!,
        BinaryType.Power => LeftArgument.Accept()! ^ RightArgument.Accept()!,
        BinaryType.Modulus => LeftArgument.Accept()! % RightArgument.Accept()!,
        BinaryType.LessOrEqualThan => LeftArgument.Accept()! <= RightArgument.Accept()!,
        BinaryType.GreaterOrEqualThan => LeftArgument.Accept()! >= RightArgument.Accept()!,
        BinaryType.LessThan => LeftArgument.Accept()! < RightArgument.Accept()!,
        BinaryType.GreaterThan => LeftArgument.Accept()! > RightArgument.Accept()!,
        BinaryType.Equal => LeftArgument.Accept()! == RightArgument.Accept()!,
        BinaryType.NotEqual => LeftArgument.Accept()! != RightArgument.Accept()!,
        BinaryType.And => LeftArgument.Accept()! & RightArgument.Accept()!,
        BinaryType.Or => LeftArgument.Accept()! | RightArgument.Accept()!,
        _ => throw new NotImplementedException(),
    };
}

public class UnaryExpre(IExpression argument, UnaryType opType) : Expression
{
    public IExpression Argument { get; set; } = argument;
    public UnaryType OperatorType { get; set; } = opType;

    public override ValueType Accept()
    {
        throw new NotImplementedException();
    }
}
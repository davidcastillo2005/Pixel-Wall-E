using PixelWallE.Parser.src.Enums;

namespace PixelWallE.Parser.src.Expressions;

public interface IInstruction
{
    void Accept(Context context);
}

public interface IExpression : IInstruction
{
    new Result Accept(Context context);
}

public abstract class Instruction : IInstruction
{
    public abstract void Accept(Context context);
}

public abstract class Expression : IExpression
{
    public abstract Result Accept(Context context);

    void IInstruction.Accept(Context context)
    {
        Accept(context);
    }
}

public class BinaryExpre(IExpression left, IExpression right, BinaryType opType) : Expression
{
    public IExpression LeftArg { get; set; } = left;
    public IExpression RightArg { get; set; } = right;
    public BinaryType OperatorType { get; set; } = opType;

    public override Result Accept(Context context)
    {
        Result left = LeftArg.Accept(context);
        Result right = RightArg.Accept(context);
        return OperatorType switch
        {
            BinaryType.Add => left! + right!,
            BinaryType.Subtract => left! - right!,
            BinaryType.Multiply => left! * right!,
            BinaryType.Divide => left! / right!,
            BinaryType.Power => left! ^ right!,
            BinaryType.Modulus => left! % right!,
            BinaryType.LessOrEqualThan => left! <= right!,
            BinaryType.GreaterOrEqualThan => left! >= right!,
            BinaryType.LessThan => left! < right!,
            BinaryType.GreaterThan => left! > right!,
            BinaryType.Equal => left! == right!,
            BinaryType.NotEqual => left! != right!,
            BinaryType.And => left! & right!,
            BinaryType.Or => left! | right!,
            _ => throw new NotImplementedException(),
        };
    }
}

public class UnaryExpre(IExpression argument, UnaryType opType) : Expression
{
    public IExpression Argument { get; set; } = argument;
    public UnaryType OperatorType { get; set; } = opType;

    public override Result Accept(Context context)
    {
        Result argument = Argument.Accept(context);
        return OperatorType switch
        {
            UnaryType.Negative => -argument,
            UnaryType.Not => !argument,
            _ => throw new Exception(),
        };
    }
}

public class VariableExpre(string name) : Expression
{
    string Name { get; set; } = name;

    public override Result Accept(Context context)
    {
        context.Variables.TryGetValue(Name, out Result? result);
        if (result is not null)
            return result;
        else
            throw new Exception();
    }
}

public class CallableExpre(string name, IExpression[] parameters) : Expression
{
    string Name { get; set; } = name;
    IExpression[] Parameters { get; set; } = parameters;

    public override Result Accept(Context context)
    {
        var objects = new dynamic[Parameters.Length];
        for (int i = 0; i < Parameters.Length; i++)
        {
            objects[i] = Parameters[i].Accept(context).Value!;
        }

        var result = context.Functions[Name](objects);
        return new Result(result);
    }
}
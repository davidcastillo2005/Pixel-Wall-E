using PixelWallE.Parser.src.Enums;
namespace PixelWallE.Parser.src.AST;

public interface IExpression
{
    Result Accept(Context context);
}

public abstract class ExpressionNode : AstNode, IExpression
{
    public override void Accept()
    {
        Accept(context);
    }
    public override abstract Result Accept(Context context);
}

public class BinaryExpreNode(IExpression left, IExpression right, BinaryOps opType) : ExpressionNode
{
    public IExpression LeftArg { get; set; } = left;
    public IExpression RightArg { get; set; } = right;
    public BinaryOps OperatorType { get; set; } = opType;

    public override Result Accept(Context context)
    {
        Result left = LeftArg.Accept(context);
        Result right = RightArg.Accept(context);
        return OperatorType switch
        {
            BinaryOps.Add => left! + right!,
            BinaryOps.Subtract => left! - right!,
            BinaryOps.Multiply => left! * right!,
            BinaryOps.Divide => left! / right!,
            BinaryOps.Power => left! ^ right!,
            BinaryOps.Modulus => left! % right!,
            BinaryOps.LessOrEqualThan => left! <= right!,
            BinaryOps.GreaterOrEqualThan => left! >= right!,
            BinaryOps.LessThan => left! < right!,
            BinaryOps.GreaterThan => left! > right!,
            BinaryOps.Equal => left! == right!,
            BinaryOps.NotEqual => left! != right!,
            BinaryOps.And => left! & right!,
            BinaryOps.Or => left! | right!,
            _ => throw new NotImplementedException(),
        };
    }
}

public class UnaryExpreNode(IExpression argument, UnaryOps opType) : ExpressionNode
{
    public IExpression Argument { get; set; } = argument;
    public UnaryOps OperatorType { get; set; } = opType;

    public override Result Accept(Context context)
    {
        Result argument = Argument.Accept(context);
        return OperatorType switch
        {
            UnaryOps.Negative => -argument,
            UnaryOps.Not => !argument,
            _ => throw new Exception(),
        };
    }
}

public class VariableExpreNode(string name) : ExpressionNode
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

public class CallableExpreNode(string name, IExpression[] parameters) : ExpressionNode
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


public class LiteralExpre(Result value) : ExpressionNode
{
    public Result Value { get; set; } = value;
    public override Result Accept(Context context)
    {
        return Value;
    }
}
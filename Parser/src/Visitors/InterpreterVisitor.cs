using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src.Visitors;

public abstract class InterpreterVisitor(Context context) : IVisitor
{
    /*
    1- Variable
    2- Action
    3- Function
    3- Parameters
    4- Assign
    5- Literal
    6- Unary
    7- Binary
    8- Label
    9- Goto
    10- CodeBlock
    */
    private readonly Context context = context;

    public Context Context { get; set; } = context;
    public void ActionVisit(string identifier, Result[] arguments)
    {
        object[] objects = [.. arguments.Select(x => x.Value!)];
        context.Actions[identifier](objects);
    }

    public void GotoVisit(string targetLabel, Result? condition)
    {
        if (condition is not null)
        {
            bool cond = condition.ToBoolean();
            if (!cond)
                return;
        }
        Context.Jump(targetLabel);
    }

    public void AssignVisit(string identifier, Result value)
    {
        Context.Variables[identifier] = value;
    }

    public void LabelVisit(string identifier, int line)
    {
        throw new NotImplementedException();
    }

    public Result BinaryVisit(Result left, BinaryOperation op, Result right)
    {
        return op switch
        {
            BinaryOperation.Add => left! + right!,
            BinaryOperation.Subtract => left! - right!,
            BinaryOperation.Multiply => left! * right!,
            BinaryOperation.Divide => left! / right!,
            BinaryOperation.Power => left! ^ right!,
            BinaryOperation.Modulus => left! % right!,
            BinaryOperation.LessOrEqualThan => left! <= right!,
            BinaryOperation.GreaterOrEqualThan => left! >= right!,
            BinaryOperation.LessThan => left! < right!,
            BinaryOperation.GreaterThan => left! > right!,
            BinaryOperation.Equal => left! == right!,
            BinaryOperation.NotEqual => left! != right!,
            BinaryOperation.And => left! & right!,
            BinaryOperation.Or => left! | right!,
            _ => throw new NotImplementedException(),
        };
    }

    public Result[] ParametersVisit(IExpression[] expressions)
    {
        List<Result> results = [];
        foreach (var item in expressions)
        {
            results.Add(item.Accept(this));
        }
        return [.. results];
    }

    public Result FunctionVisit(string identifier, Result[] arguments)
    {
        object[] objects = [.. arguments.Select(x => x.Value!)];
        var result = context.Functions[identifier](objects);
        return new Result(result);
    }

    public Result LiteralVisit(Result value)
    {
        return value;
    }

    public Result UnaryVisit(Result argument, UnaryOperation op)
    {
        throw new NotImplementedException();
    }

    public Result VariableVisit(string identifier)
    {
        Context.Variables.TryGetValue(identifier, out Result? result);
        if (result is not null)
            return result;
        else
            throw new NotImplementedException();
    }

    public void Visit(IStatement statement)
    {
        statement.Accept(this);
    }

    public void CodeBlockVisit(IStatement[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].Accept(this);
            if (Context.IsJumping)
            {
                i = Context.Labels[Context.TargetLabel!];
                Context.EndJump();
            }
        }
    }
}
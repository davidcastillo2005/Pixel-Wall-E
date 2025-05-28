using PixelWallE.Parser.src.AST;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src.Visitors;

public abstract class StatementVisitor(Context context) : IVisitor
{
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
            bool cond = condition.ToBool();
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

    public Result BinaryVisit(Result left, BinaryOps op, Result right)
    {
        return op switch
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

    public Result[] ParamsVisit(IExpression[] expressions)
    {
        List<Result> results = [];
        foreach (var item in expressions)
        {
            results.Add(item.Accept(this));
        }
        return [.. results];
    }

    public Result FuncVisit(string identifier, Result[] arguments)
    {
        object[] objects = [.. arguments.Select(x => x.Value!)];
        var result = context.Functions[identifier](objects);
        return new Result(result);
    }

    public Result LiteralVisit(Result value)
    {
        return value;
    }

    public Result UnaryVisit(Result argument, UnaryOps op)
    {
        throw new NotImplementedException();
    }

    public Result VariableVisit(string identifier)
    {
        Context.Variables.TryGetValue(identifier, out Result? result);
        if (result is not null)
            return result;
        else
            throw new Exception();
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
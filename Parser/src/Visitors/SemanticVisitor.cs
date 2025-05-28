using PixelWallE.Parser.src.AST;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src.Visitors;

public class SemanticVisitor(Context context) : IVisitor
{
    private readonly Context context = context;
    public List<Exception> Exceptions { get; set; } = [];

    public void ActionVisit(string targetLabel, Result[] arguments)
    {
        throw new NotImplementedException();
    }

    public void AssignVisit(string identifier, Result value)
    {
        if (!context.Variables.ContainsKey(identifier))
        {
            //TODO Poner mensaje al error: Cuando tratas de asignar un valor a una variable no declarada.
            Exceptions.Add(new Exception($"Variable '{identifier}' not declared."));
        }
        else
        {
            context.Variables[identifier] = new Result(null, value.Type);
        }
    }

    public Result BinaryVisit(Result left, BinaryOps op, Result right)
    {
        if (left.Type != right.Type)
        {
            //TODO Poner mensaje al error: Cuando tratas de hacer una operacion con tipos diferentes.
            Exceptions.Add(new Exception($"Type mismatch: {left.Type} and {right.Type} cannot be used with operator {op}."));
            return new Result(null, value.Type);
        }
        else if (op == BinaryOps.Divide && right.Value == 0)
        {
            //TODO Poner mensaje al error: Cuando tratas de dividir por cero.
            Exceptions.Add(new Exception("Division by zero is not allowed."));
            return new Result(null, ResultType.Error);
        }
        else if (op == BinaryOps.Modulus && right.Value == 0)
        {
            //TODO Poner mensaje al error: Cuando tratas de hacer modulo por cero.
            Exceptions.Add(new Exception("Modulo by zero is not allowed."));
            return new Result(null, ResultType.Error);
        }
        else
        {
            // Assuming the operation is valid, return a new Result.
            return new Result(null, left.Type); // Placeholder for actual operation result.
        }
    }

    public void CodeBlockVisit(IStatement[] lines)
    {
        throw new NotImplementedException();
    }

    public Result FuncVisit(string identifier, Result[] arguments)
    {
        throw new NotImplementedException();
    }

    public void GotoVisit(string targetLabel, Result? condition)
    {
        throw new NotImplementedException();
    }

    //TODO Poner mensaje al error: Cuando tratas hacer un Visit a un label ya existente.
    public void LabelVisit(string identifier, int line)
    {
        if (context.Labels.ContainsKey(identifier))
            Exceptions.Add(new Exception(""));
    }

    public Result LiteralVisit(Result value)
    {
        return value;
    }

    public Result[] ParamsVisit(IExpression[] expressions)
    {
        throw new NotImplementedException();
    }

    public Result UnaryVisit(Result argument, UnaryOps op)
    {
        throw new NotImplementedException();
    }

    public Result VariableVisit(string identifier)
    {
        throw new NotImplementedException();
    }

    public void Visit(IStatement statement)
    {
        throw new NotImplementedException();
    }

    public void SearchLabel(IStatement[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] is LabelStmnt label)
            {
                label.Accept(this);
            }
        }
    }
}
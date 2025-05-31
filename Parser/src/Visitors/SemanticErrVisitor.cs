using PixelWallE.Parser.src.AST;
using PixelWallE.Parser.src.Enums;
using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src.Visitors;

public class SemanticErrVisitor(Context context) : IVisitor
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
    private readonly Context Context = context;
    public List<Exception> Exceptions { get; set; } = [];

    public void Visit(IStatement statement)
    {
        statement.Accept(this);
    }

    public Result VariableVisit(string identifier)
    {
        if (!Context.Variables.TryGetValue(identifier, out Result? value))
        {
            AddException("Variable '" + identifier + "' not declared.");
        }
        else if (HasNullResult(value))
        {
            AddException($"Variable '{identifier}' is null.");
        }
        return NullResult();
    }

    public void ActionVisit(string identifier, Result[] arguments)
    {
        if (!Context.Actions.ContainsKey(identifier))
        {
            AddException($"Method '{identifier}' not defined.");
        }
    }

    public Result FunctionVisit(string identifier, Result[] arguments)
    {
        if (!Context.Functions.ContainsKey(identifier))
        {
            AddException("Method '" + identifier + "' not defined.");
        }
        return NullResult();
    }

    public Result[] ParametersVisit(IExpression[] parameters)
    {
        List<Result> results = [];
        foreach (var item in parameters)
        {
            results.Add(item.Accept(this));
        }
        return [.. results];
    }

    public void AssignVisit(string identifier, Result value)
    {
        if (!Context.Variables.ContainsKey(identifier))
        {
            AddException($"Variable '{identifier}' not declared.");
        }
        else
        {
            Context.Variables[identifier] = NullResult();
        }
    }

    public Result LiteralVisit(Result value)
    {
        return NullResult();
    }

    public Result UnaryVisit(Result argument, UnaryOperationType op)
    {
        if (HasNullResult(argument))
        {
            AddException("Cannot perform unary operation with null value.");
            return NullResult();
        }
        else if (argument.Value is int argInt)
        {
            if (op == UnaryOperationType.Not)
            {
                AddException($"Cannot perform {op} operation on an integer.");
                return NullResult();
            }
        }
        else if (argument.Value is bool argBool)
        {
            //TODO Buscar casos de error semántico en que la variable es booleana y la operación es unaria.
        }
        else if (argument.Value is string argString)
        {
            //TODO Buscar casos de error semántico en que la variable es booleana y la operación es unaria.
        }
        return NullResult();
    }

    public Result BinaryVisit(Result left, BinaryOperationType op, Result right)
    {
        if (HasNullResult(left, right))
        {
            AddException("Cannot perform operation with null values.");
            return NullResult();
        }
        else if (left.Type != right.Type)
        {
            Exceptions.Add(new Exception($"Type mismatch: {left.Type} and {right.Type} cannot be used with operator {op}."));
            return NullResult();
        }
        else if (right.Value is int rInt)
        {
            if ((op == BinaryOperationType.Divide || op == BinaryOperationType.Modulus) && rInt == 0)
            {
                Exceptions.Add(new Exception("Division by zero is not allowed."));
                return NullResult();
            }
        }
        else if (right.Value is bool rBool)
        {
            //TODO Poner mensaje al error: Cuando tratas de hacer una operacion binaria con un booleano.
        }
        else if (right.Value is string rString)
        {
            //TODO Poner mensaje al error: Cuando tratas de hacer una operacion binaria con un string.
        }
        else
        {
            AddException($"Unsupported operation: {op} for types {right.Type}");
            return NullResult();
        }
        return NullResult();
    }

    public void LabelVisit(string identifier, int line)
    {
        if (Context.Labels.ContainsKey(identifier))
            AddException("Label '" + identifier + "' already exists.");
    }

    public void GotoVisit(string targetLabel, Result? condition)
    {
        if (Context.Labels.ContainsKey(targetLabel))
        {
            AddException("Label '" + targetLabel + "' not found.");
        }
    }

    public void CodeBlockVisit(IStatement[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            IStatement? item = lines[i];
            item.Accept(this);
        }
    }

    public void SearchLabel(IStatement[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] is LabelStatement label)
            {
                label.Accept(this);
            }
        }
    }

    #region Tools
    private bool HasNullResult(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result is null || result.Value is null)
                return true;
        }
        return false;
    }

    private void AddException(string message)
    {
        Exceptions.Add(new ParserException(message));
    }

    private void AddException(Exception e)
    {
        Exceptions.Add(e);
    }

    private Result NullResult()
    {
        return NullResult();
    }

    #endregion
}
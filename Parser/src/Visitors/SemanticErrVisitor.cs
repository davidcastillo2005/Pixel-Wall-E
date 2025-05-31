using PixelWallE.Lexer.src;
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
    public List<ParserException> Exceptions { get; set; } = [];

    public void Visit(IStatement statement, Coord coord) => statement.Accept(this);

    public Result VariableVisit(string identifier, Coord coord)
    {
        if (!Context.Variables.TryGetValue(identifier, out Result? value))
        {
            //TODO Error: Variable 'identifier' not declared.
            throw new Exception();
        }
        return (Result)GetResult(coord, value);
    }

    public void ActionVisit(string identifier, Result[] arguments, Coord coord)
    {
        if (Context.Handler.TryGetErrAction(identifier, arguments, this))
        {
            AddException(coord, "Wall-E is already spawned.");
        }
    }

    public Result FunctionVisit(string identifier, Result[] arguments, Coord coord)
    {
        if (Context.Handler.TryGetErrFunction(identifier, arguments, out Result result))
        {
            //TODO Error: Method 'identifier' not declared.
            throw new Exception();
        }
        return result;
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

    public void AssignVisit(string identifier, Result value, Coord coord)
    {
        if (!Context.Variables.ContainsKey(identifier))
        {
            //TODO Error: 
        }
        else
        {
            Context.Variables[identifier] = GetResult(coord, value);
        }
    }

    public Result LiteralVisit(Result value, Coord coord)
    {
        return GetResult(coord, value);
    }

    public Result UnaryVisit(Result argument, UnaryOperationType op, Coord coord)
    {
        if (argument.Value is int && op == UnaryOperationType.Not)
        {
            //TODO Error: Unsupported {op} for {argument.Type}.
        }
        else if (argument.Value is bool && op == UnaryOperationType.Negative)
        {
            //TODO Error: Unsupported {op} for {argument.Type}.
        }
        return GetResult(coord, argument);
    }

    public Result BinaryVisit(Result left, BinaryOperationType op, Result right, Coord coord)
    {
        if (left.Type != right.Type)
        {
            AddException(coord, $"Unsupported {op} for {left.Type} and {right.Type}");
        }
        else if (right.Value is int rInt && (op == BinaryOperationType.Divide || op == BinaryOperationType.Modulus) && rInt == 0)
        {
            AddException(coord, "Division by zero is not supported");
        }
        switch (op)
        {
            case BinaryOperationType.Add:
            case BinaryOperationType.Subtract:
            case BinaryOperationType.Multiply:
            case BinaryOperationType.Divide:
            case BinaryOperationType.Power:
            case BinaryOperationType.Modulus:
                return GetResult(coord, new Result(typeof(int)));
            case BinaryOperationType.Or:
            case BinaryOperationType.And:
            case BinaryOperationType.LessOrEqualThan:
            case BinaryOperationType.LessThan:
            case BinaryOperationType.GreaterOrEqualThan:
            case BinaryOperationType.GreaterThan:
            case BinaryOperationType.Equal:
            case BinaryOperationType.NotEqual:
                return GetResult(coord, new Result(typeof(bool)));
            default:
                throw new Exception();
        }
    }

    public void LabelVisit(string identifier, Coord coord)
    {
        if (Context.Labels.ContainsKey(identifier))
            throw new Exception();
    }

    public void GotoVisit(string targetLabel, Result? condition, Coord coord)
    {
        throw new NotImplementedException();
    }

    public void CodeBlockVisit(IStatement[] lines)
    {
        SearchLabel(lines);
        foreach (IStatement? item in lines)
        {
            if (item is not LabelStatement)
                item.Accept(this);
        }
    }

    public void SearchLabel(IStatement[] lines)
    {
        foreach (var item in lines)
        {
            if (item is LabelStatement label)
            {
                label.Accept(this);
            }
        }
    }

    #region Tools

    private Result GetResult(Coord coord, Result value)
    {
        var type = value.Type;
        if (type != typeof(int) && type != typeof(bool) && type != typeof(string))
        {
            AddException(coord, "Unsupported value");
        }
        return new Result(type);
    }

    public void AddException(Coord coord, string message)
    {
        Exceptions.Add(new ParserException(coord, message));
    }

    #endregion
}
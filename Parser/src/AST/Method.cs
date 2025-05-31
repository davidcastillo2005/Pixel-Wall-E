using PixelWallE.Parser.src.Interfaces;
namespace PixelWallE.Parser.src.AST;

public abstract class Method(string identifier, IExpression[] arguments)
{
    public string Identifier { get; set; } = identifier;
    public IExpression[] Arguments { get; set; } = arguments;
}

public class Function(
    string identifier,
    IExpression[] arguments) : Method(identifier, arguments), IExpression
{
    public Result Accept(IVisitor visitor)
        => visitor.FunctionVisit(Identifier, visitor.ParametersVisit(Arguments));
}

public class Action(
    string identifier,
    IExpression[] arguments) : Method(identifier, arguments), IStatement
{
    public void Accept(IVisitor visitor)
        => visitor.ActionVisit(Identifier, visitor.ParametersVisit(Arguments));
}
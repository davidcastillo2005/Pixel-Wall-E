using PixelWallE.Parser.src.Interfaces;
namespace PixelWallE.Parser.src.AST;

public class AssignStatement(string identifier, IExpression value) : IStatement
{
    public string Identifier { get; } = identifier;
    public IExpression Value { get; } = value;

    public void Accept(IVisitor visitor)
        => visitor.AssignVisit(Identifier, Value.Accept(visitor));
}

public class LabelStatement(string identifier, int line) : IStatement
{
    public string Identifier { get; set; } = identifier;
    public int Line { get; } = line;

    public void Accept(IVisitor visitor)
        => visitor.LabelVisit(Identifier, Line);
}

public class GoToStatement(string targetLabel, IExpression? condition) : IStatement
{
    public string TargetLabel { get; } = targetLabel;
    public IExpression? Condition { get; } = condition;

    public void Accept(IVisitor visitor)
        => visitor.GotoVisit(TargetLabel, Condition?.Accept(visitor));
}
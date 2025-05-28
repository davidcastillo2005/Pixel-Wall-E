using PixelWallE.Parser.src.Interfaces;
namespace PixelWallE.Parser.src.AST;

public class AssignStmnt(string identifier, IExpression value) : IStatement
{
    public string Identifier { get; } = identifier;
    public IExpression Value { get; } = value;

    public void Accept(IVisitor visitor)
    {
        visitor.AssignVisit(Identifier, Value.Accept(visitor));
    }
}

public class LabelStmnt(string value, int index) : IStatement
{
    public string Value { get; set; } = value;
    public int Index { get; } = index;

    public void Accept(IVisitor visitor) { }
}

public class GoToStmnt(string targetLabel, IExpression? condition) : IStatement
{
    public string TargetLabel { get; } = targetLabel;
    public IExpression? Condition { get; } = condition;

    public void Accept(IVisitor visitor)
    {
        visitor.GotoVisit(TargetLabel, Condition?.Accept(visitor));
    }
}
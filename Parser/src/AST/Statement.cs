using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src.AST;

public abstract class Statement : IStatement
{
    public abstract void Accept(IVisitor visitor);
}

public class AssignStmnt(string identifier, IExpression value) : Statement
{
    public string Identifier { get; } = identifier;
    public IExpression Value { get; } = value;

    public override void Accept(IVisitor visitor)
    {
        visitor.AssignVisit(Identifier, Value.Accept(visitor));
    }
}

public class LabelStmnt(string value, int index) : Statement
{
    public string Value { get; set; } = value;
    public int Index { get; } = index;

    public override void Accept(IVisitor visitor) { }

    // public override void SearchLabel(Context context)
    // {
    //     context.Labels[Value] = Index;
    // }
}

public class GoToStmnt(string targetLabel, IExpression? condition) : Statement
{
    public string TargetLabel { get; } = targetLabel;
    public IExpression? Condition { get; } = condition;

    public override void Accept(IVisitor visitor)
    {
        visitor.GotoVisit(TargetLabel, Condition?.Accept(visitor));
    }
}
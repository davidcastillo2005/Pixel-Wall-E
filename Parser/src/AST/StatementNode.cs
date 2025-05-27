namespace PixelWallE.Parser.src.AST;

public interface IStatement : ISearch
{
    void Accept(Context context);
}

public abstract class StatementNode : AstNode, IStatement
{
    public abstract void Accept(Context context);
}

public class LabelStmntNode(string value, int index) : StatementNode
{
    public string Value { get; set; } = value;
    public int Index { get; } = index;

    public override void Accept(Context context) { }

    public override void SearchLabel(Context context)
    {
        context.Labels[Value] = Index;
    }
}

public class GoToStmntNode(string targetLabel, IExpression? condition) : StatementNode
{
    public string TargetLabel { get; } = targetLabel;
    public IExpression? Condition { get; } = condition;

    public override void Accept(Context context)
    {
        if (Condition is not null)
        {
            bool cond = Condition.Accept(context).ToBool();
            if (!cond)
                return;
        }
        context.Jump(TargetLabel);
    }
}
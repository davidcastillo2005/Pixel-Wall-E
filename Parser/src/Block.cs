using PixelWallE.Parser.src.AST;

namespace PixelWallE.Parser.src;

public class Block(AstNode[] lines) : StatementNode
{
    public AstNode[] Lines { get; protected set; } = lines;

    public override void Accept(Context context)
    {
        for (int i = 0; i < Lines.Length; i++)
        {
            Lines[i].Accept(context);
            if (context.IsJumping)
            {
                i = context.Labels[context.TargetLabel!];
                context.EndJump();
            }
        }
    }

    public override void SearchLabel(Context context)
    {
        for (int i = 0; i < Lines.Length; i++)
        {
            Lines[i].SearchLabel(context);
        }
    }
}

public class AssignExpre(string name, IExpression value) : StatementNode
{
    public string Name { get; } = name;
    public IExpression Value { get; } = value;

    public override void Accept(Context context)
    {
        context.Variables[Name] = Value.Accept(context);
    }
}
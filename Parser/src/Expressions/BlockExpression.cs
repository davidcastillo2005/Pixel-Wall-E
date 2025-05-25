namespace PixelWallE.Parser.src.Expressions;

public class BlockExpression : Instruction
{
    public IInstruction[] Lines { get; protected set; }

    public BlockExpression(IInstruction[] lines) => Lines = lines;

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

public class AssignExpre(string name, IExpression value) : Instruction
{
    public string Name { get; } = name;
    public IExpression Value { get; } = value;

    public override void Accept(Context context)
    {
        context.Variables[Name] = Value.Accept(context);
    }
}
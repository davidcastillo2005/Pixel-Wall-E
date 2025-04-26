namespace PixelWallE.Parser.src.Expressions;

public class BlockExpression(IInstruction[] lines) : Instruction
{
    public IInstruction[] Lines { get; protected set; } = lines;

    public override void Accept()
    {
        Array.ForEach(Lines, x => x.Accept());
    }
}

public class AssignExpre(string name, IExpression value) : Instruction
{
    public string Name { get; } = name;
    public IExpression Value { get; } = value;

    public override void Accept()
    {
        var value = Value.Accept();
    }
}
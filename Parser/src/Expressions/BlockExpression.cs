namespace PixelWall_E.Parser.src.Expressions;

public class BlockExpression(IExpression[] lines) : Expression
{
    public override void Accept()
    {
        Array.ForEach(lines, x => x.Accept());
    }
}

public class AssignExpre<T>(string name, IExpression<T> value) : Expression
{
    public override void Accept()
    {
    }
}
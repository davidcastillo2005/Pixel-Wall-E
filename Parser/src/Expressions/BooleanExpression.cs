namespace PixelWall_E.Parser.src.Expressions;

public class ANDExpre(IExpression<bool> left, IExpression<bool> right) : BinaryExpre<bool>(left, right)
{
    public override bool Accept()
    {
        return Left.Accept() && Right.Accept();
    }
}

public class ORExpre(IExpression<bool> left, IExpression<bool> right) : BinaryExpre<bool>(left, right)
{
    public override bool Accept()
    {
        return Left.Accept() || Right.Accept();
    }
}
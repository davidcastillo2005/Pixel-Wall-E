namespace PixelWall_E.Parser.src.Expressions;

public class IntegerExpre(int i) : Expression<int>
{
    public int Integer { get; set; } = i;
    public override int Accept()
    {
        return Integer;
    }
}

public class AdditionExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return LeftArgument.Accept() + RightArgument.Accept();
    }
}

public class SubtractionExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return LeftArgument.Accept() - RightArgument.Accept();
    }
}

public class MultiplicationExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return LeftArgument.Accept() * RightArgument.Accept();
    }
}

public class DivisionExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return LeftArgument.Accept() / RightArgument.Accept();
    }
}

public class ExponentiationExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return (int)Math.Pow(LeftArgument.Accept(), RightArgument.Accept());
    }
}
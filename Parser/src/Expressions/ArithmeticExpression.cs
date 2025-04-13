namespace PixelWall_E.Parser.src.Expressions;

public class NumberExpre(int num) : Expression<int>
{
    public int Num { get; set; } = num;
    public override int Accept()
    {
        return Num;
    }
}

public class AdditionExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return Left.Accept() + Right.Accept();
    }
}

public class SubtractionExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return Left.Accept() - Right.Accept();
    }
}

public class MultiplicationExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return Left.Accept() * Right.Accept();
    }
}

public class DivisionExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return Left.Accept() / Right.Accept();
    }
}

public class ExponentiationExpre(IExpression<int> left, IExpression<int> right) : BinaryExpre<int>(left, right)
{
    public override int Accept()
    {
        return (int)Math.Pow(Left.Accept(), Right.Accept());
    }
}
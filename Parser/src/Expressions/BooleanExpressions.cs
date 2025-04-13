namespace PixelWall_E.Parser.src.Expressions;

public class BooleanLiteralExpre(bool b) : Expression<bool>
{
    public bool Boolean { get; set; } = b;

    public override bool Accept()
    {
        return Boolean;
    }
}

public class NotExpre(IExpression<bool> expre) : UnaryExpre<bool>(expre)
{
    public override bool Accept()
    {
        return !Argument.Accept();
    }
}

public class AndExpre(IExpression<bool> left, IExpression<bool> right) : BinaryExpre<bool>(left, right)
{
    public override bool Accept()
    {
        return LeftArgument.Accept() && RightArgument.Accept();
    }
}

public class OrExpre(IExpression<bool> left, IExpression<bool> right) : BinaryExpre<bool>(left, right)
{
    public override bool Accept()
    {
        return LeftArgument.Accept() || RightArgument.Accept();
    }
}
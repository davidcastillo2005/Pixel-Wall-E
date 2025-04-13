namespace PixelWall_E.Parser.src.Expressions;

public interface IExpression
{
    void Accept();
}

public interface IExpression<T> : IExpression
{
    new T Accept();
}

public abstract class Expression : IExpression
{
    public abstract void Accept();
}

public abstract class Expression<T> : IExpression<T>
{
    public abstract T Accept();

    void IExpression.Accept() => Accept();
}

public abstract class BinaryExpre<T>(IExpression<T> left, IExpression<T> right) : Expression<T>
{
    public IExpression<T> LeftArgument { get; set; } = left;
    public IExpression<T> RightArgument { get; set; } = right;
}

public abstract class UnaryExpre<T>(IExpression<T> value) : Expression<T>
{
    public IExpression<T> Argument { get; set; } = value;
}
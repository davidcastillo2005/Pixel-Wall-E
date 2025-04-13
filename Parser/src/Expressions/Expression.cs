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
    public IExpression<T> Left { get; set; } = left;
    public IExpression<T> Right { get; set; } = right;
}

public abstract class UnaryArithExpre<T>(IExpression<T> value) : Expression<T>
{
    public IExpression<T> Value { get; set; } = value;
}
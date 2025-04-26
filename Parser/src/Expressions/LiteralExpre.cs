namespace PixelWallE.Parser.src.Expressions;

public class LiteralExpre(ValueType value) : Expression
{
    public ValueType Value { get; set; } = value;
    public override ValueType Accept()
    {
        return Value;
    }
}

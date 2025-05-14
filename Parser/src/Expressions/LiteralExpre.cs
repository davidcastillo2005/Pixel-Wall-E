namespace PixelWallE.Parser.src.Expressions;

public class LiteralExpre(Result value) : Expression
{
    public Result Value { get; set; } = value;
    public override Result Accept(Context context)
    {
        return Value;
    }
}

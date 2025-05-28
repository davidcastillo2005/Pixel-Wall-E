namespace PixelWallE.Parser.src.Interfaces;

public interface IExpression
{
    Result Accept(IVisitor visitor);
}

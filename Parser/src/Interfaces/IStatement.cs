namespace PixelWallE.Parser.src.Interfaces;

public interface IStatement
{
    void Accept(IVisitor visitor);
}

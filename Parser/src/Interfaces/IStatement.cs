using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src.AST;

public interface IStatement
{
    void Accept(IVisitor visitor);
}

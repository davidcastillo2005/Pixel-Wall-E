using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src.AST;

public class CodeBlock(IStatement[] lines) : IStatement
{
    public IStatement[] Lines { get; protected set; } = lines;

    public void Accept(IVisitor visitor) => visitor.CodeBlockVisit(Lines);
}
using System.Reflection;
using PixelWallE.Parser.src.AST;
using PixelWallE.Parser.src.Interfaces;

namespace PixelWallE.Parser.src;

public class CodeBlock(IStatement[] lines) : Statement, IStatement
{
    public IStatement[] Lines { get; protected set; } = lines;

    public override void Accept(IVisitor visitor)
    {
        visitor.CodeBlockVisit(Lines);
    }

    // public override void SearchLabel(Context context)
    // {
    //     for (int i = 0; i < Lines.Length; i++)
    //     {
    //         Lines[i].SearchLabel(context);
    //     }
    // }
}
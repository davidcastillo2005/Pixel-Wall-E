using PixelWallE.Lexer.src;
using PixelWallE.Parser.src.Visitors;

namespace PixelWallE.Parser.src;

public class Context
{
    public bool IsJumping { get; set; } = false;
    public string? TargetLabel { get; set; } = null;
    public IHandleMethods Handler { get; }
    public Context(IHandleMethods handler)
    {
        Variables = [];
        Labels = [];
        Handler = handler;
    }
    public Dictionary<string, Result> Variables { get; set; }
    public Dictionary<string, int> Labels { get; set; }

    public void Jump(string targetLabel)
    {
        IsJumping = true;
        TargetLabel = targetLabel;
    }

    public void EndJump()
    {
        IsJumping = false;
        TargetLabel = null;
    }
}

public interface IHandleMethods
{
    Result CallFunction(string Name, Result[] @params);
    void CallAction(string Name, Result[] @params);
    bool TryGetErrFunction(string Name, Result[] @params, out Result result);
    bool TryGetErrAction(string Name, Result[] @params, SemanticErrVisitor errVisitor);
}
namespace PixelWallE.Parser.src;

public class Context
{
    public bool IsJumping { get; set; } = false;
    public string? TargetLabel { get; set; } = null;
    public Context(Dictionary<string, Func<dynamic[], dynamic>> functions, Dictionary<string, Action<dynamic[]>> actions)
    {
        Variables = [];
        Labels = [];
        Functions = functions;
        Actions = actions;
    }
    public Dictionary<string, Result> Variables { get; set; }
    public Dictionary<string, Func<dynamic[], dynamic>> Functions { get; set; }
    public Dictionary<string, Action<dynamic[]>> Actions { get; set; }
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
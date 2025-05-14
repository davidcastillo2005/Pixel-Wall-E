namespace PixelWallE.Parser.src;

public class Context
{
    public Context(Dictionary<string, Func<dynamic[], dynamic>> functions, Dictionary<string, Action<dynamic[]>> actions)
    {
        Variables = [];
        Functions = functions;
        Actions = actions;
    }
    public Dictionary<string, Result> Variables { get; set; }
    public Dictionary<string, Func<dynamic[], dynamic>> Functions { get; set; }
    public Dictionary<string, Action<dynamic[]>> Actions { get; set; }
}
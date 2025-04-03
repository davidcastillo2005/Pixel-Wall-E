namespace Lexer;
internal class Program
{
    private static void Main(string[] args)
    {
        string i = "Spawn(0 + y, 0)";
        src.Lexer lexer = new();
        src.Token[] o = lexer.Tokenize(i);
        foreach (var item in o)
        {
            System.Console.WriteLine(item.Value);
        }
    }
}
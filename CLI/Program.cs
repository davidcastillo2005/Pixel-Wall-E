namespace PixelWallE.CLI;

internal class Program
{
    private static void Main(string[] args)
    {
        // try
        // {
        Parser.src.Parser.Parser parser = new();
        var ast = parser.Parse(Lexer.src.Lexer.ScanInput(Reader.src.Reader.ReadFile("C:\\Users\\Audiovisual1\\Documents\\Pixel Wall-E\\0.pw")!));
        ast.Accept();
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine($"Error: {e}");
        // }
    }
}
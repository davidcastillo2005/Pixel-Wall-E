using PixelWall_E.Lexer;
using PixelWall_E.Parser;
using PixelWall_E.Reader;
namespace PixelWall_E.CLI;

internal class Program
{
    private static void Main(string[] args)
    {
        // try
        // {
        Parser.src.Parser parser = new();
        parser.Parse(Lexer.src.Lexer.ScanInput(Reader.src.Reader.ReadFile("C:\\Users\\Audiovisual1\\Documents\\Pixel Wall-E\\0.pw")!));
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine($"Error: {e}");
        // }
    }
}
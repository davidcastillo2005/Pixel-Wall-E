using PixelWallE.Parser.src;

namespace PixelWallE.CLI;

internal class Program
{
    private static void Main(string[] args)
    {
        // try
        // {
        // var tokens = Lexer.src.Lexer.Tokenize(Reader.src.Reader.ReadFile("C:\\Users\\Audiovisual1\\Documents\\Pixel Wall-E\\0.pw")!);
        Reader.src.Reader reader = new();
        Lexer.src.Lexer lexer = new();
        Parser.src.Parser parser = new();
        Context context = new(
        new Dictionary<string, Func<dynamic[], dynamic>>()
        {
        {"Func", Func}
        },
        []);


        var input = reader.ReadFile(@"C:\Users\Audiovisual1\Documents\Pixel Wall-E\0.pw");
        var tokens = lexer.Scan(input!);
        var ast = parser.Parse(tokens);
        
        // }
        // catch (Exception e)
        // {    
        //     Console.WriteLine($"Error: {e}");
        // }
    }

    private static dynamic Func(dynamic[] @params)
    {
        return @params[0] == @params[1];
    }
}
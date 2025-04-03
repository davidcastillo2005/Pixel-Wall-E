namespace Lexer;
internal class Program
{
    private static void Main(string[] args)
    {
        string i = " BlueBerry(0, 0) \n Color(Black) \n n <- 5 \n k <- 3 + 3 * 10 \n n <- k * 2 \n actual-x <- GetActualX() \n i <- 0 \n loop-1 \n DrawLine(1, 0, 1) \n i <- i + 1 \n is-brush-color-blue <- IsBrushColor(Blue) \n Goto [loop-ends-here] (is-brush-color-blue == 1) \n GoTo [loop1] (i < 10) \n Color(Blue) \n GoTo [loop1] (1 == 1) \n loop-ends-here";
        src.Token[] o = src.Lexer.Tokenize(i);
        foreach (var item in o)
        {
            System.Console.WriteLine($"[{item.Type}, \"{item.Lexeme}\"]");
        }
    }
}
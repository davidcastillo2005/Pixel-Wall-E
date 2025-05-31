using PixelWallE.Lexer.src;

namespace PixelWallE.Parser.src;

public class ParserException : Exception
{
    public int Row { get; private set; } = 0;
    public int Column { get; private set; } = 0;
    public int Length { get; private set; } = 0;

    public ParserException(Coord coord, string message) : base(message)
    {
        Row = coord.Row;
        Column = coord.Col;
        Length = coord.Length;
    }

    public string PrintMessage()
    {
        return $"<{Row}, {Column}> {Message}";
    }
}
namespace PixelWall_E.Reader.src;

public class Reader
{
    public static string? ReadFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath) && Path.GetExtension(filePath) == ".pw")
            {
                string[] lines = File.ReadAllLines(filePath);
                string r = "";
                foreach (var line in lines)
                {
                    r += line + '\n';
                }
                return r;
            }
            else
            {
                Console.WriteLine("File does not exist or is not a .pw file.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }

        return null;
    }
}
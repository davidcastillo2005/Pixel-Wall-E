using ViewPixelWall_E.Interfaces;

namespace ViewPixelWall_E
{
    public class Handlers
    {
        public IPaint Paint { get; }

        public Handlers(IPaint paint)
        {
            Paint = paint;
        }
        public int GetActualX() => Paint.wallE.Position!.Value.x;

        public int GetActualY() => Paint.wallE.Position!.Value.y;

        public int GetCanvasSize() 
        {
            
            return 0;
        }

        public int GetColorCount(string color, int x1, int y1, int x2, int y2)
        {
            return 0;
        }
    }
}

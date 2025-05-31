using System.Windows.Media;

namespace ViewPixelWall_E
{
    public class WallE
    {
        public (int x, int y)? Position { get; set; } = null;
        public Brush Brush { get; set; } = Brushes.White;
        public uint Size
        {
            get { return size; }
            set
            {
                size = value - (value + 1) % 2;
            }
        }
        
        private uint size = 1;
    }
}

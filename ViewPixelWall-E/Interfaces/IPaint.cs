using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ViewPixelWall_E.Interfaces
{
    public interface IPaint
    {
        public Rectangle[,] MainCanvasMatrix { get; }
        public WallE wallE { get; }
    }
}

using System;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TetrisClient
{
    /// <summary>
/// klasse die tetromino vorm aanmaakt en roteert.
/// </summary>
    public class Tetromino
    {
        
        public Vector2 Position = new(3, 0);
        public Matrix Shape { get; set; }
        public Brush Color { get; set; }


        

        public void Rotate()
        {
            Shape = Shape.Rotate90();
        }

    }
}
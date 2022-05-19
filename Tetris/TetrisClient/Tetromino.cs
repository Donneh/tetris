using System.Numerics;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TetrisClient
{
    public class Tetromino
    {
        
        public Vector2 Position = new(3, 0);
        public Matrix Shape { get; set; }
        public Brush Color { get; set; }


        public Rectangle ToRectangle()
        {
            return new Rectangle()
            {
                Width = 25, // Breedte van een 'cell' in de Grid
                Height = 25, // Hoogte van een 'cell' in de Grid
                Stroke = Brushes.White, // De rand
                StrokeThickness = 1, // Dikte van de rand
                Fill = Brushes.Red, // Achtergrondkleur
            };
        }
    }
}
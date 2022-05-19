using System;
using System.Windows.Media;

namespace TetrisClient
{
    public class TetrominioService
    {
        private Matrix[] PossibleShapes =
        {
            new(new int[,]
            {
                {1, 0, 0, 0},
                {1, 1, 1, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0},
            }),
            new(new int[,]
            {
                {0, 0, 0, 0},
                {1, 1, 1, 1},
                {0, 0, 0, 0},
                {0, 0, 0, 0}
            }),
            new(new int[,]
            {
                {0, 0, 1},
                {1, 1, 1},
                {0, 0, 0}
            }),
            new(new int[,]
            {
                {0, 1, 1, 0},
                {0, 1, 1, 0},
                {0, 0, 0, 0},
                {0, 0, 0, 0}
            }),
            new(new int[,]
            {
                {0, 1, 1},
                {1, 1, 0},
                {0, 0, 0}
            }),
            new(new int[,]
            {
                {0, 1, 0},
                {1, 1, 1},
                {0, 0, 0}
            }),
            new(new int[,]
            {
                { 1, 1, 0 },
                { 0, 1, 1 },
                { 0, 0, 0 }
            })
        };
        
        private Brush[] Colors =
        {
            Brushes.Cyan,
            Brushes.Blue,
            Brushes.Orange,
            Brushes.Yellow,
            Brushes.Green,
            Brushes.Purple,
            Brushes.Red,
        };

        public Tetromino GetRandomTetromino()
        {
            var Random = new Random();
            var randomInt = Random.Next(0, PossibleShapes.Length);

            var tetromino = new Tetromino();
            tetromino.Shape = PossibleShapes[randomInt];
            tetromino.Color = Colors[randomInt];

            return tetromino;
        }
    }
}
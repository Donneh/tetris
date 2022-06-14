using System;

namespace TetrisClient
{
    public class TetrominoService
    {
        private readonly Random random;

        private int[,] LShape()
        {
            return new int[,]
                {
                {1, 0, 0},
                {1, 1, 1},
                {0, 0, 0},
                };
        }

        private int[,] IShape()
        {
            return new int[,]{
                { 0, 0, 0, 0},
                { 2, 2, 2, 2},
                { 0, 0, 0, 0},
                { 0, 0, 0, 0} };
        }

        private int[,] JShape()
        {
            return new int[,]
                {
                {0, 0, 3},
                {3, 3, 3},
                {0, 0, 0}
                };

        }

        private int[,] OShape()
        {
            return new int[,]
                    {
            {4, 4,},
            {4, 4}
                    };
        }

        private int[,] SShape()
        {
            return new int[,]
                        {
                {0, 5, 5},
                {5, 5, 0},
                {0, 0, 0}
                        };

        }

        private int[,] TShape()
        {
            return new int[,] {
                {0, 6, 0},
                {6, 6, 6},
                {0, 0, 0}
            };
        }

        private int[,] ZShape()
        {

            return new int[,] {

                { 7, 7, 0 },
                { 0, 7, 7 },
                { 0, 0, 0 }

            };

        }

        public TetrominoService(int seed)
        {
            random = new Random(seed);
        }

        public Tetromino GetRandomTetromino()
        {
            var randomInt = random.Next(0, 7);
                
            return new Tetromino
            {
                Shape = GetRandomBLock(randomInt)
            };
        }

        private Matrix GetRandomBLock(int number) => number switch
        {
            0 => new Matrix(LShape()),
            1 => new Matrix(IShape()),
            2 => new Matrix(JShape()),
            3 => new Matrix(OShape()),
            4 => new Matrix(SShape()),
            5 => new Matrix(TShape()),
            6 => new Matrix(ZShape()),
            _ => throw new ArgumentOutOfRangeException(nameof(number), $"Not expected code: {number}")
        };

    };

   
}
        
       
    

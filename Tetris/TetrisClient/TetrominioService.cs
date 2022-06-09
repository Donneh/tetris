using System;
using System.Windows.Media;

namespace TetrisClient
{
    public class TetrominioService
    {
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



        //{
        //new(new int[,]
        //{
        //    {1, 0, 0},
        //    {1, 1, 1},
        //    {0, 0, 0},                
        //}),
        //new(new int[,]
        //{
        //    {0, 0, 0, 0},
        //    {1, 1, 1, 1},
        //    {0, 0, 0, 0},
        //    {0, 0, 0, 0}
        //}),
        //new(new int[,]
        //{
        //    {0, 0, 1},
        //    {1, 1, 1},
        //    {0, 0, 0}
        //}),
        //new(new int[,]
        //{
        //    {0, 1, 1, 0},
        //    {0, 1, 1, 0},
        //    {0, 0, 0, 0},
        //    {0, 0, 0, 0}
        //}),
        //new(new int[,]
        //{
        //    {0, 1, 1},
        //    {1, 1, 0},
        //    {0, 0, 0}
        //}),
        //new(new int[,]
        //{
        //    {0, 1, 0},
        //    {1, 1, 1},
        //    {0, 0, 0}
        //}),
        //new(new int[,]
        //{
        //    { 1, 1, 0 },
        //    { 0, 1, 1 },
        //    { 0, 0, 0 }
        //})


        public Tetromino GetRandomTetromino()
        {
            var Random = new Random();
            var randomInt = Random.Next(0, 7); 
            //var randomInt = 0;
            var tetromino = new Tetromino();
            tetromino.Shape = GetRandomBLock(0);

            return tetromino;
        }

        public Matrix GetRandomBLock(int number) => number switch
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
        
       
    

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
                { 1, 1, 1, 1},
                { 0, 0, 0, 0},
                { 0, 0, 0, 0} };
        }

        private int[,] JShape()
        {
            return new int[,]
                {
                {0, 0, 1},
                {1, 1, 1},
                {0, 0, 0}
                };

        }

        private int[,] OShape()
        {
            return new int[,]
                    {
            {0, 1, 1, 0},
            {0, 1, 1, 0},
            {0, 0, 0, 0},
            {0, 0, 0, 0}
                    };
        }

        private int[,] SShape()
        {
            return new int[,]
                        {
                {0, 1, 1},
                {1, 1, 0},
                {0, 0, 0}
                        };

        }

        private int[,] TShape()
        {
            return new int[,] {
                {0, 1, 0},
                {1, 1, 1},
                {0, 0, 0}
            };
        }

        private int[,] ZShape()
        {

            return new int[,] {

                { 1, 1, 0 },
                { 0, 1, 1 },
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




        private Brush[] Colors =
           {
            Brushes.Blue,
            Brushes.Cyan,
            Brushes.Orange,
            Brushes.Yellow,
            Brushes.Green,
            Brushes.Purple,
            Brushes.Red,
        };

        public Tetromino GetRandomTetromino()
        {
            var Random = new Random();
            //var randomInt = Random.Next(0, PossibleShapes.Length); 
            var randomInt = 0;
            var tetromino = new Tetromino();
            tetromino.Shape = new Matrix(LShape());
            tetromino.Color = Colors[randomInt];

            return tetromino;
        }

    };

   
}
        
       
    

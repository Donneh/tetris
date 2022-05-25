using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TetrisClient
{
    public class TetrisEngine
    {
        public int dropSpeedInSeconds = 1;
        public Board Board;
        private TetrominioService _tetrominioService = new TetrominioService();
        public Tetromino currentTetromino;
        public List<Tetromino> stuckTetrominoes = new List<Tetromino>();
        public TetrisEngine()
        {
            currentTetromino = _tetrominioService.GetRandomTetromino();
            Board = new Board();
        }

        public bool CheckCollision()
        {
            var shape = currentTetromino.Shape.Value;

            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                   
                    if (shape[y, x] == 0) {
                        Debug.WriteLine('a');
                        continue;
                    }
                    if (currentTetromino.Position.Y + y > Board.squares.GetLength(0)) {
                        Debug.WriteLine('b');
                        return true;
                    }

                 

                }
            }

            return false;
        }


        public bool CheckSideCollision()
        {


            var shape = currentTetromino.Shape.Value;

            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {

                    if (shape[y, x] == 0)
                    {
                        Debug.WriteLine('a');
                        continue;
                    }
                   

                    if (currentTetromino.Position.X + x > (Board.squares.GetLength(1)) - 2)
                    {
                        Debug.WriteLine('c');
                        return true;
                    }                    

                    if (currentTetromino.Position.X + x == 0)
                    {
                        return true;
                    }

                }
            }

            return false;
        }


        internal bool CheckCollison()
        {
            throw new NotImplementedException();
        }

        

        public void SpawnTetromino()
        {
            stuckTetrominoes.Add(currentTetromino);
            currentTetromino = _tetrominioService.GetRandomTetromino();
        }
    }
}
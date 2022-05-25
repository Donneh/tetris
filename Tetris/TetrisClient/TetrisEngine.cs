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

        public string CheckCollision()
        {
            var shape = currentTetromino.Shape.Value;

            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                   
                    if (shape[y, x] == 0) {
                        continue;
                    }
                    if (currentTetromino.Position.Y + y > Board.squares.GetLength(0)) {                       
                        return "under";
                    }

                    if (currentTetromino.Position.X + x > (Board.squares.GetLength(1)) - 2)
                    {
                        return "right";
                    }                    

                    if (currentTetromino.Position.X + x == 0) {
                        return "left";
                    }

                }
            }

            return "";
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
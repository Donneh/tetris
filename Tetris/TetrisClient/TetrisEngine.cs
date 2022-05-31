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

        public void SpawnTetromino()
        {
            AddStuck();
            currentTetromino = _tetrominioService.GetRandomTetromino();
        }

        private void AddStuck()
        {
            stuckTetrominoes.Add(currentTetromino);
            var shape = currentTetromino.Shape.Value;

            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    if (shape[y, x] == 0) {
                        continue;
                    }

                    var newYPos = (int)currentTetromino.Position.Y + y - 1;
                    var newXPos = (int)currentTetromino.Position.X + x - 1;
                    Board.squares[newYPos, newXPos] = 1;
                }
            }
        }

        public bool MovePossible(Tetromino desiredPosition)
        {
            var shape = desiredPosition.Shape.Value;

            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    if (shape[y, x] == 0)
                    {
                        continue;
                    }

                    var newYPos = (int)(desiredPosition.Position.Y + y);
                    var newXPos  = (int)(desiredPosition.Position.X + x);
                    if (newYPos > Board.squares.GetLength(0) - 1)
                    {
                        return false;
                    }
                    if (Board.squares[newYPos, newXPos] == 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
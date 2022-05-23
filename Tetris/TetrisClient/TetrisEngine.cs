using System.Collections.Generic;
using System.Diagnostics;

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
                    return currentTetromino.Position.Y + y > Board.squares.GetLength(0);
                }
            }

            return false;
        }

        public void SpawnTetromino()
        {
            stuckTetrominoes.Add(currentTetromino);
            currentTetromino = _tetrominioService.GetRandomTetromino();
        }
    }
}
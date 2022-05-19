using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace TetrisClient
{
    public class TetrisEngine
    {
        public int dropSpeedInSeconds = 1;
        private int[,] _board = new int[15, 4];
        private TetrominioService _tetrominioService = new TetrominioService();
        public Tetromino currentTetromino;
        public TetrisEngine()
        {
            currentTetromino = _tetrominioService.GetRandomTetromino();
        }
    }
}
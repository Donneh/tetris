using System.Diagnostics;

namespace TetrisClient
{
    public class Board
    {
        private const int Columns = 10;
        private const int Rows = 19;
        public int[,] squares;

        public Board()
        { 
            squares = new int[Rows, Columns];
        }

        
    }
}
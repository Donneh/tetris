namespace TetrisClient
{
    public class Board
    {
        private const int Columns = 9;
        private const int Rows = 14;
        public int[,] squares;

        public Board()
        { 
            squares = new int[Rows, Columns];
        }
        
        
    }
}
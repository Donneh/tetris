namespace TetrisClient
{
    public class Board
    {
        private const int Columns = 10;
        private const int Rows = 20;
        private int[,] _squares;

        public Board()
        {
            this._squares = new int[Rows, Columns];
        }
        
        
    }
}
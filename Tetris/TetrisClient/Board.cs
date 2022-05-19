namespace TetrisClient
{
    public class Board
    {
        private int columns = 10;
        private int rows = 20;
        private int[] board;

        public Board()
        {
            this.board = new[] { rows, columns };
        }
    }
}
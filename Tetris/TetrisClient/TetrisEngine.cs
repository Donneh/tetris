using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TetrisClient
{
    public class TetrisEngine
    {
        public Board Board;
        private TetrominioService _tetrominioService = new TetrominioService();
        public Tetromino currentTetromino;
        public List<Tetromino> stuckTetrominoes = new List<Tetromino>();
        public readonly int dropSpeedInMilliSeconds = 1;

        public TetrisEngine()
        {
            currentTetromino = _tetrominioService.GetRandomTetromino();
            Board = new Board();
        }



        public void SpawnTetromino()
        {           
            currentTetromino = _tetrominioService.GetRandomTetromino();
        }

        public void AddStuck()
        {
            //var tet = new Tetromino();
           // tet.Shape = currentTetromino.Shape;
            //tet.Position = currentTetromino.Position;
            stuckTetrominoes.Add(currentTetromino);
            var shape = currentTetromino.Shape.Value;

            for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
            {
                for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
                {
                    if (shape[yOffset, xOffset] == 0) {
                        continue;
                    }

                    var newYPos = (int)currentTetromino.Position.Y + yOffset - 1;
                    var newXPos = (int)currentTetromino.Position.X + xOffset;
                    Board.squares[newYPos, newXPos] = 1;
                }
            }
        }

        public bool MovePossible(Tetromino desiredPosition)
        {
            var shape = desiredPosition.Shape.Value;

            for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
            {
                for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
                {
                    if (shape[yOffset, xOffset] == 0)
                    {
                        continue;
                    }


                    var newYPos = (int)(desiredPosition.Position.Y + yOffset);
                    var newXPos  = (int)(desiredPosition.Position.X + xOffset);

                    if (newYPos > Board.squares.GetLength(0))
                    {                        
                        //AddStuck();
                        //SpawnTetromino();
                        return false;
                    }                    

                    
                    if (newYPos == 0) {
                        newYPos++;
                    }
                    if (Board.squares[newYPos -1 , newXPos] == 1)
                    {                        
                       // AddStuck();
                       // SpawnTetromino();
                        return false;
                    }
                    
                }
            }

            return true;
        }

        public bool SideMovePossible(Tetromino desiredPosition)
        {
            var shape = desiredPosition.Shape.Value;

            for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
            {
                for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
                {
                    if (shape[yOffset, xOffset] == 0)
                    {
                        continue;
                    }

                    var newYPos = (int)(desiredPosition.Position.Y + yOffset);
                    var newXPos = (int)(desiredPosition.Position.X + xOffset);
                    if (newXPos < 0 || (newXPos + 1) > (Board.squares.GetLength(1)))
                    {

                        return false;
                    }
                    if (newYPos == 0)
                    {
                        newYPos++;
                    }
                    if (Board.squares[newYPos - 1, newXPos] == 1)
                    {                        
                        return false;
                    }
                }
            }
                    return true;
        }

        public bool isRowFull(int row) {

            for (var i = 0; i<Board.squares.GetLength(1); i++) {
                
                if(Board.squares[row, i] == 0){
                    return false;
                }                               
            }
            return true; 
        }

        public List<int> looper() {
            List<int> allFullRowsIndex = new List<int>();
            for (var i = 0; i < Board.squares.GetLength(0); i++)
            {
                if (isRowFull(i)) {
                    
                    allFullRowsIndex.Add(i);
                }
            }
            return allFullRowsIndex;
        }

        public List<int> RemoveTetrominoPart()
        {
            List<int> rows = looper();
           
            //func om te kijken of tetromino in de gegeven rij zit           
            
            foreach (var Tetromino in stuckTetrominoes)
            {
                var shape = Tetromino.Shape.Value;
                
                

                for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
                    {
                    for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
                    {
                        if(shape[yOffset, xOffset] == 1)
                        {
                            if (rows.Count != 0)
                            {
                                foreach (int y in rows)
                                {
                                    shape[((int)(y - (Tetromino.Position.Y - 1))), (xOffset)] = 0;
                                }
                            }
                            break;
                                                      
                        }
                    }
                    
                }
            }
            //return de getallen van de rijen die je verwijdert zodat je de tetromino's omlaag kan gooien. :)
            return null;
        }
    }
}
﻿using System;
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
        public readonly int dropSpeedInMilliSeconds = 100;

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
                        AddStuck();
                        SpawnTetromino();
                        return false;
                    }                    

                    
                    if (newYPos == 0) {
                        newYPos++;
                    }
                    if (Board.squares[newYPos -1 , newXPos] == 1)
                    {                        
                        AddStuck();
                        SpawnTetromino();
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

        public void removeTetrominoPart(List<int> FullRows)
        {
            List<int> rows = looper();
            Debug.WriteLine(rows);
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
                            for (var i = 0; i < Board.squares.GetLength(1); i++) {
                            //Debug.WriteLine(i);
                            foreach(int x in rows) {
                                Debug.WriteLine("i" + i);
                                Debug.WriteLine("X" + x);
                                
                                shape[x-1, 1] = 0;
                            }                                
                            }                           
                           // Debug.WriteLine(Tetromino.Position.X + xOffset);
                           // Debug.WriteLine(Tetromino.Position.Y + yOffset);
                           // shape[yOffset, xOffset] = 0;
                        }
                    }
                    
                }
            }
        }
    }
}
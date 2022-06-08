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
        public List<List<int>> playingGrid = new List<List<int>>(20);

        public TetrisEngine()
        {
            currentTetromino = _tetrominioService.GetRandomTetromino();
            Board = new Board();
            for(var i = 0; i < 20; i++)
{
                playingGrid.Add(Enumerable.Repeat(0, 10).ToList());
            }

        }



        public void SpawnTetromino()
        {          
            currentTetromino = _tetrominioService.GetRandomTetromino();
        }

        public void AddStuck()
        {
            var tet = new Tetromino();
            tet.Shape = new Matrix( currentTetromino.Shape.Value);
            tet.Position = new System.Numerics.Vector2(currentTetromino.Position.X, currentTetromino.Position.Y);
            tet.Color = currentTetromino.Color;
            stuckTetrominoes.Add(tet);
            var shape = tet.Shape.Value;

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
                        return false;
                    }                    

                    
                    if (newYPos == 0) {
                        newYPos++;
                    }
                    if (Board.squares[newYPos -1 , newXPos] == 1)
                    {                                               
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

        public void ClearLines() {
            var rowsToReplace = new List<List<int>>();
            for (var rowIndex = 0; rowIndex < playingGrid.Count; rowIndex++)
            {
                var row = playingGrid[rowIndex];

                // check if row is full
                if (!row.Contains(0))
                {
                    // reset row (no need to recreate row)
                    for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
                    {
                        row[columnIndex] = 0;
                    }

                    // add row to list
                    rowsToReplace.Add(row);
                }
            }

            // process full rows - delete it from current position and insert on top
            foreach (var row in rowsToReplace)
            {
                playingGrid.Remove(row);
                playingGrid.Insert(0, row);
            }

            if (rowsToReplace.Count != 0)
            {
                for (var rowIndex = 0; rowIndex < playingGrid.Count; rowIndex++)
                {
                    foreach (var col in playingGrid[rowIndex])
                    {
                        Debug.WriteLine(rowIndex + "" + col);
                    }
                }
            }


            }


        //public void MoveDown(int row) {
        //    for (var i = 0; i < playingGrid.Length; i++) {
            
        //    }
        //}

        public void DrawInArray()
        {
            foreach(var tetromino in stuckTetrominoes) {
                int[,] values = tetromino.Shape.Value;
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    for (int j = 0; j < values.GetLength(1); j++)
                    {
                        if (values[i, j] != 1) {
                            continue;
                        }

                        var rectangle = tetromino.ToRectangle();
                        playingGrid[(int)(i + tetromino.Position.Y)][(int)(j + tetromino.Position.X)] = 1;


                    }
                }
            }

        }

        

        



        //public List<int> RemoveTetrominoPart()
        //{

        //    foreach (var Tetromino in stuckTetrominoes)
        //    {
               

               

        //            var shape = Tetromino.Shape.Value;

        //            for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
        //            {
        //                //y as van tetromino
        //                for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
        //                {

        //                    //check if tetromino in row with 4 blocks

        //                    //x as van tetromino

        //                    if (shape[yOffset, xOffset] == 1)
        //                    {
        //                        //kijk of vorm niet 0 is
        //                        foreach (var row in fullRows)
        //                        {
                               
        //                            shape[((int)(row - (Tetromino.Position.Y - 1))), (xOffset)] = 0;
        //                        //Debug.WriteLine(Tetromino.Position.Y );
        //                        //Debug.WriteLine(row);
                                   
        //                    }
                            

        //                }
                        
        //            }
        //        }
        //    }
        //    return fullRows;
        //    //        //return de getallen van de rijen die je verwijdert zodat je de tetromino's omlaag kan gooien. :)
            
        //}

        
                            }
    }


﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace TetrisClient
{
    public class TetrisEngine
    {

        private TetrominioService _tetrominioService = new TetrominioService();
        public Tetromino currentTetromino;
        public Tetromino ghostPiece;
        public List<Tetromino> stuckTetrominoes = new List<Tetromino>();
        public  double dropSpeed = 1;
        public List<List<int>> playingGrid = new List<List<int>>(20);
        public int clearedLines = 0;
        public int level = 1;
        public bool levelChanged;

        public TetrisEngine()
        {
            currentTetromino = _tetrominioService.GetRandomTetromino();
            ghostPiece = new Tetromino
            {
                Shape = new Matrix(currentTetromino.Shape.Value),
                Position = new System.Numerics.Vector2(currentTetromino.Position.X, currentTetromino.Position.Y+18),
                Color = Brushes.Gray
            };
            for (var i = 0; i < 20; i++)
            {
                playingGrid.Add(Enumerable.Repeat(0, 10).ToList());
            }
        }



        public void SpawnTetromino()
        {
            ClearLines();
            currentTetromino = _tetrominioService.GetRandomTetromino();
            ghostPiece = new Tetromino
            {
                Shape = new Matrix(currentTetromino.Shape.Value),
                Position = new System.Numerics.Vector2(currentTetromino.Position.X, currentTetromino.Position.Y+3),
                Color = currentTetromino.Color
            };
        }

        public bool AddStuck()
        {
            var tet = new Tetromino
            {
                Shape = new Matrix(currentTetromino.Shape.Value),
                Position = new System.Numerics.Vector2(currentTetromino.Position.X, currentTetromino.Position.Y),
                Color = currentTetromino.Color
            };
            stuckTetrominoes.Add(tet);
            var shape = tet.Shape.Value;

            for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
            {
                for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
                {
                    if (shape[yOffset, xOffset] == 0)
                    {
                        continue;
                    }

                    var newYPos = (int)currentTetromino.Position.Y + yOffset;
                    var newXPos = (int)currentTetromino.Position.X + xOffset;
                    //Debug.WriteLine(newYPos);
                    if (newYPos > 0)
                    {
                        //prettyprint();
                        playingGrid[newYPos][newXPos] = shape[yOffset, xOffset];
                        //prettyprint();
                    }
                    else {
                        return false;
                    }
                }
            }            
            return true;
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
                    var newXPos = (int)(desiredPosition.Position.X + xOffset);
                    
                    if (newYPos > playingGrid.Count-1)
                    {
                        return false;
                    }


                    if (newYPos == 0)
                    {
                        newYPos++;
                    }


                    if (playingGrid[newYPos][newXPos] != 0)
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
                    if (newXPos < 0 || (newXPos + 1) > (playingGrid[0].Count()))
                    {

                        return false;
                    }
                    if (newYPos == 0)
                    {
                        newYPos++;
                    }


                    
                    if (playingGrid[newYPos - 1][ newXPos] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void ClearLines()
        {
            


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
                //prettyprint();
                clearedLines++;
                playingGrid.Remove(row);
                playingGrid.Insert(0, row);
                //prettyprint();
            }

            if (rowsToReplace.Count > 0)
            {
                //prettyprint();
                RemoveTetrominoPart();
            }

            SetDropspeed(clearedLines);
        }



        public void SetDropspeed(int clearedLines) 
        {
            var tvar = (clearedLines / 5) + 1;
            if (tvar > level)
            {
                levelChanged = true;
                level = (clearedLines / 5) + 1;
                dropSpeed = Math.Pow(0.8 - ((level - 1) * 0.007), (level - 1));
                Debug.WriteLine(dropSpeed);
            }
        }



        public void RemoveTetrominoPart()
        {
            List<Tetromino> tetrominos = new();

            var rowIndex = 0;
            var ColIndex = 0;


            //for (var rowI = 0; rowI < playingGrid.Count; rowI++)
            //{
            //    for (var ColI = 0; ColI < playingGrid[rowI].Count; ColI++)
            //    {

            //        if (playingGrid[rowI][ColI] != 0)
            //        {
            //            //Debug.WriteLine(rowI + "     " + ColI);
            //        }
            //    }
            //}



            for (var y = 0; y < 2; y++)
            {
                rowIndex = 0;
                for (var i = 0; i < 5; i++)
                {
                    //for (var y = 0; y < 4; y++) {
                    tetrominos.Add(new Tetromino
                    {
                        Shape = new Matrix(new int[,] {
                                        { playingGrid[rowIndex][ColIndex], playingGrid[rowIndex][ColIndex + 1], playingGrid[rowIndex][ColIndex + 2], playingGrid[rowIndex][ColIndex + 3] },
                                        { playingGrid[rowIndex + 1][ColIndex], playingGrid[rowIndex + 1][ColIndex + 1], playingGrid[rowIndex + 1][ColIndex + 2], playingGrid[rowIndex + 1][ColIndex + 3]},
                                        { playingGrid[rowIndex + 2][ColIndex], playingGrid[rowIndex + 2][ColIndex + 1], playingGrid[rowIndex + 2][ColIndex + 2], playingGrid[rowIndex + 2][ColIndex + 3] },
                                        { playingGrid[rowIndex + 3][ColIndex], playingGrid[rowIndex + 3][ColIndex + 1], playingGrid[rowIndex + 3][ColIndex + 2], playingGrid[rowIndex + 3][ColIndex + 3] }})
                        ,
                        Color = Brushes.Blue,
                        Position = new System.Numerics.Vector2(ColIndex,rowIndex)
                    }); ;
                    rowIndex += 4;
                    //}
                }
                ColIndex += 4;
            }
            rowIndex = 0;
            ColIndex = 8;

            for (var i = 0; i < 10; i++)
            {
                tetrominos.Add(new Tetromino
                {
                    Shape = new Matrix(new int[,] {
                                        { playingGrid[rowIndex][ColIndex], playingGrid[rowIndex][ColIndex + 1] },
                                        { playingGrid[rowIndex + 1][ColIndex], playingGrid[rowIndex + 1][ColIndex + 1]
                        }
                
                    }),
                Position = new System.Numerics.Vector2(ColIndex,rowIndex),
                Color=Brushes.Blue
                }); ;

                rowIndex += 2;
            }
            
            stuckTetrominoes = tetrominos;
            
        }

        public void prettyprint()
        {
            for (int i = 0; i < playingGrid.Count; i++)
            {
                for (int j = 0; j < playingGrid[0].Count; j++)
                {
                    Debug.Write(string.Format("{0} ", playingGrid[i][j]));
                }
                Debug.Write(Environment.NewLine + Environment.NewLine);
            }
            
            //foreach (var tet in stuckTetrominoes)
            //{
            //    for (int i = 0; i < tet.Shape.Value.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < tet.Shape.Value.GetLength(0); j++)
            //        {
            //            Debug.Write(string.Format("{0} ", tet.Shape.Value[i, j]));
            //        }
            //        Debug.Write(Environment.NewLine + Environment.NewLine);
            //    }
            //    //Console.ReadLine();}
            //}


        }
    }
    }
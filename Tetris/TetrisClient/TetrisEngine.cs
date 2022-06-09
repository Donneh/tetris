using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

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
            for (var i = 0; i < 20; i++)
            {
                playingGrid.Add(Enumerable.Repeat(0, 10).ToList());
            }

        }



        public void SpawnTetromino()
        {
            ClearLines();
            currentTetromino = _tetrominioService.GetRandomTetromino();
        }

        public void AddStuck()
        {
            var tet = new Tetromino();
            tet.Shape = new Matrix(currentTetromino.Shape.Value);
            tet.Position = new System.Numerics.Vector2(currentTetromino.Position.X, currentTetromino.Position.Y);
            tet.Color = currentTetromino.Color;
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

                    var newYPos = (int)currentTetromino.Position.Y + yOffset - 1;
                    var newXPos = (int)currentTetromino.Position.X + xOffset;
                    playingGrid[newYPos][ newXPos] = 1;
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
                    var newXPos = (int)(desiredPosition.Position.X + xOffset);

                    if (newYPos > Board.squares.GetLength(0))
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

        public void ClearLines()
        {
            DrawInArray();
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
            if (rowsToReplace.Count > 0)
            {

                RemoveTetrominoPart();
            }


        }


        //public void MoveDown(int row) {
        //    for (var i = 0; i < playingGrid.Length; i++) {

        //    }
        //}

        public void DrawInArray()
        {
            foreach (var tetromino in stuckTetrominoes)
            {
                int[,] values = tetromino.Shape.Value;
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    for (int j = 0; j < values.GetLength(1); j++)
                    {
                        if (values[i, j] != 1)
                        {
                            continue;
                        }

                        var rectangle = tetromino.ToRectangle();
                        playingGrid[(int)(i + tetromino.Position.Y)][(int)(j + tetromino.Position.X)] = 1;


                    }
                }
            }

        }




        public void printTetrominoPosition()
        {
            foreach (var tetro in stuckTetrominoes)
            {
                var shape = tetro.Shape.Value;

                for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
                {
                    //y as van tetromino
                    for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
                    {
                        if (shape[yOffset, xOffset] == 1)
                        {
                            Debug.WriteLine("y " + (tetro.Position.Y + yOffset) + "x " + (tetro.Position.X + xOffset));
                        }
                    }
                }
            }
        }


        public void RemoveTetrominoPart()
        {
            List<Tetromino> tetrominos = new();

            var rowIndex = 0;
            var ColIndex = 0;

            
            //for (var rowI = 0; rowI< playingGrid.Count; rowI++)
            //{
            //    for (var ColI = 0; ColI < playingGrid[rowI].Count; ColI++)
            //    {

            //        if (playingGrid[rowI][ColI] == 1)
            //        {
            //            Debug.WriteLine("OEEEE");
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

                //Debug.WriteLine(rowIndex + "       " + ColIndex);


                //}

                //}
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
                Color=Brushes.AliceBlue
                }); ;

                Debug.WriteLine(rowIndex + "           " + ColIndex);
                Debug.WriteLine(playingGrid[rowIndex][ColIndex] + " " + playingGrid[rowIndex][ColIndex + 1] + " " + playingGrid[rowIndex + 1][ColIndex] +" " + playingGrid[rowIndex + 1][ColIndex + 1]);
                rowIndex += 2;
            }
            
            stuckTetrominoes = tetrominos;           
            prettyprint();
            
        }

        public void prettyprint()
        {
            Debug.WriteLine("AA");
            foreach (var tet in stuckTetrominoes)
            {
                for (int i = 0; i < tet.Shape.Value.GetLength(0); i++)
                {
                    for (int j = 0; j < tet.Shape.Value.GetLength(0); j++)
                    {
                        Debug.Write(string.Format("{0} ", tet.Shape.Value[i, j]));
                    }
                    Debug.Write(Environment.NewLine + Environment.NewLine);
                }
                //Console.ReadLine();}
            }


        }
    }
    }


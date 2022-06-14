using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;

namespace TetrisClient
{
    public class TetrisEngine
    {
        private readonly TetrominoService tetrominoService;
        public Tetromino CurrentTetromino;
        public Tetromino GhostPiece;
        public List<Tetromino> StuckTetrominoes = new List<Tetromino>();
        public double DropSpeed = 1;
        public readonly List<List<int>> PlayingGrid = new List<List<int>>(20);
        private int clearedLines;
        public int Level = 1;
        public bool LevelChanged;
        private readonly int randomSeed;

        public TetrisEngine(int seed)
        {
            randomSeed = seed;
            tetrominoService = new TetrominoService(randomSeed);
            CurrentTetromino = tetrominoService.GetRandomTetromino(randomSeed);
            GhostPiece = new Tetromino
            {
                Shape = new Matrix(CurrentTetromino.Shape.Value),
                Position = new System.Numerics.Vector2(CurrentTetromino.Position.X, CurrentTetromino.Position.Y + 18),
                Color = Brushes.Gray
            };
            for (var i = 0; i < 20; i++)
            {
                PlayingGrid.Add(Enumerable.Repeat(0, 10).ToList());
            }
        }

        public void SpawnTetromino()
        {
            ClearLines();
            CurrentTetromino = tetrominoService.GetRandomTetromino(randomSeed);
            GhostPiece = new Tetromino
            {
                Shape = new Matrix(CurrentTetromino.Shape.Value),
                Position = new System.Numerics.Vector2(CurrentTetromino.Position.X, CurrentTetromino.Position.Y + 3),
                Color = CurrentTetromino.Color
            };
        }

        public bool AddStuck()
        {
            var tet = new Tetromino
            {
                Shape = new Matrix(CurrentTetromino.Shape.Value),
                Position = new System.Numerics.Vector2(CurrentTetromino.Position.X, CurrentTetromino.Position.Y),
                Color = CurrentTetromino.Color
            };
            StuckTetrominoes.Add(tet);
            var shape = tet.Shape.Value;

            for (var yOffset = 0; yOffset < shape.GetLength(0); yOffset++)
            {
                for (var xOffset = 0; xOffset < shape.GetLength(1); xOffset++)
                {
                    if (shape[yOffset, xOffset] == 0)
                    {
                        continue;
                    }

                    var newYPos = (int) CurrentTetromino.Position.Y + yOffset;
                    var newXPos = (int) CurrentTetromino.Position.X + xOffset;
                    //Debug.WriteLine(newYPos);
                    if (newYPos > 0)
                    {
                        //prettyprint();
                        PlayingGrid[newYPos][newXPos] = shape[yOffset, xOffset];
                        //prettyprint();
                    }
                    else
                    {
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


                    var newYPos = (int) (desiredPosition.Position.Y + yOffset);
                    var newXPos = (int) (desiredPosition.Position.X + xOffset);

                    if (newYPos > PlayingGrid.Count - 1)
                    {
                        return false;
                    }


                    if (newYPos == 0)
                    {
                        newYPos++;
                    }


                    if (PlayingGrid[newYPos][newXPos] != 0)
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

                    var newYPos = (int) (desiredPosition.Position.Y + yOffset);
                    var newXPos = (int) (desiredPosition.Position.X + xOffset);
                    if (newXPos < 0 || (newXPos + 1) > (PlayingGrid[0].Count()))
                    {
                        return false;
                    }

                    if (newYPos == 0)
                    {
                        newYPos++;
                    }


                    if (PlayingGrid[newYPos - 1][newXPos] != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ClearLines()
        {
            var rowsToReplace = new List<List<int>>();
            for (var rowIndex = 0; rowIndex < PlayingGrid.Count; rowIndex++)
            {
                var row = PlayingGrid[rowIndex];

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
                clearedLines++;
                PlayingGrid.Remove(row);
                PlayingGrid.Insert(0, row);
            }

            if (rowsToReplace.Count > 0)
            {
                RemoveTetrominoPart();
            }

            SetDropspeed(clearedLines);
        }


        public void SetDropspeed(int clearedLines)
        {
            var tvar = (clearedLines / 5) + 1;
            if (tvar > Level)
            {
                LevelChanged = true;
                Level = (clearedLines / 5) + 1;
                DropSpeed = Math.Pow(0.8 - ((Level - 1) * 0.007), (Level - 1));
                Debug.WriteLine(DropSpeed);
            }
        }


        public void RemoveTetrominoPart()
        {
            List<Tetromino> tetrominos = new();

            var rowIndex = 0;
            var ColIndex = 0;

            for (var y = 0; y < 2; y++)
            {
                rowIndex = 0;
                for (var i = 0; i < 5; i++)
                {
                    //for (var y = 0; y < 4; y++) {
                    tetrominos.Add(new Tetromino
                    {
                        Shape = new Matrix(new int[,]
                        {
                            {
                                PlayingGrid[rowIndex][ColIndex], PlayingGrid[rowIndex][ColIndex + 1],
                                PlayingGrid[rowIndex][ColIndex + 2], PlayingGrid[rowIndex][ColIndex + 3]
                            },
                            {
                                PlayingGrid[rowIndex + 1][ColIndex], PlayingGrid[rowIndex + 1][ColIndex + 1],
                                PlayingGrid[rowIndex + 1][ColIndex + 2], PlayingGrid[rowIndex + 1][ColIndex + 3]
                            },
                            {
                                PlayingGrid[rowIndex + 2][ColIndex], PlayingGrid[rowIndex + 2][ColIndex + 1],
                                PlayingGrid[rowIndex + 2][ColIndex + 2], PlayingGrid[rowIndex + 2][ColIndex + 3]
                            },
                            {
                                PlayingGrid[rowIndex + 3][ColIndex], PlayingGrid[rowIndex + 3][ColIndex + 1],
                                PlayingGrid[rowIndex + 3][ColIndex + 2], PlayingGrid[rowIndex + 3][ColIndex + 3]
                            }
                        }),
                        Color = Brushes.Blue,
                        Position = new System.Numerics.Vector2(ColIndex, rowIndex)
                    });
                    ;
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
                    Shape = new Matrix(new int[,]
                    {
                        {PlayingGrid[rowIndex][ColIndex], PlayingGrid[rowIndex][ColIndex + 1]},
                        {
                            PlayingGrid[rowIndex + 1][ColIndex], PlayingGrid[rowIndex + 1][ColIndex + 1]
                        }
                    }),
                    Position = new System.Numerics.Vector2(ColIndex, rowIndex),
                    Color = Brushes.Blue
                });
                ;

                rowIndex += 2;
            }

            StuckTetrominoes = tetrominos;
        }

        public void prettyprint()
        {
            for (int i = 0; i < PlayingGrid.Count; i++)
            {
                for (int j = 0; j < PlayingGrid[0].Count; j++)
                {
                    Debug.Write(string.Format("{0} ", PlayingGrid[i][j]));
                }

                Debug.Write(Environment.NewLine + Environment.NewLine);
            }
        }
    }
}
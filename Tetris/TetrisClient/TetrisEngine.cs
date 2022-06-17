using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows.Media;

namespace TetrisClient
{
    //Wij hebben alleen gebruik gemaakt van linq bij Enum.repeat aangezien wij nergens anders iets moesten selecteren en dan filteren.
    //Dit zou ervoor zorgen dat wij ergens in de code konden kijken of linq te gebruiken is, maar dit maakt naar ons mening de code onoverzichtelijk en daarnaast was het dan linq gebruiken voor het gebruik zodat we punten krijgen, terwijl dit ook (al is het slechts een heel klein beetje) zorgt voor minder snelheid naast naar onze mening onduidelijkheid. 

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
        

        /// <summary>
        /// Dit is de constructor van TetrisEngine.
        /// Het geeft een seed mee aan de TetrominoService(die vervolgens een RandomTetromino eruit spawnt).
        /// Daarnaast wordt er een randomcurrenttetromino aangemaakt.
        /// Ook wordt er een ghostpiece aangemaakt.
        /// Verder wordt het board ook de goede grootte gemaakt.
        /// </summary>
        /// <param name="seed"></param>
        public TetrisEngine(int seed)
        {
            tetrominoService = new TetrominoService(seed);
            CurrentTetromino = tetrominoService.GetRandomTetromino();
            CreateGhostPiece();
            
            for (var i = 0; i < 20; i++)
            {
                PlayingGrid.Add(Enumerable.Repeat(0, 10).ToList());
            }
        }

        /// <summary>
        /// Cleared de lines.
        /// Maakt een nieuwe tetromino aan voor current.
        /// creeert een ghostpiece
        /// </summary>

        public void SpawnTetromino()
        {
            ClearLines();
            CurrentTetromino = tetrominoService.GetRandomTetromino();
            CreateGhostPiece();
        }

        /// <summary>
        /// Maakt een Ghosttetromino aan die variables heeft van currenttetromino
        /// </summary>
        public void CreateGhostPiece() {
            GhostPiece = new Tetromino
            {
                Shape = new Matrix(CurrentTetromino.Shape.Value),
                Position = new Vector2(CurrentTetromino.Position.X, CurrentTetromino.Position.Y + 18),
                Color = Brushes.Gray
            };
        }

        /// <summary>
        /// Voegt de currentTetromino toe aan de playingGrid 
        /// </summary>
        /// <returns>true als ie toegevoegd kan worden. Anders false</returns>
        public bool AddStuck()
        {          
            StuckTetrominoes.Add(CurrentTetromino);
            var shape = CurrentTetromino.Shape.Value;

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
                    
                    if (newYPos > 0)
                    {                       
                        PlayingGrid[newYPos][newXPos] = shape[yOffset, xOffset];                        
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Zet een tetromino neer met een desired position (de positie waar de tetromino heen wilt) en kijkt of die move mogelijk is.
        /// </summary>
        /// <param name="desiredPosition"></param>
        /// <returns>true als move mogelijk is. Anders false</returns>

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

        /// <summary>
        /// Doet hetzelfde als movepossible, maar checkt dan op side moves
        /// </summary>
        /// <param name="desiredPosition"></param>
        /// <returns>returnt true als de move mag. Anders returnt het false;</returns>

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
                    if (newXPos < 0 || (newXPos + 1) > (PlayingGrid[0].Count))
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

        /// <summary>
        /// Leegt volle regels en zet ze aan de bovenkant van de lijst. Vervolgens triggered het de setdropspeed functie.
        /// </summary>
        private void ClearLines()
        {
            var rowsToReplace = new List<List<int>>();
            for (var rowIndex = 0; rowIndex < PlayingGrid.Count; rowIndex++)
            {
                var row = PlayingGrid[rowIndex];

                // check of rij vol is
                if (!row.Contains(0))
                {
                    // reset rij
                    for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
                    {
                        row[columnIndex] = 0;
                    }

                    // voegt rij toe aan lijst
                    rowsToReplace.Add(row);
                }
            }

            // process volle rijen en delete die van de huidige positie van de lijst en zet ze aan top
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

        /// <summary>
        /// kijkt hoe veel rijen er al geleegd zijn en zet op basis daarvan de levels en de dropspeed
        /// </summary>
        /// <param name="clearedLines"></param>

        public void SetDropspeed(int clearedLines)
        {
            var tvar = (clearedLines / 5) + 1;
            if (tvar > Level)
            {
                LevelChanged = true;
                Level = (clearedLines / 5) + 1;
                DropSpeed = Math.Pow(0.8 - ((Level - 1) * 0.007), (Level - 1));
            }
        }

        /// <summary>
        /// Dit is de functie die gedaan wordt na de clear lines methode. Deze tekent de playinggrid volledig in de stucktetrominos door het board op te delen in tetrominos.
        /// </summary>
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
                    // eerste 2x4
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
                }

                ColIndex += 4;
            }

            rowIndex = 0;
            ColIndex = 8;
            //laatste 2
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

                rowIndex += 2;
            }

            StuckTetrominoes = tetrominos;
        }

        
    }
}
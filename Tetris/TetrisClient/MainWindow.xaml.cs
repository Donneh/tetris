﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TetrisClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TetrisEngine engine;
        private DispatcherTimer timer;
        private Boolean gamePlayable = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            var seed =  Guid.NewGuid().GetHashCode();
            engine = new TetrisEngine(seed);
            StartGameLoop();
        }

        private void OnGridLoaded(object sender, EventArgs e) {
            TetrisGrid.Focus();
        }
        /// <summary>
        /// Het aanmaken van een gameloop met dropspeed en een dispatchertimer
        /// </summary>
        private void StartGameLoop()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GameTick);
            timer.Interval = TimeSpan.FromSeconds(engine.DropSpeed);
            timer.Start();
        }

        /// <summary>
        /// Een Gametick die elke keer wanneer deze getriggered wordt wat functies uitvoert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTick(object sender, EventArgs e)
        {
            TetrisGrid.Children.Clear(); 
            DrawGhostPiece(); 
            DrawCurrentTetromino();
            DrawStuckTetrominoes();                     
            MoveDown();
            if (engine.LevelChanged) {
                levelTxt.Text = "Level: " + engine.Level;
                timer.Interval = TimeSpan.FromSeconds(engine.DropSpeed);
                engine.LevelChanged = false;
            }
        }
        
        /// <summary>
        /// Dit is de functie die kijkt welke kant de tetromino heen gaat door de input van de user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveObject(object sender, KeyEventArgs e)
        {
            if (gamePlayable)
            {
                switch (e.Key.ToString())
                {
                    case "Right":
                        DrawGhostPiece();
                        var desiredPosition = new Tetromino
                        {
                            Shape = engine.CurrentTetromino.Shape,
                            Position = engine.CurrentTetromino.Position
                        };
                        desiredPosition.Position.X++;
                        if (engine.SideMovePossible(desiredPosition))
                        {
                            engine.CurrentTetromino.Position = desiredPosition.Position;
                        }
                        break;
                    case "Left":
                        DrawGhostPiece();
                        desiredPosition = new Tetromino
                        {
                            Shape = engine.CurrentTetromino.Shape,
                            Position = engine.CurrentTetromino.Position
                        };
                        desiredPosition.Position.X--;
                        if (engine.SideMovePossible(desiredPosition))
                        {
                            engine.CurrentTetromino.Position = desiredPosition.Position;
                        }
                        break;
                    case "Down":
                        desiredPosition = new Tetromino
                        {
                            Shape = engine.CurrentTetromino.Shape,
                            Position = engine.CurrentTetromino.Position
                        };
                        desiredPosition.Position.Y++;
                        while (engine.MovePossible(desiredPosition))
                        {
                            desiredPosition.Position.Y++;
                        }
                        desiredPosition.Position.Y--;
                        engine.CurrentTetromino.Position = desiredPosition.Position;
                        if (!engine.AddStuck())
                        {
                            timer.Stop();
                            PauseButton.Visibility = Visibility.Hidden;
                            System.Windows.MessageBox.Show("GAME OVER");
                        }
                        break;
                    case "Up":
                        engine.CurrentTetromino.Rotate();
                        engine.GhostPiece.Rotate();
                        break;
                }
            }
                       
        }

        /// <summary>
        /// Beweegt de tetromino naar beneden
        /// </summary>
        private void MoveDown()
        {
            var desiredPosition = new Tetromino
            {
                Shape = engine.CurrentTetromino.Shape,
                Position = engine.CurrentTetromino.Position
            };
            desiredPosition.Position.Y++;
            
            if (engine.MovePossible(desiredPosition))
            {
                engine.CurrentTetromino.Position = desiredPosition.Position;
            }
            else {
                if(!engine.AddStuck()) {
                    timer.Stop();
                    PauseButton.Visibility = Visibility.Hidden;
                    System.Windows.MessageBox.Show("GAME OVER");
                }
                engine.SpawnTetromino();
            }
        }

        /// <summary>
        /// Maakt de tetromino op het board een kleur dmv het cijfer wat de tetromino heeft.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Brush GetColorFromCode(int code) => code switch
        {
            0 => Brushes.Black,
            1 => Brushes.Aqua,
            2 => Brushes.Blue,
            3 => Brushes.Orange,
            4 => Brushes.Yellow,
            5 => Brushes.Lime,
            6 => Brushes.Magenta,
            7 => Brushes.Red,
            8 => Brushes.Gray,
            _ => throw new ArgumentOutOfRangeException(nameof(code), $"Not expected code: {code}")
        };


        /// <summary>
        /// tekent de huidige tetromino in het board
        /// </summary>
        private void DrawCurrentTetromino()
        {
            int[,] values = engine.CurrentTetromino.Shape.Value;         
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {                    
                    if (values[i, j] == 0) continue;
                    var rectangle = new Rectangle()
                    {
                        Width = 25, // Breedte van een 'cell' in de Grid
                        Height = 25, // Hoogte van een 'cell' in de Grid
                        Stroke = Brushes.Black, // De rand
                        StrokeThickness = 2.5, // Dikte van de rand
                        Fill = GetColorFromCode(values[i,j]), // Achtergrondkleur
                    };
                    
                    
                    TetrisGrid.Children.Add(rectangle); // Voeg de rectangle toe aan de Grid
                    Grid.SetRow(rectangle, (int)(i + engine.CurrentTetromino.Position.Y)); // Zet de rij
                    Grid.SetColumn(rectangle, (int)(j + engine.CurrentTetromino.Position.X)); // Zet de kolom
                }
            }
            
        }

        /// <summary>
        /// tekent de ghostpiece in het board
        /// </summary>
        private void DrawGhostPiece() 
        {
            
            var desiredPosition = new Tetromino
            {
                Shape = engine.CurrentTetromino.Shape,
                Position = engine.CurrentTetromino.Position
            };
            desiredPosition.Position.Y++;
            while (engine.MovePossible(desiredPosition))
            {
                desiredPosition.Position.Y++;
            }
            desiredPosition.Position.Y--;
            engine.GhostPiece.Position = desiredPosition.Position;
            int[,] values = engine.GhostPiece.Shape.Value;
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    if (values[i, j] == 0) continue;
                    var rectangle = new Rectangle()
                    {
                        Width = 25, // Breedte van een 'cell' in de Grid
                        Height = 25, // Hoogte van een 'cell' in de Grid
                        Stroke = Brushes.Black, // De rand
                        StrokeThickness = 2.5, // Dikte van de rand
                        Fill = GetColorFromCode(8), // Achtergrondkleur
                    };


                    TetrisGrid.Children.Add(rectangle); // Voeg de rectangle toe aan de Grid
                    Grid.SetRow(rectangle, (int)(i + engine.GhostPiece.Position.Y)); // Zet de rij
                    Grid.SetColumn(rectangle, (int)(j + engine.CurrentTetromino.Position.X)); // Zet de kolom
                }
            }
        }

        /// <summary>
        /// tekent de stuck tetrominos in het board
        /// </summary>
        private void DrawStuckTetrominoes()
        {
            foreach (var tetromino in engine.StuckTetrominoes)
            {
                int[,] values = tetromino.Shape.Value;
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    for (int j = 0; j < values.GetLength(1); j++)
                    {
                        
                        if (values[i, j] == 0) continue;

                        var rectangle = new Rectangle()
                        {
                            Width = 25, // Breedte van een 'cell' in de Grid
                            Height = 25, // Hoogte van een 'cell' in de Grid
                            Stroke = Brushes.Black, // De rand
                            StrokeThickness = 2.5, // Dikte van de rand
                            Fill = GetColorFromCode(values[i, j]), // Achtergrondkleur
                        };

                        TetrisGrid.Children.Add(rectangle); // Voeg de rectangle toe aan de Grid
                        Grid.SetRow(rectangle, (int)(i + tetromino.Position.Y)); // Zet de rij
                        Grid.SetColumn(rectangle, (int)(j + tetromino.Position.X)); // Zet de kolom
                    }
                }
            }
        }

        /// <summary>
        /// functie die de pauzeknop regelt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseClick(object sender, RoutedEventArgs e)
        {
            
            timer.IsEnabled = !timer.IsEnabled;
            gamePlayable = !gamePlayable;
            if (PauseButton.Content.Equals("Pause"))
            {
                PauseButton.Content = "Play";
            }
            else {
                
                PauseButton.Content = "Pause";
            }
        }
    }
}

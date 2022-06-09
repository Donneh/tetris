using System;
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

        public MainWindow()
        {
            InitializeComponent();            
            engine = new TetrisEngine();
            StartGameLoop();
        }

        private void OnGridLoaded(object sender, EventArgs e) {
            TetrisGrid.Focus();
        }

        private void StartGameLoop()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GameTick);
            timer.Interval = new TimeSpan(0, 0,0, 0, engine.dropSpeedInMilliSeconds);
            timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            TetrisGrid.Children.Clear();
            DrawCurrentTetromino();
            DrawStuckTetrominoes();
            engine.DrawInArray();
            
            MoveDown();
            //engine.ClearLines();
            //engine.RemoveTetrominoPart();
            //engine.printTetrominoPosition();
            ;
        }
        
        private void MoveObject(object sender, KeyEventArgs e)
        {            
            
            switch (e.Key.ToString()) {
                case "Right":
                    var desiredPosition = new Tetromino
                    {
                        Shape = engine.currentTetromino.Shape,
                        Position = engine.currentTetromino.Position
                    };
                    desiredPosition.Position.X++;
                    if (engine.SideMovePossible(desiredPosition))
                    {
                        engine.currentTetromino.Position = desiredPosition.Position;
                    }
                    break;
                case "Left":
                    desiredPosition = new Tetromino
                    {
                        Shape = engine.currentTetromino.Shape,
                        Position = engine.currentTetromino.Position
                    };
                    desiredPosition.Position.X--;
                    if (engine.SideMovePossible(desiredPosition))
                    {
                        engine.currentTetromino.Position = desiredPosition.Position;
                    }
                    break;
                case "Down":
                    desiredPosition = new Tetromino
                    {
                        Shape = engine.currentTetromino.Shape,
                        Position = engine.currentTetromino.Position
                    };
                    desiredPosition.Position.Y++;
                    while (engine.MovePossible(desiredPosition))
                    {
                        desiredPosition.Position.Y++;
                    }
                    desiredPosition.Position.Y--;
                    engine.currentTetromino.Position = desiredPosition.Position;
                    engine.AddStuck();
                    break;
                case "Up": 
                    engine.currentTetromino.Rotate();
                    break;
            }
                       
        }

        private void MoveDown()
        {
            var desiredPosition = new Tetromino
            {
                Shape = engine.currentTetromino.Shape,
                Position = engine.currentTetromino.Position
            };
            desiredPosition.Position.Y++;

            if (engine.MovePossible(desiredPosition))
            {
                engine.currentTetromino.Position = desiredPosition.Position;
            }
            else {
                engine.AddStuck();
                engine.SpawnTetromino();
            }
        }

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
            8 => Brushes.Black,
            _ => throw new ArgumentOutOfRangeException(nameof(code), $"Not expected code: {code}")
        };

        private void DrawCurrentTetromino()
        {
            int[,] values = engine.currentTetromino.Shape.Value;         
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
                    Grid.SetRow(rectangle, (int)(i + engine.currentTetromino.Position.Y)); // Zet de rij
                    Grid.SetColumn(rectangle, (int)(j + engine.currentTetromino.Position.X)); // Zet de kolom
                }
            }
            
        }

        private void DrawStuckTetrominoes()
        {
            foreach (var tetromino in engine.stuckTetrominoes)
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


        private void PauseClick(object sender, RoutedEventArgs e)
        {
            
            timer.IsEnabled = !timer.IsEnabled;
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

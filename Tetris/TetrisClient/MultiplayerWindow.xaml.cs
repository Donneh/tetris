﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows.Shapes;

namespace TetrisClient
{
    public partial class MultiplayerWindow : Window
    {
        private HubConnection _connection;
        private Random P1Random;
        private Random P2Random;
        private readonly TetrisEngine engine;
        private DispatcherTimer timer;
        
        public MultiplayerWindow()
        {
            InitializeComponent();

            // De url waar de meegeleverde TetrisHub op draait:
            string url = "http://127.0.0.1:5000/TetrisHub"; 
            
            // De Builder waarmee de connectie aangemaakt wordt:
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();
            
            // De eerste paramater moet gelijk zijn met de methodenaam in TetrisHub.cs
            // Wat er tussen de <..> staat bepaald wat de type van de paramater `seed` is.
            // Op deze manier loopt het onderstaande gelijk met de methode in TetrisHub.cs.
            _connection.On<int>("ReadyUp", seed =>
            {
                // Seed van de andere client:
                P2Random = new Random(seed);
                MessageBox.Show(seed.ToString());
            });
            
            // Let op: het starten van de connectie moet *nadat* alle event listeners zijn gezet!
            // Als de methode waarin dit voorkomt al `async` (asynchroon) is, dan kan `Task.Run` weggehaald worden.
            // In het startersproject staat dit in de constructor, daarom is dit echter wel nodig:
            Task.Run(async () => await _connection.StartAsync());
            
            engine = new TetrisEngine();
            StartGameLoop();
        }

        // Events kunnen `async` zijn in WPF:
        private async void StartGame_OnClick(object sender, RoutedEventArgs e)
        {
            // Als de connectie nog niet is geïnitialiseerd, dan kan er nog niks verstuurd worden:
            if (_connection.State != HubConnectionState.Connected)
            {
                return;
            }
            
            int seed = Guid.NewGuid().GetHashCode();
            
            P1Random = new Random(seed);

            // Het aanroepen van de TetrisHub.cs methode `ReadyUp`.
            // Hier geven we de int mee die de methode `ReadyUp` verwacht.
            await _connection.InvokeAsync("ReadyUp", seed);
        }
        
        private void OnGridLoaded(object sender, EventArgs e) {
            TetrisGrid.Focus();
        }

        private void StartGameLoop()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GameTick);
            timer.Interval = TimeSpan.FromSeconds(engine.dropSpeed);
            timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            TetrisGrid.Children.Clear(); 
            DrawGhostPiece(); 
            DrawCurrentTetromino();
            DrawStuckTetrominoes();                     
            MoveDown();
            if (engine.levelChanged) {
                levelTxt.Text = "Level: " + engine.level;
                timer.Interval = TimeSpan.FromSeconds(engine.dropSpeed);
                engine.levelChanged = false;
            }
            ;
        }
        
        private void MoveObject(object sender, KeyEventArgs e)
        {            
            
            switch (e.Key.ToString()) {
                case "Right":
                    DrawGhostPiece();
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
                    DrawGhostPiece();
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
                    if (!engine.AddStuck()) {
                        timer.Stop();
                        PauseButton.Visibility = Visibility.Hidden;
                        System.Windows.MessageBox.Show("GAME OVER");
                    }
                    break;
                case "Up": 
                    engine.currentTetromino.Rotate();
                    engine.ghostPiece.Rotate(); 
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
                if(!engine.AddStuck()) {
                    timer.Stop();
                    PauseButton.Visibility = Visibility.Hidden;
                    System.Windows.MessageBox.Show("GAME OVER");
                }
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
            8 => Brushes.Gray,
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


        private void DrawGhostPiece() 
        {
            
            var desiredPosition = new Tetromino
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
            engine.ghostPiece.Position = desiredPosition.Position;
            int[,] values = engine.ghostPiece.Shape.Value;
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
                    Grid.SetRow(rectangle, (int)(i + engine.ghostPiece.Position.Y)); // Zet de rij
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;

namespace TetrisClient
{
    public partial class MultiplayerWindow : Window
    {
        private HubConnection _connection;

        private TetrisEngine _engine;
        private List<List<int>> gridWithCurrent = new();
        private DispatcherTimer timer;
        public bool gamePlayable = true;

        /// <summary>
        /// tekent de playinggrid in een nieuwe grid. Dit is gedaan zodat presentatie en taakspecifieke logica in het board gescheuden zijn.
        /// </summary>
        private void ListTolist() 
        {
            gridWithCurrent = new List<List<int>>();
            foreach (var row in _engine.PlayingGrid) 
            {
                gridWithCurrent.Add(new(row));
            }
            DrawCurrentInGridWithCurrent();
        }

        /// <summary>
        /// tekent de currenttetromino in de nieuwe grid.
        /// </summary>
        private void DrawCurrentInGridWithCurrent() {
            int[,] values = _engine.CurrentTetromino.Shape.Value;
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


                    gridWithCurrent[(int)(i + _engine.CurrentTetromino.Position.Y)][(int)(j + _engine.CurrentTetromino.Position.X)] = values[i, j]; 
                }
            }
        }

        public MultiplayerWindow()
        {           
            InitializeComponent();

            // De url waar de meegeleverde TetrisHub op draait:
            const string url = "http://127.0.0.1:5000/TetrisHub"; 
            
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
                _engine = new TetrisEngine(seed);
            });

            _connection.On<List<List<int>>>("UpdateGrid", grid =>
            {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    DrawGrid(grid, PlayerTwoGrid);
                });
            });
            
            // Let op: het starten van de connectie moet *nadat* alle event listeners zijn gezet!
            // Als de methode waarin dit voorkomt al `async` (asynchroon) is, dan kan `Task.Run` weggehaald worden.
            // In het startersproject staat dit in de constructor, daarom is dit echter wel nodig:
            Task.Run(async () => await _connection.StartAsync());
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
            
            
            
            _engine = new TetrisEngine(seed);
            StartGameLoop();
            // Het aanroepen van de TetrisHub.cs methode `ReadyUp`.
            // Hier geven we de int mee die de methode `ReadyUp` verwacht.
            await _connection.InvokeAsync("ReadyUp", seed);
        }

        private async void SendBoard()
        {
            ListTolist();
            await _connection.InvokeAsync("UpdateGrid", gridWithCurrent);
        }

        private void OnGridLoaded(object sender, RoutedEventArgs e)
        {
            TetrisGrid.Focus();
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
        
        private void DrawGrid(List<List<int>> grid, Grid target)
        {
            for (var row = 0; row < grid.Count; row++)
            {
                for (var col = 0; col < grid[row].Count; col++)
                {
                 //   if (grid[row][col] == 0) continue;
                    
                    var rectangle = new Rectangle()
                    {
                        Width = 25, // Breedte van een 'cell' in de Grid
                        Height = 25, // Hoogte van een 'cell' in de Grid
                        Stroke = Brushes.Black, // De rand
                        StrokeThickness = 2.5, // Dikte van de rand
                        Fill = GetColorFromCode(grid[row][col]), // Achtergrondkleur
                    };
                    
                    target.Children.Add(rectangle); // Voeg de rectangle toe aan de Grid
                    Grid.SetRow(rectangle, row); // Zet de rij
                    Grid.SetColumn(rectangle, col); // Zet de kolom
                }
            }
        }

        
        private void StartGameLoop()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GameTick);
            timer.Interval = TimeSpan.FromSeconds(_engine.DropSpeed);
            timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            TetrisGrid.Children.Clear(); 
            DrawCurrentTetromino();
            DrawGhostPiece(); 
            DrawStuckTetrominoes();                     
            MoveDown();
            if (_engine.LevelChanged) {
                levelTxt.Text = "Level: " + _engine.Level;
                timer.Interval = TimeSpan.FromSeconds(_engine.DropSpeed);
                _engine.LevelChanged = false;
            }

            SendBoard();
        }

        

        private void MoveObject(object sender, KeyEventArgs e)
        {
            switch (e.Key.ToString()) {
                case "Right":
                    DrawGhostPiece();
                    var desiredPosition = new Tetromino
                    {
                        Shape = _engine.CurrentTetromino.Shape,
                        Position = _engine.CurrentTetromino.Position
                    };
                    desiredPosition.Position.X++;
                    if (_engine.SideMovePossible(desiredPosition))
                    {
                        _engine.CurrentTetromino.Position = desiredPosition.Position;
                    }
                    break;
                case "Left":
                    DrawGhostPiece();
                    desiredPosition = new Tetromino
                    {
                        Shape = _engine.CurrentTetromino.Shape,
                        Position = _engine.CurrentTetromino.Position
                    };
                    desiredPosition.Position.X--;
                    if (_engine.SideMovePossible(desiredPosition))
                    {
                        _engine.CurrentTetromino.Position = desiredPosition.Position;
                    }
                    break;
                case "Down":
                    desiredPosition = new Tetromino
                    {
                        Shape = _engine.CurrentTetromino.Shape,
                        Position = _engine.CurrentTetromino.Position
                    };
                    desiredPosition.Position.Y++;
                    while (_engine.MovePossible(desiredPosition))
                    {
                        desiredPosition.Position.Y++;
                    }
                    desiredPosition.Position.Y--;
                    _engine.CurrentTetromino.Position = desiredPosition.Position;
                    if (!_engine.AddStuck()) {
                        timer.Stop();
                        // PauseButton.Visibility = Visibility.Hidden;
                        System.Windows.MessageBox.Show("GAME OVER");
                    }
                    break;
                case "Up": 
                    _engine.CurrentTetromino.Rotate();
                    _engine.GhostPiece.Rotate(); 
                    break;
            }
                       
        }
        
        private void MoveDown()
        {
            var desiredPosition = new Tetromino
            {
                Shape = _engine.CurrentTetromino.Shape,
                Position = _engine.CurrentTetromino.Position
            };
            desiredPosition.Position.Y++;

            

            if (_engine.MovePossible(desiredPosition))
            {
                _engine.CurrentTetromino.Position = desiredPosition.Position;
            }
            else {
                if(!_engine.AddStuck()) {
                    timer.Stop();
                    // PauseButton.Visibility = Visibility.Hidden;
                    System.Windows.MessageBox.Show("GAME OVER");
                }
                _engine.SpawnTetromino();
            }

        }
        
        private void DrawCurrentTetromino()
        {
            int[,] values = _engine.CurrentTetromino.Shape.Value;         
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
                    Grid.SetRow(rectangle, (int)(i + _engine.CurrentTetromino.Position.Y)); // Zet de rij
                    Grid.SetColumn(rectangle, (int)(j + _engine.CurrentTetromino.Position.X)); // Zet de kolom
                }
            }
            
        }


        private void DrawGhostPiece() 
        {
            
            var desiredPosition = new Tetromino
            {
                Shape = _engine.CurrentTetromino.Shape,
                Position = _engine.CurrentTetromino.Position
            };
            desiredPosition.Position.Y++;
            while (_engine.MovePossible(desiredPosition))
            {
                desiredPosition.Position.Y++;
            }
            desiredPosition.Position.Y--;
            _engine.GhostPiece.Position = desiredPosition.Position;
            int[,] values = _engine.GhostPiece.Shape.Value;
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
                    Grid.SetRow(rectangle, (int)(i + _engine.GhostPiece.Position.Y)); // Zet de rij
                    Grid.SetColumn(rectangle, (int)(j + _engine.CurrentTetromino.Position.X)); // Zet de kolom
                }
            }
        }

        private void DrawStuckTetrominoes()
        {
            foreach (var tetromino in _engine.StuckTetrominoes)
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
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.AspNetCore.SignalR.Client;

namespace TetrisClient
{
    public partial class MultiplayerWindow : Window
    {
        private HubConnection _connection;
        private Random P1Random;
        private Random P2Random;

        private TetrisEngine _engine;
        
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
            
            P1Random = new Random(seed);

            // Het aanroepen van de TetrisHub.cs methode `ReadyUp`.
            // Hier geven we de int mee die de methode `ReadyUp` verwacht.
            await _connection.InvokeAsync("ReadyUp", seed);

            await _connection.InvokeAsync("UpdateGrid", _engine.playingGrid);
        }

        private void MoveObject(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("MoveObject()");
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
                    if (grid[row][col] == 0) continue;
                    
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
    }
}

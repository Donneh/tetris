using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace TetrisClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TetrisEngine engine;
        
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
            var timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GameTick);
            timer.Interval = new TimeSpan(0, 0,0, 0, engine.dropSpeedInMilliSeconds);
            timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            TetrisGrid.Children.Clear();
            DrawCurrentTetromino();
            DrawStuckTetrominoes();
            MoveDown();
        }
        
        private void MoveObject(object sender, KeyEventArgs e)
        {            
            
            switch (e.Key.ToString()) {
                case "Right":                 
                    var desiredPosition = new Tetromino();
                    desiredPosition.Shape = engine.currentTetromino.Shape;
                    desiredPosition.Position = engine.currentTetromino.Position;
                    desiredPosition.Position.X++;
                    if (engine.MovePossible(desiredPosition))
                    {
                        engine.currentTetromino.Position = desiredPosition.Position;
                    }
                    break;
                case "Left":
                    desiredPosition = new Tetromino();
                    desiredPosition.Shape = engine.currentTetromino.Shape;
                    desiredPosition.Position = engine.currentTetromino.Position;
                    desiredPosition.Position.X--;
                    if (engine.MovePossible(desiredPosition))
                    {
                        engine.currentTetromino.Position = desiredPosition.Position;
                    }
                    break;
                case "Up":                   
                    engine.currentTetromino.Rotate();
                    break;

            }
                       
        }

        private void MoveDown()
        {
            var desiredPosition = new Tetromino();
            desiredPosition.Shape = engine.currentTetromino.Shape;
            desiredPosition.Position = engine.currentTetromino.Position;
            desiredPosition.Position.Y++;
            if (engine.MovePossible(desiredPosition))
            {
                engine.currentTetromino.Position = desiredPosition.Position;
            }
        }

        private void DrawCurrentTetromino()
        {
            int[,] values = engine.currentTetromino.Shape.Value;         
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {                    
                    if (values[i, j] != 1) continue;
                   
                    var rectangle = engine.currentTetromino.ToRectangle();
                    
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
                        
                        if (values[i, j] != 1) continue;

                        var rectangle = tetromino.ToRectangle();

                        TetrisGrid.Children.Add(rectangle); // Voeg de rectangle toe aan de Grid
                        Grid.SetRow(rectangle, (int)(i + tetromino.Position.Y)); // Zet de rij
                        Grid.SetColumn(rectangle, (int)(j + tetromino.Position.X)); // Zet de kolom
                    }
                }
            }
        }
    }
}

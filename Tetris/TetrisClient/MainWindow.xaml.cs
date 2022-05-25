﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TetrisClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TetrisEngine engine;
        private DispatcherTimer timer;
        
        public MainWindow()
        {
            InitializeComponent();            
            engine = new TetrisEngine();
            timer = new DispatcherTimer();

            engine.CheckCollision();
            StartGameLoop();
            
        }


        private void moveObject(Object sender, KeyEventArgs e) {   
            
            switch (e.Key.ToString()) {
                case "Right":
                    if (!(engine.CheckSideCollision() == true)) {
                    engine.currentTetromino.Position.X++;
                    }                   
                    break;
                case "Left":                  
                    if (!(engine.CheckSideCollision() == true))
                    {
                        engine.currentTetromino.Position.X--;
                    }                   

                    break;
                case "Down":
                    if (engine.CheckCollision() == true)
                    {
                        engine.SpawnTetromino();
                    }
                    else {
                    engine.currentTetromino.Position.Y++;
                    }                   
                    break;
                case "Up":                   
                    engine.currentTetromino.Rotate();
                    break;

            }
                       
        }

        void StartGameLoop()
        {
            var timer = new DispatcherTimer();
            timer.Tick += new EventHandler(GameTick);
            timer.Interval = new TimeSpan(0, 0, engine.dropSpeedInSeconds);
            timer.Start();
        }

        private void GameTick(object sender, EventArgs e)
        {
            TetrisGrid.Children.Clear();
            MoveDown();
            Draw();
            DrawStuck();
        }

        private void OnGridLoaded(object sender, EventArgs e) {
            TetrisGrid.Focus();
        
        }

        private void MoveDown()
        {
            if (!engine.CheckCollision() == true)
            {                
                engine.currentTetromino.Position.Y++;
            }
            else {
                engine.SpawnTetromino();
            }
            
        }

        void Draw()
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

        void DrawStuck()
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

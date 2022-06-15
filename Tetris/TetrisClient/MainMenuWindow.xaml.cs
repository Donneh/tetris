using System.Windows;

namespace TetrisClient
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void StartSinglePlayer(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();

            this.Close();
        }

        private void StartMultiPlayer(object sender, RoutedEventArgs e)
        {
            var window = new MultiplayerWindow();
            window.Show();

            this.Close();
        }
    }
}
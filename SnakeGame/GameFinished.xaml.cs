using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for GameFinished.xaml
    /// </summary>
    public partial class GameFinished : Window
    {
        private int _score;
        public GameFinished(int score)
        {
            InitializeComponent();
            _score = score;
            txtGameOver.Text = $"Game Over! Your final score: {_score}";
        }

        private void btnTryAgain_Click(object sender, RoutedEventArgs e)
        {
            // Get all hidden windows
         
            var gameWindow = new MainWindow();
            gameWindow.Show();
            this.Close();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

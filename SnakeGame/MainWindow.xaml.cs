using SnakeGame.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // This list describes the Bonus Red pieces of Food on the Canvas
        private readonly List<Point> _bonusPoints = new List<Point>();
        // This list describes the body of the snake on the Canvas
        private readonly List<Point> _snakePoints = new List<Point>();
        private BackgroundWorker _movementWorker;
        private readonly Brush _snakeColor = Brushes.Fuchsia;
        private bool _gameOver = false;
        private readonly Point _startingPoint = new Point(100, 100);
        private Point _currentPosition = new Point();

        // Movement direction initialisation
        private  int _direction = (int)Movingdirection.Downwards;

        /* Placeholder for the previous movement direction
         * The snake needs this to avoid its own body.  */
        private int _previousDirection = 0;

        /* Here user can change the size of the snake. 
         * Possible sizes are THIN, NORMAL and THICK */
        private readonly int _headSize = (int)SnakeSize.Thick;

        private int _length = 100;
        private int _score = 0;
        private readonly Random _rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            /* Here user can change the speed of the snake. 
             * Possible speeds are FAST, MODERATE, SLOW and DAMNSLOW */
            _movementWorker = new BackgroundWorker();
            _movementWorker.WorkerSupportsCancellation = true;
            _movementWorker.DoWork += MovementWorker_DoWork;
            _movementWorker.RunWorkerAsync();

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            PaintSnake(_startingPoint);
            _currentPosition = _startingPoint;
            // Instantiate Food Objects
            PaintBonus(0);
        }

        private void PaintSnake(Point currentposition)
        {

            // This method is used to paint a frame of the snake´s body each time it is called.

            Ellipse newEllipse = new Ellipse
            {
                Fill = _snakeColor,
                Width = _headSize,
                Height = _headSize
            };

            Canvas.SetTop(newEllipse, currentposition.Y);
            Canvas.SetLeft(newEllipse, currentposition.X);

            int count = PaintCanvas.Children.Count;
            PaintCanvas.Children.Add(newEllipse);
            _snakePoints.Add(currentposition);

            // Restrict the tail of the snake
            if (count > _length)
            {
                PaintCanvas.Children.RemoveAt(count - _length);
                _snakePoints.RemoveAt(count - _length);
            }
        }

        private void PaintBonus(int index)
        {
            Point bonusPoint = new Point(_rnd.Next(5, 780), _rnd.Next(5, 480));

            Ellipse newEllipse = new Ellipse
            {
                Fill = Brushes.Red,
                Width = _headSize,
                Height = _headSize
            };

            Canvas.SetTop(newEllipse, bonusPoint.Y);
            Canvas.SetLeft(newEllipse, bonusPoint.X);
            PaintCanvas.Children.Insert(index, newEllipse);
            _bonusPoints.Insert(index, bonusPoint);

        }


        private void MovementWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_gameOver)
            {
                // Expand the body of the snake in the current direction of movement
                switch (_direction)
                {
                    case (int)Movingdirection.Downwards:
                        _currentPosition.Y++;
                        break;
                    case (int)Movingdirection.Upwards:
                        _currentPosition.Y--;
                        break;
                    case (int)Movingdirection.Toleft:
                        _currentPosition.X--;
                        break;
                    case (int)Movingdirection.Toright:
                        _currentPosition.X++;
                        break;
                }

                // Update the UI on the main thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PaintSnake(_currentPosition);
                    // Restrict to boundaries of the Canvas
                    CanvasBoudaries();

                    // Hitting a bonus Point causes the lengthen-Snake Effect
                    BonusPointHit();

                    // Restrict hits to body of Snake
                    RestrictHitsToBodyOfSnake();
                });

                // Adjust the game speed; ( less heavy on the cpu)
                Thread.Sleep((int)GameSpeed.Fast);
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    _direction = (int)Movingdirection.Downwards;
                    break;
                case Key.Up:
                    _direction = (int)Movingdirection.Upwards;
                    break;
                case Key.Left:
                    _direction = (int)Movingdirection.Toleft;
                    break;
                case Key.Right:
                    _direction = (int)Movingdirection.Toright;
                    break;
                default:
                    return; // Exit the method if the key is not handled
            }
            _previousDirection = _direction;
        }
        private void GameOver()
        {
            _gameOver = true;
            // stop the timer on game over
            _movementWorker.CancelAsync();
            // show the gameOver windows and inject the score
            var gameOver = new GameFinished(_score);
            gameOver.Show();
            this.Close();
        }

        #region snakeBoundariesAndPointsLogic
        private void CanvasBoudaries()
        {
            if ((_currentPosition.X < 5) || (_currentPosition.X > 780) ||
                (_currentPosition.Y < 5) || (_currentPosition.Y > 480))
                GameOver();
        }

        private void BonusPointHit()
        {
            int n = 0;
            foreach (Point point in _bonusPoints)
            {

                if ((Math.Abs(point.X - _currentPosition.X) < _headSize) &&
                    (Math.Abs(point.Y - _currentPosition.Y) < _headSize))
                {
                    _length += 10;
                    _score += 10;

                    // In the case of food consumption, erase the food object
                    // from the list of bonuses as well as from the canvas
                    _bonusPoints.RemoveAt(n);
                    PaintCanvas.Children.RemoveAt(n);
                    PaintBonus(n);
                    break;
                }
                n++;
            }
        }
        private void RestrictHitsToBodyOfSnake()
        {
            for (int q = 0; q < (_snakePoints.Count - _headSize * 2); q++)
            {
                Point point = new Point(_snakePoints[q].X, _snakePoints[q].Y);
                if ((Math.Abs(point.X - _currentPosition.X) < (_headSize)) &&
                     (Math.Abs(point.Y - _currentPosition.Y) < (_headSize)))
                {
                    GameOver();
                    break;
                }

            }
        }
        #endregion
    }
}

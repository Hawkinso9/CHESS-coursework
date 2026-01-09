using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CHESS_coursework
{
    /// <summary> 
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChessGame chessgame;
        private bool gamestarted;
        private Button[,] buttons = new Button[8, 8];
        private Tuple<int, int>? selectedSquare = null;//this tracks square the user selected first(piece they will move)
        private bool[,] Highlighted = new bool[8, 8]; //used to highlight possible moves
        public MainWindow()
        {
            chessgame = new ChessGame();
            InitializeComponent();
            gamestarted = false;
        }
        public void Moving_Piece(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var position = (Tuple<int, int>)button.Tag;  //get the position of the button clicked
            int x = position.Item1;
            int y = position.Item2;
            
            if (selectedSquare == null)
            {
                // First click: select piece
                bool[,]? possMoves = chessgame.SelectedSquares(x, y);//gets possible moves for the selected piece
                if (possMoves != null) //if there are possible moves (my function returns null if no moves)
                {
                    selectedSquare = Tuple.Create(x, y); //store the selected square
                    Highlighted = possMoves;
                    for (int i = 0; i < 8; i++) //loops through the bool array to highlight poss moves
                    {
                        for(int j = 0; j < 8; j++)
                        {

                            if (Highlighted[i, j])
                            {
                                buttons[i, j].Background = Brushes.Blue; //if it is a possible move highlight it
                            }
                            else
                            {
                                buttons[i, j].Background = Brushes.Transparent; //else make it transparent
                            }
                        }
                    }
                }
            }
            else
            {
                // Second click: try to move
                int startX = selectedSquare.Item1; //get the starting coordinates from the selected square
                int startY = selectedSquare.Item2;
                if (Highlighted[x, y]) //checks if the selected square is a possible move
                {
                    chessgame.PlayMove(startX, startY, x, y); //plays the move
                    updateGUI(); //updates GUI
                }
                for (int i = 0; i < 8; i++) 
                {
                    for (int j = 0; j < 8; j++)
                    {
                        buttons[i, j].Background = Brushes.Transparent; //resets all button backgrounds
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Highlighted[i,j] = false; //resets highlighted array
                    }
                }
                selectedSquare = null; //resets selected square so logic works 

            }
        }

        private void InitialiseClick(object sender, RoutedEventArgs e) //starts the board
        {
            if (gamestarted) { return; } //prevents multiple clicks on the button
            gamestarted = true;
            InitialiseButton.Opacity = 0; //makes the button invisible after clicking it
            for (int x = 0; x < 8; x++) //this creates the buttons on the grid
            {
                for (int y = 0; y < 8; y++)
                {
                    buttons[x, y] = new Button
                    {
                        Background = Brushes.Transparent, //makes button clear so they still work when invisible
                        Opacity = 1,
                        Tag = new Tuple<int, int>(x, y),

                    };
                    buttons[x, y].Click += new RoutedEventHandler(Moving_Piece);
                    Grid.SetRow(buttons[x, y], x);
                    Grid.SetColumn(buttons[x, y], y);
                    chessGrid.Children.Add(buttons[x, y]);
                }
            }
            updateGUI();
        }

        public void updateGUI()
        {
            Piece[,] board = chessgame.GetBoard();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = board[x, y];
                    Button btn = buttons[x, y];

                    if (piece is Pawn && piece.Color == PieceColor.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_plt60.png")) // the pack:application:,,, tells wpf to look for a resource file in the project
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Pawn && piece.Color == PieceColor.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_pdt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Rook && piece.Color == PieceColor.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_rlt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Rook && piece.Color == PieceColor.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_rdt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Knight && piece.Color == PieceColor.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_nlt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Knight && piece.Color == PieceColor.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_ndt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Bishop && piece.Color == PieceColor.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_blt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Bishop && piece.Color == PieceColor.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_bdt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece.GetType() == typeof(Queen) && piece.Color == PieceColor.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_qlt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Queen && piece.Color == PieceColor.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_qdt60.png"))
                        };
                        btn.Opacity = 01;
                    }
                    else if (piece is King && piece.Color == PieceColor.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_klt60.png"))
                        };
                        btn.Opacity = 01;
                    }
                    else if (piece is King && piece.Color == PieceColor.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_kdt60.png"))
                        };
                        btn.Opacity = 01;
                    }
                    else if (piece is Empty)
                    {
                        btn.Content = null;
                        btn.Opacity = 01;
                    }
                }
            }
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {

                }
            }
        }

    }
}
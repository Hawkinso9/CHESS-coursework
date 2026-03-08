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
    ///  UI only: buttons, highlights, showing messages, reading user clicks
    //  Should NOT contain chess rules or board logic
    //  Should NOT call Board directly Should call ChessGame public methods
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChessGame chessgame;
        private bool gamestarted;
        private Button[,] buttons = new Button[8, 8];
        private Tuple<int, int>? selectedSquare = null;//this tracks square the user selected first(piece they will move)
        private bool[,] highlighted = new bool[8, 8]; //used to highlight possible moves
        public MainWindow()
        {
            chessgame = new ChessGame();
            InitializeComponent();
            gamestarted = false;
        }
        // TODO (later): chessgame should be constructed with settings selected in the pre-game lobby
        //  time control, handicap settings, load game id, etc
        private void MovingPiece(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var position = (Tuple<int, int>)button.Tag;  //get the position of the button clicked
            int x = position.Item1;
            int y = position.Item2;


            if (selectedSquare == null)
            {

                bool[,] possMoves = chessgame.LegalSquares(x, y);//gets possible moves for the selected piece

                if (possMoves == null)
                {
                    // board handled a second-click move — clear GUI selection/highlights
                    selectedSquare = null;
                    ClearHighlights(); // implement helper to reset Highlighted and button backgrounds
                    updateGUI();
                    return;
                }
                // First click: select piece
                selectedSquare = Tuple.Create(x, y); //store the selected square
                highlighted = possMoves;
                for (int i = 0; i < 8; i++) //loops through the bool array to highlight poss moves
                {
                    for (int j = 0; j < 8; j++)
                    {

                        if (highlighted[i, j])
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



            else
            {
                // Second click: try to move
                int startX = selectedSquare.Item1; //get the starting coordinates from the selected square
                int startY = selectedSquare.Item2;
                if (startX == x && startY == y) //error handling for if the user clicks the same square again
                {

                    return;
                }
                if (highlighted[x, y])
                {
                    bool movevalid = chessgame.TryMove(startX, startY, x, y);

                    if (movevalid)
                    {
                        updateGUI(); //  always refresh after a successful move

                        if (chessgame.awaitingPromotion)
                        {
                            Piece queen = new Queen(chessgame.currentTurn);
                            chessgame.PromotePawn(queen);
                            updateGUI(); //  refresh again after promotion
                        }
                    }

                    ClearHighlights();
                    selectedSquare = null;
                }
                else
                {
                    
                    ClearHighlights();
                    selectedSquare = null;
                }
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
                    buttons[x, y].Click += new RoutedEventHandler(MovingPiece);
                    Grid.SetRow(buttons[x, y], x);
                    Grid.SetColumn(buttons[x, y], y);
                    chessGrid.Children.Add(buttons[x, y]);
                }
            }
            updateGUI();
        }
        private void UndoMove(object sender, RoutedEventArgs e)
        {
            
                chessgame.UndoLastMove();
            
            updateGUI();
        }
        private void ClearHighlights()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    buttons[i, j].Background = Brushes.Transparent; // resets all button backgrounds
                    highlighted[i, j] = false; // resets highlighted array
                }
            }
        }
        private void updateGUI()
       
        {
            Piece[,] board = chessgame.GetBoard();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = board[x, y];
                    Button btn = buttons[x, y];

                    if (piece is Pawn && piece.Colour == PieceColour.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_plt60.png")) // the pack:application:,,, tells wpf to look for a resource file in the project
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Pawn && piece.Colour == PieceColour.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_pdt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Rook && piece.Colour == PieceColour.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_rlt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Rook && piece.Colour == PieceColour.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_rdt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Knight && piece.Colour == PieceColour.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_nlt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Knight && piece.Colour == PieceColour.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_ndt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Bishop && piece.Colour == PieceColour.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_blt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Bishop && piece.Colour == PieceColour.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_bdt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece.GetType() == typeof(Queen) && piece.Colour == PieceColour.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_qlt60.png"))
                        };
                        btn.Opacity = 1;
                    }
                    else if (piece is Queen && piece.Colour == PieceColour.black)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_qdt60.png"))
                        };
                        btn.Opacity = 01;
                    }
                    else if (piece is King && piece.Colour == PieceColour.white)
                    {
                        btn.Content = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Images/Chess_klt60.png"))
                        };
                        btn.Opacity = 01;
                    }
                    else if (piece is King && piece.Colour == PieceColour.black)
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
using System.Windows;

namespace CHESS_coursework
{
    class ChessGame
    {
        // chessgame should: have the turn control, check for checkmate, stalemate, draw conditions and handle moving pieces on the board
        private Board b;
        private Stack<Board> boardStack;

        public ChessGame()
        {
            b = new Board();
            boardStack = new Stack<Board>();
            
            
        }
        
        private bool isGameOver = false;
        public bool awaitingPromotion { get; private set; } = false;
        private int promoX;
        private int promoY;


        public PieceColour currentTurn { get; private set; } = PieceColour.white;
        public bool TryMove(int startX, int startY, int endX, int endY) 
        {
            Piece testpiece = GetPieceFromBoard(startX, startY);
            if (testpiece is Empty || testpiece.Colour != currentTurn)
            {
                return false; // No piece to move or not player's turn
            }
            boardStack.Push(b.Clone()); // makes a clone of tghe current board state
            bool applied = b.ApplyMove(startX, startY, endX, endY); //this will return a record even if the move is invalid atm
            
            
            if (!applied) // If move is invalid, revert to previous board state
            {


                b = boardStack.Pop(); 
                return false;
            }
            bool check = b.IsInCheck(currentTurn); //check if the current player is in check after the move
            if (check)
            {
                b = boardStack.Pop();
                return false;
            }
            if (b.IsPawnPromotion(endX, endY)) 
            {
                awaitingPromotion = true;
                promoX = endX;
                promoY = endY;
                return true; // UI handles promotion
            }
            PieceColour oppcolour = (currentTurn == PieceColour.white) ? PieceColour.black : PieceColour.white;
            bool checkopponent = b.IsInCheck((currentTurn == PieceColour.white) ? PieceColour.black : PieceColour.white);//check if the opponent is in check after the move
            if (checkopponent)
            {
                if (!b.HasAnyLegalMoves(oppcolour))
                {
                    MessageBox.Show("Checkmate jit");
                    isGameOver = true; //stops game as its checkmate
                    
                    currentTurn = (currentTurn == PieceColour.white) ? PieceColour.black : PieceColour.white;
                    return true;
                }
                MessageBox.Show(" you are now in check");
            }
            if (!checkopponent)
            {
                if (!b.HasAnyLegalMoves(oppcolour))
                {
                    MessageBox.Show("Stalemate jit");
                    isGameOver = true; //stops game as its stalemate
                }
            }
            // Toggle turn
            currentTurn = (currentTurn == PieceColour.white) ? PieceColour.black : PieceColour.white; //if current turn is white, switch to black, else switch to white
            return true;
        }

        public Piece GetPieceFromBoard(int x, int y) 
        {
            return b.GetPiece(x, y);
        }
        public Piece[,] GetBoard() { return b.GetGrid(); } 
        public bool[,]? LegalSquares(int x, int y) 
        {

            return b.GetLegalMoves(x, y);
        }
        public void PlayMove(int startX, int startY, int endX, int endY) 
        {
            boardStack.Push(b.Clone()); // Save current board state before move
            b.MovePiece(startX, startY, endX, endY);

        }
        public void PromotePawn(Piece promotion)
        {
            if (!awaitingPromotion)
            {
                return; // No promotion pending
            }
            b.PawnPromotion(promoX, promoY, promotion);
            awaitingPromotion = false;
        }
        
        public void UndoLastMove() //classdiagram
        {
            // Guard against empty stack and null
            if (boardStack == null || boardStack.Count == 0)
            {
                return;
            }
            b = boardStack.Pop();
            
            currentTurn = (currentTurn == PieceColour.white) ? PieceColour.black : PieceColour.white; // Switch back turn
        }
    }
             
}

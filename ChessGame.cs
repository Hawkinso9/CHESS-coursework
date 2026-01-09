using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHESS_coursework
{
    class ChessGame
    {
        private Board b;
        public ChessGame()
        {
            b = new Board();
            
        }
        public Piece GetPieceFromBoard(int x, int y)
        {
            return b.GetPiece(x,y);
        }
        public Piece[,] GetBoard() { return b.GetGrid(); }
        public bool[,]? SelectedSquares(int x, int y)
        {
            return b.SelectedSquare( x,y);
        }
        public void PlayMove(int startX, int startY, int endX, int endY)
        {
            b.MovePiece(startX, startY, endX, endY);
        }
        /* public void CreateChess()
         {
             b.CreateBoard();
         }*/
    }
}

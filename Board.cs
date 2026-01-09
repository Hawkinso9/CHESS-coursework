using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CHESS_coursework
{
    class Board
    {
        private Piece[,] grid;
        int win = 0;
        private int? selectedX = null;
        private int? selectedY = null;
        public Board() //sets up the board with pieces in their starting positions x is the column y is the row
        {
            grid = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    grid[i, j] = new Empty();
                }
            }
            grid[0, 0] = new Rook(PieceColor.black);
            grid[0, 1] = new Knight(PieceColor.black);
            grid[0, 2] = new Bishop(PieceColor.black);
            grid[0, 3] = new Queen(PieceColor.black);
            grid[0, 4] = new King(PieceColor.black);
            grid[0, 5] = new Bishop(PieceColor.black);
            grid[0, 6] = new Knight(PieceColor.black);
            grid[0, 7] = new Rook(PieceColor.black);
            grid[7, 0] = new Rook(PieceColor.white);
            grid[7, 1] = new Knight(PieceColor.white);
            grid[7, 2] = new Bishop(PieceColor.white);
            grid[7, 3] = new Queen(PieceColor.white);
            grid[7, 4] = new King(PieceColor.white);
            grid[7, 5] = new Bishop(PieceColor.white);
            grid[7, 6] = new Knight(PieceColor.white);
            grid[7, 7] = new Rook(PieceColor.white);
            for (int i = 0; i < 8; i++)
            {
                grid[1, i] = new Pawn(PieceColor.black);
            }
            for (int i = 0; i < 8; i++)
            {
                grid[6, i] = new Pawn(PieceColor.white);
            }
        }
        public Piece? GetPiece(int x, int y)
        {
            return grid[x, y];
        }
        public bool[,]? SelectedSquare(int x, int y)
        {
            Piece temp = grid[x, y];
            
            
            bool[,] possibleMoves = temp.PossibleMoves(x, y, temp, grid);


            if (selectedX == null || selectedY == null)
            {
                selectedX = x;
                selectedY = y;
                bool[,] moves = temp.PossibleMoves(x, y, temp, grid);
                return moves;

            }
            else
            {
                // Second selection: attempt move
                Piece selectedPiece = grid[(int)selectedX, (int)selectedY]; //gets type from grid has to be cast to int as selectedX and selectedY are nullable
                possibleMoves = selectedPiece.PossibleMoves((int)selectedX, (int)selectedY, selectedPiece, grid); //gets possible moves for the selected piece
                if (possibleMoves[x, y] == true)
                {
                    MovePiece((int)selectedX, (int)selectedY, x, y);
                }
                // Reset selection for next turn
                selectedX = null;
                selectedY = null;

                return null;
            }
            
            // check whether its the first or second selection
            //if its the second selection then call move piece
            //ifs its first selection then highlight possible moves by calling get moves from piece
            //and then having a bool 2d array to track possible moves and returning that to the gui to highlight squares
        }

            
        
        public void MovePiece(int startX, int startY, int endX, int endY) //unfinished
        {
            Piece tempold = grid[startX, startY];
            Piece tempnew = grid[endX, endY];
            if (Convert.ToString(tempold.Color) != "None")
            {
                if (tempold.Color != tempnew.Color)
                {
                    grid[endX, endY] = new Empty();

                    grid[endX, endY] = tempold;
                    grid[startX, startY] = new Empty();
                }
                else
                {
                    MessageBox.Show("the pieces are the same colour");
                }
            }
        }
        public PieceColor GetPieceColour(int x, int y)
        {
            Piece temp = grid[x, y];
            return temp.Color; 
                
        }
        public Piece[,] UpdateGui(int x, int y, Piece var)
        {

            grid[(int)x, (int)y] = var;
            return grid;
        }

        public Piece[,] GetGrid() { return grid; }
        

        public int getWin() { return win; }
    }
}
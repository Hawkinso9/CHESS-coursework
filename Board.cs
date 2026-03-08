using System.Diagnostics.Eventing.Reader;
using System.Windows;

namespace CHESS_coursework
{
    class Board
    {
        private Piece[,] grid;
        
        private int? selectedX = null;
        private int? selectedY = null;

        bool pieceSelected = false;
        public Board(Board b) //deep copy constructor
        {
            this.grid = new Piece[8, 8]; 
            for (int i = 0; i < 8; i++)
            {
                for (int j=0; j < 8; j++)
                {
                    
                    Piece src = b.GetPiece(i, j) ?? new Empty();
                    switch (src)
                    {
                        case Rook r:
                            grid[i, j] = new Rook(r);
                            break;
                        case Knight n:
                            grid[i, j] = new Knight(n);
                            break;
                        case Bishop bs:
                            grid[i, j] = new Bishop(bs);
                            break;
                        case Queen q:
                            grid[i, j] = new Queen(q);
                            break;
                        case King k:
                            grid[i, j] = new King(k);
                            break;
                        case Pawn p:
                            grid[i, j] = new Pawn(p);
                            break;
                        case Empty e:
                            grid[i, j] = new Empty(e);
                            break;
                        default:
                            grid[i, j] = new Empty();
                            break;
                    }

                }
            }
            
            this.selectedX = b.selectedX; //selectedX and selectedY are nullable ints, so they can be directly copied without worrying about deep copying
            this.selectedY = b.selectedY;
            this.pieceSelected = b.pieceSelected; //pieceSelected is a bool, so it can also be directly copied
        }
        public Board Clone() => new Board(this);
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
            grid[0, 0] = new Rook(PieceColour.black);
            grid[0, 1] = new Knight(PieceColour.black);
            grid[0, 2] = new Bishop(PieceColour.black);
            grid[0, 3] = new Queen(PieceColour.black);
            grid[0, 4] = new King(PieceColour.black);
            grid[0, 5] = new Bishop(PieceColour.black);
            grid[0, 6] = new Knight(PieceColour.black);
            grid[0, 7] = new Rook(PieceColour.black);
            grid[7, 0] = new Rook(PieceColour.white);
            grid[7, 1] = new Knight(PieceColour.white);
            grid[7, 2] = new Bishop(PieceColour.white);
            grid[7, 3] = new Queen(PieceColour.white);
            grid[7, 4] = new King(PieceColour.white);
            grid[7, 5] = new Bishop(PieceColour.white);
            grid[7, 6] = new Knight(PieceColour.white);
            grid[7, 7] = new Rook(PieceColour.white);
            for (int i = 0; i < 8; i++)
            {
                grid[1, i] = new Pawn(PieceColour.black);
            }
            for (int i = 0; i < 8; i++)
            {
                grid[6, i] = new Pawn(PieceColour.white);
            }
        }
        public Piece GetPiece(int x, int y)
        {
            return grid[x, y];
        }
        public bool HasAnyLegalMoves(PieceColour colour)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++) 
                {
                    if (grid[x, y].Colour == colour) //loops through the board to find pieces of the specified colour
                    {
                        bool[,]? legalMoves = GetLegalMoves(x, y);//gets the legal moves for that piece

                        if (legalMoves != null) // if the piece has legal moves
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if (legalMoves[i, j])
                                    {
                                        return true; // Found at least one legal move
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false; // No legal moves found
        }
        public void PawnPromotion(int x, int y, Piece Type)
        {
            PieceColour colour = grid[x, y].Colour;
            if (colour == PieceColour.white)
            {
                if (Type is Queen)
                {
                    grid[x, y] = new Queen(PieceColour.white);
                }
                else if (Type is Rook)
                {
                    grid[x, y] = new Rook(PieceColour.white);
                }
                else if (Type is Bishop)
                {
                    grid[x, y] = new Bishop(PieceColour.white);
                }
                else if (Type is Knight)
                {
                    grid[x, y] = new Knight(PieceColour.white);
                }
            }
            else if (colour == PieceColour.black)
            {
                if (Type is Queen)
                {
                    grid[x, y] = new Queen(PieceColour.black);
                }
                else if (Type is Rook)
                {
                    grid[x, y] = new Rook(PieceColour.black);
                }
                else if (Type is Bishop)
                {
                    grid[x, y] = new Bishop(PieceColour.black);
                }
                else if (Type is Knight)
                {
                    grid[x, y] = new Knight(PieceColour.black);
                }
            }
        }
        public bool[,]? GetLegalMoves(int x, int y)
        {
            PieceColour piececolour = GetPieceColour(x, y);
            PieceColour enemyKing = GetEnemyPieceColour(x,y); //gets the colour of the grid then returns the opposite colour 
            bool[,]? pseudomoves = SelectedSquare(x,y); //gets the possible moves for the piece without considering checks          
            if (pseudomoves == null)
            {
                return null;
            }
            for (int i = 0;i < 8; i++)
            {
                for(int j = 0;j < 8; j++)
                {
                    if (pseudomoves[i, j])
                    {
                        // simulate the move on a clone
                        Board clone = this.Clone();
                        clone.MovePiece(x, y, i, j);
                        if (clone.IsInCheck(piececolour)) // checks if the move would put the player's own king in check
                        {
                            pseudomoves[i, j] = false;
                        }
                    }
                }
            }
            return pseudomoves;
        }
        public bool[,]? SelectedSquare(int x, int y) 
        {
           
            Piece temp = grid[x, y];

            if (temp is Empty || temp.Colour == PieceColour.none)
            {
                return null;
            }

            return temp.PossibleMoves(x, y, temp, grid);
        }

        public void MovePiece(int startX, int startY, int endX, int endY)  
        {
            Piece tempold = grid[startX, startY];
            Piece tempnew = grid[endX, endY];
            if (Convert.ToString(tempold.Colour) != "None") // might need to change all the some.colour to the board function GetPieceColour
            {
                if (tempold.Colour != tempnew.Colour)
                {
                    grid[endX, endY] = new Empty();

                    grid[endX, endY] = tempold;
                    grid[startX, startY] = new Empty();
                }
                else // if the piece is trying to move onto a square occupied by a piece of the same colour do nothing (invalid move)
                {

                }
                selectedX = null;
                selectedY = null;
                pieceSelected = false;
            }
        }
        public bool ApplyMove(int startX, int startY, int endX, int endY) //basically a extra layer over movepiece which saves the move
        {
            int j = (endY - startY);
            int rookStartY;
            int rookEndY;
            Piece movedPiece = grid[startX, startY];
            Piece capturedPiece = grid[endX, endY];
            if (movedPiece is King && j == 2)
            {
                if (endY > startY) // if the king is moving to the right (kingside castling)
                {
                    rookStartY = 7;
                    rookEndY = startY + 1;
                }
                else // if the king is moving to the left (queenside castling)
                {
                    rookStartY = 0;
                    rookEndY = startY - 1;
                }
                Piece rook = grid[startX, rookStartY];
                MovePiece(startX, startY, endX, endY);
                MovePiece(startX, rookStartY, startX, rookEndY);
                rook.HasMoved = true;
                movedPiece.HasMoved = true;
                return true;
            }
            
            else
            {
                MovePiece(startX, startY, endX, endY);
                movedPiece.HasMoved = true; 
                return true;
                
            }
        }
        
        public bool IsInCheck( PieceColour colour)
        {
            //gets the king's position
            var (kingX, kingY) = FindKing(colour);

            // If king not found (for safety)
            if (kingX < 0 || kingY < 0) return false;

            //check every opponent piece to see if it can move to the king's square
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    var p = grid[x, y];
                    if (p.Colour == PieceColour.none) continue;
                    if (p.Colour == colour) continue; // only check opponent pieces

                    bool[,] moves = p.PossibleMoves(x, y, p, grid);
                    if (moves != null && kingX >= 0 && kingX < 8 && kingY >= 0 && kingY < 8 && moves[kingX, kingY]) 
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private (int x, int y) FindKing(PieceColour colour)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (grid[i, j] is King && grid[i, j].Colour == colour)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1); // King not found (should not happen in a valid game)
        }
        public bool IsPawnPromotion(int x, int y)
        {
            Piece temp = grid[x, y];
            if (temp is Pawn)
            {
                if (( x == 7 && temp.Colour == PieceColour.black ) || ( x == 0 && temp.Colour == PieceColour.white ))
                {
                    return true;

                }
            }
            return false;
        }
        public PieceColour GetPieceColour(int x, int y)
        {
            Piece temp = grid[x, y]; 
            return temp.Colour;

        }
        public PieceColour GetEnemyPieceColour(int x, int y)
        {
            Piece temp = grid[x, y];
            if (temp.Colour == PieceColour.white)
            {
                return PieceColour.black;
            }
            if (temp.Colour == PieceColour.black)
            {
                return PieceColour.white;
            }
            return PieceColour.none;

        }
        public Piece[,] UpdateGui(int x, int y, Piece var)
        {

            grid[(int)x, (int)y] = var;
            return grid;
        }

        public Piece[,] GetGrid() { return grid; } 



    }
}
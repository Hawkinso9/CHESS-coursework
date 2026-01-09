using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHESS_coursework //how to store moves, whether to store a white and black versions,
{
    public enum PieceColor
    {
        white,
        black,
        none
    }

    internal abstract class Piece
    {
        public PieceColor Color { get; set; }


        public bool[,] PossibleMoves(int x, int y, Piece Type, Piece?[,] Board) // x is column y is row
        {
            bool[,] PossibleMoves = new bool[8, 8];

            bool InBoard(int x, int y)
            {return x >= 0 && x < 8 && y >= 0 && y < 8;}

            int[,] KingMoves =
            {
                {x-1,y-1}, {x-1,y},{x-1,y+1},{x,y-1}, {x,y+1}, {x+1,y-1}, {x+1,y},{x+1,y+1}
            };
            int[,] KnightMoves =
            {
                {x-2,y-1}, {x-2,y+1}, {x-1,y-2}, {x-1,y+2}, {x+1,y-2}, {x+1,y+2}, {x+2,y-1}, {x+2,y+1}
            };
            void KingKnightChecker(int x, int y)
            {
                if (!InBoard(x, y)) { return; } //if outside bounds stops 
                Piece? target = Board[x,y];
                if (target == null || target.Color != Type.Color) //if the target square is empty or occupied by an opponents piece
                {
                    PossibleMoves[x, y] = true;
                }
            }
            

            if (Type is King) //checks what type of piece it is
            {
                for (int i = 0; i < 8; i++)
                {
                    KingKnightChecker(KingMoves[i, 0], KingMoves[i, 1]);
                }

            }
            if (Type is Knight) //checks what type of piece it is
            {
                for (int i = 0; i < 8; i++)
                {
                    KingKnightChecker(KnightMoves[i, 0], KnightMoves[i, 1]);
                }

            }
            if (Type is Bishop)
            {
                int[,] vectordirection = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } }; // diagonal directions

                for (int d = 0; d < 4; d++)
                {
                    int dirx = vectordirection[d, 0];
                    int diry = vectordirection[d, 1];
                    for (int step = 1; step <= 7; step++)
                    {
                        int xx = x + dirx * step; // Calculate new x position
                        int yy = y + diry * step;  // Calculate new y position

                        if (!InBoard(xx, yy)) { break; }

                        Piece? target = Board[xx, yy];
                        if (target == null)
                        {
                            PossibleMoves[xx, yy] = true; // empty square
                        }
                        else
                        {
                            if (target.Color != Type.Color)
                            {
                                PossibleMoves[xx, yy] = true; // capture enemy
                            }
                            break; // stop in either case (enemy or own piece)
                        }
                    }
                }

            }
            if (Type is Rook) //checks what type of piece it is
            {
                int[,] vectordirection = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // horizontal and vertical directions
                for (int d = 0; d < 4; d++)
                {
                    int dirx = vectordirection[d, 0];
                    int diry = vectordirection[d, 1];
                    for (int step = 1; step <= 7; step++)
                    {
                        int xx = x + dirx * step; // Calculate new x position
                        int yy = y + diry * step;  // Calculate new y position

                        if (!InBoard(xx, yy)) { break; }

                        Piece? target = Board[xx, yy];
                        if (target == null)
                        {
                            PossibleMoves[xx, yy] = true; // empty square
                        }
                        else
                        {
                            if (target.Color != Type.Color)
                            {
                                PossibleMoves[xx, yy] = true; // capture enemy
                            }
                            break; // stop in either case (enemy or own piece)
                        }
                    }
                }
            }
            if (Type is Queen) //checks what type of piece it is
            {
                int[,] vectordirection = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
                for (int d = 0; d < 8; d++)
                {
                    int dirx = vectordirection[d, 0];
                    int diry = vectordirection[d, 1];
                    for (int step = 1; step <= 7; step++)
                    {
                        int xx = x + dirx * step; // Calculate new x position
                        int yy = y + diry * step;  // Calculate new y position

                        if (!InBoard(xx, yy)) { break; }

                        Piece? target = Board[xx, yy];
                        if (target == null)
                        {
                            PossibleMoves[xx, yy] = true; // empty square
                        }
                        else
                        {
                            if (target.Color != Type.Color)
                            {
                                PossibleMoves[xx, yy] = true; // capture enemy
                            }
                            break; // stop in either case (enemy or own piece)

                        }
                        
                    }
                }

            }
            if (Type is Pawn pawn) //checks what type of piece it is check if its in a certain row and certain coloar and if true then allow 2 moves
            {
                if ((y == 1 && pawn.Color == PieceColor.white) || (y == 6 && pawn.Color == PieceColor.black))
                {
                    int direction = (pawn.Color == PieceColor.white) ? -1 : 1; //sets direction based on color
                    int[,] PawnMoves =
                        { { y, x + direction },{ y, x + 2 * direction }};
                    for (int i = 0; i != 2; i++) //loops through all possible pawn moves
                    {
                        int tempx = PawnMoves[i, 0];//gets x coordinate of possible move

                        int tempy = PawnMoves[i, 1];//gets y coordinate of possible move

                        if (tempy >= 0 && tempy < 8 && tempx >= 0 && tempx < 8 )//checks if the move is within bounds of the board
                        {

                            PossibleMoves[PawnMoves[i, 0], PawnMoves[i, 1]] = true;//if it is within bounds, marks it as a possible move


                        }
                    }
                }
                else
                {
                    int direction = (pawn.Color == PieceColor.white) ? 1 : -1;
                    int[,] PawnMoves = { { x - direction, y  } };
                    int tempx = PawnMoves[0, 0];//gets x coordinate of possible move
                    int tempy = PawnMoves[0, 1];//gets y coordinate of possible move
                    if (tempy >= 0 && tempy < 8 && tempx >= 0 && tempx < 8)//checks if the move is within bounds of the board
                    {
                        PossibleMoves[PawnMoves[0, 0], PawnMoves[0, 1]] = true;//if it is within bounds, marks it as a possible move
                    }

                }

            }
            return PossibleMoves;
        }
    }
        class Empty : Piece
        {
            public Empty()
            {
                Color = PieceColor.none;
            }
            int[,] giveMoves = new int[8, 8];

        }
        class Pawn : Piece
        {

            public Pawn(PieceColor color)
            {
                Color = color;
            }


        }
        class Knight : Piece
        {
            public Knight(PieceColor color)
            {
                Color = color;
            }


        }
        class Bishop : Piece
        {
            public Bishop(PieceColor color)
            {
                Color = color;
            }
        }
        class Rook : Piece
        {
            public Rook(PieceColor color)
            {
                Color = color;
            }
        }
        class King : Piece
        {

            public King(PieceColor color)
            {
                Color = color;
            }

        }
        class Queen : Piece
        {

            public Queen(PieceColor color)
            {
                Color = color;
            }
        }

    
}


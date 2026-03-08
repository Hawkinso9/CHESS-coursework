using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Text;
using System.Windows.Controls;

namespace CHESS_coursework //how to store moves, whether to store a white and black versions,
{
    public enum PieceColour
    {
        white,
        black,
        none
    }

    internal abstract class Piece
    {

        public PieceColour Colour { get; set; }
        public bool HasMoved { get; set; } = false; //used for king and rook castling pawn not needed as logic is done

        public bool[,] PossibleMoves(int x, int y, Piece piece, Piece?[,] Board) // x is column y is row
        {
            bool[,] PossibleMoves = new bool[8, 8];

            bool InBoard(int x, int y)
            {return x >= 0 && x < 8 && y >= 0 && y < 8;}
            bool IsEmpty(Piece? test)
            {
                return test.Colour == PieceColour.none;
            }

            bool IsEnemy(Piece? test,Piece current)
            {
                return test.Colour != PieceColour.none && test.Colour != current.Colour;
            }

            
            // King moves
            if (piece is King)
            {
                int[,] KingMoves ={{x-1,y-1}, {x-1,y}, {x-1,y+1}, {x,y-1}, {x,y+1}, {x+1,y-1}, {x+1,y}, {x+1,y+1}};

                for (int i = 0; i < 8; i++)
                {
                    int tempx = KingMoves[i, 0];
                    int tempy = KingMoves[i, 1];
                    if (!InBoard(tempx, tempy)) continue;
                    Piece? target = Board[tempx, tempy];
                    if (IsEmpty(target) || IsEnemy(target, piece)) //checkes if the target square is empty or occupied by an enemy piece
                    {
                        PossibleMoves[tempx, tempy] = true;
                    }
                }
                if (!piece.HasMoved) //castling logic only applies if the king has not moved
                {
                        // Check for king-side castling
                        if (Board[x, 7] is Rook rookH && !rookH.HasMoved && IsEmpty(Board[x, 5]) && IsEmpty(Board[x, 6]))
                        {
                            PossibleMoves[x, 6] = true; // Mark the king-side castling square as a possible move
                        }
                        // Check for queen-side castling
                        if (Board[x, 0] is Rook rookA && !rookA.HasMoved && IsEmpty(Board[x, 1]) && IsEmpty(Board[x, 2]) && IsEmpty(Board[x, 3]))
                        {
                            PossibleMoves[x, 2] = true; // Mark the queen-side castling square as a possible move
                    }
                }
                
            }

            // Knight moves
            if (piece is Knight)
            {
                int[,] KnightMoves ={{x-2,y-1}, {x-2,y+1}, {x-1,y-2}, {x-1,y+2}, {x+1,y-2}, {x+1,y+2}, {x+2,y-1}, {x+2,y+1}};

                for (int i = 0; i < 8; i++)
                {
                    int cx = KnightMoves[i, 0];
                    int cy = KnightMoves[i, 1];
                    if (!InBoard(cx, cy)) continue;
                    Piece? target = Board[cx, cy];
                    if (IsEmpty(target) || IsEnemy(target, piece))
                    {
                        PossibleMoves[cx, cy] = true;
                    }
                }
            }

            
            void Slide(int[,] directions) //centralized sliding function for bishop rook and queen
            {
                int directionCount = directions.GetLength(0);//gets number of directions cant be .Length as its 2d array

                for (int d = 0; d < directionCount; d++)
                {
                    int dirx = directions[d, 0];
                    int diry = directions[d, 1];
                    for (int step = 1; step < 8; step++)
                    {
                        int xx = x + dirx * step;
                        int yy = y + diry * step;
                        if (!InBoard(xx, yy)) break;
                        Piece? target = Board[xx, yy];
                        if (IsEmpty(target))
                        {
                            PossibleMoves[xx, yy] = true;
                            continue; // keep scanning along this direction
                        }
                        if (IsEnemy(target, piece))
                        {
                            PossibleMoves[xx, yy] = true; // can capture enemy
                            break; // stop after capture
                        }
                        // own piece blocks movement
                        break;
                    }
                }
            }

            if (piece is Bishop bishop)
            {
                int[,] vectordirection = bishop.GetDirections();
                Slide(vectordirection);
            }

            if (piece is Rook rook)
            {
                int[,] vectordirection = rook.GetDirections();
                Slide(vectordirection);
            }
            
            if (piece is Queen queen)
            {
                int[,] vectordirection = queen.GetDirections();
                Slide(vectordirection);
            }

            if (piece is Pawn pawn) //checks what type of piece it is
            {
                int attackdirection = (pawn.Colour == PieceColour.white) ? -1 : 1; //sets direction based on color
                int[,] pawnattack = { { x + attackdirection, y + attackdirection }, { x + attackdirection, y - attackdirection } }; // this is for attacking moves
                for (int i = 0; i != 2; i++) //loops through all possible pawn attack moves
                {
                    int tempx = pawnattack[i, 0];//gets x coordinate of possible move
                    int tempy = pawnattack[i, 1];//gets y coordinate of possible move
                    if (tempy >= 0 && tempy < 8 && tempx >= 0 && tempx < 8 )//checks if the move is within bounds of the board
                    {
                        Piece? target = Board[tempx, tempy];
                        if (!IsEmpty(target) && IsEnemy(pawn, target)) //if there is an enemy piece in the attack square
                        {
                            PossibleMoves[pawnattack[i, 0], pawnattack[i, 1]] = true;//marks it as a possible move
                        }
                    }
                }
                if ((x == 6 && pawn.Colour == PieceColour.white) || (x == 1 && pawn.Colour == PieceColour.black))//checks what type of piece it is check if its in a certain row and certain coloir and if true then allow 2 moves
                {
                    int direction = (pawn.Colour == PieceColour.white) ? -1 : 1; //sets direction based on color
                    int[,] PawnMoves ={ { x + direction, y },{ x + 2 * direction, y },{ x + direction, y + direction }, { x + direction, y - direction } };

                    for (int i = 0; i != 2; i++) //loops through all possible pawn moves
                    {

                        int tempx = PawnMoves[i, 0];//gets x coordinate of possible move
                        int tempy = PawnMoves[i, 1];//gets y coordinate of possible move

                        if (tempy >= 0 && tempy < 8 && tempx >= 0 && tempx < 8 )//checks if the move is within bounds of the board
                        {
                            Piece? target = Board[tempx, tempy];
                            if (IsEmpty(target))
                            {
                                PossibleMoves[PawnMoves[i, 0], PawnMoves[i, 1]] = true;//if it is within bounds, marks it as a possible move
                            }
                            else
                            {
                                break;//if the first move is blocked, the second move is also blocked
                            }
                        }
                    }
                 }
                else
                {
                    int direction = (pawn.Colour == PieceColour.white) ? 1 : -1;
                    int[,] PawnMoves = { { x - direction, y  } };
                    int tempx = PawnMoves[0, 0];//gets x coordinate of possible move
                    int tempy = PawnMoves[0, 1];//gets y coordinate of possible move
                    if (tempy >= 0 && tempy < 8 && tempx >= 0 && tempx < 8)//checks if the move is within bounds of the board
                    {
                        
                        Piece? target = Board[tempx, tempy];
                        if (IsEmpty(target))
                        {
                            PossibleMoves[PawnMoves[0, 0], PawnMoves[0, 1]] = true;//if it is within bounds, marks it as a possible move
                        }
                        else
                        {
                           
                        }
                    }
                }

            }
            return PossibleMoves;
        }
    }
        class Empty : Piece
        {
            public Empty(Empty r)  //copy constructor used in cloning the board
            {
                Colour = r.Colour;
            HasMoved = r.HasMoved;
        }
        public Empty()
            {
                Colour = PieceColour.none;
        }
            int[,] giveMoves = new int[8, 8];

        }
        class Pawn : Piece
        {
            public Pawn(Pawn r)  //copy constructor for pawn used in cloning the board
            {
                Colour = r.Colour;
            HasMoved = r.HasMoved;
        }
        public Pawn(PieceColour color)
            {
                Colour = color;
            }


        }
        class Knight : Piece
        {
            public Knight(Knight r)  //copy constructor for knight used in cloning the board
            {
                Colour = r.Colour;
            HasMoved = r.HasMoved;
        }
        public Knight(PieceColour color)
            {
                Colour = color;
            }


        }
        class Bishop : Piece
        {
            public Bishop(Bishop r)  //copy constructor for bishop used in cloning the board
            {
                Colour = r.Colour;
            HasMoved = r.HasMoved;
        }   
        public Bishop(PieceColour color)
            {
                Colour = color;
            }
            private int[,] vectordirection = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
            public int[,] GetDirections()
            {
                return vectordirection;
            }   
        }
        class Rook : Piece
        {
             public Rook(Rook r)  //copy constructor for rook used in cloning the board
        { 
                    Colour = r.Colour;
                     HasMoved = r.HasMoved;
        }
        public Rook(PieceColour color)
            {
                Colour = color;
            }
        int[,] vectordirection = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
        public int[,] GetDirections()
        {
                
            return vectordirection;
        }
    }
        class King : Piece
        {
        public King(King r)  //copy constructor for king used in cloning the board
        {
            Colour = r.Colour;
            HasMoved = r.HasMoved;
        }
        public King(PieceColour color)
            {
                Colour = color;
            }
            
    }
        class Queen : Piece
        {
            public Queen(Queen r)  //copy constructor for queen used in cloning the board
            {
                Colour = r.Colour;
            HasMoved = r.HasMoved;
        }
        public Queen(PieceColour color)
            {
                Colour = color;
            }
        int[,] vectordirection = { { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 }, { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
        public int[,] GetDirections()
        {
            return vectordirection;
        }
    }

    
}


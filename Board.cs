using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHESS_coursework
{
    class Board
    {
        private Piece?[,] grid = new Piece?[8, 8];
        int win = 0;
        public Board()
        {
            
        }

        public void checkwin() 
        {
            win = 2;
        }

        public int getWin() { return win; }
    }
}

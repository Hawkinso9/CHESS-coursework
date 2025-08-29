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

        public Board getBoard() { return b; }

        public void doSomething()
        {
            b.checkwin();
        }
    }
}

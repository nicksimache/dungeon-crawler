using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Board
{
    internal class Board
    {
        public Room[,] board { get; private set; }

        public Board()
        {
            board = new Room[25, 25];
            for(int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(0); j++)
                {
                    board[i,j] = new Room();
                }
            }
            generateBoard();
            
        }

        public Board(int X, int Y)
        {
            board = new Room[X, Y];
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++)
                {
                    board[i, j] = new Room();
                }
            }
            generateBoard();
        }

        public void generateBoard()
        {
            makeRoom(board.GetLength(0)/2, board.GetLength(0) / 2, 1);
        }

        public void makeRoom(int X, int Y, int difficulty)
        {
            Random random = new Random();

            board[X, Y].difficulty = difficulty;

            if (random.Next(0, 10) > 0.8 * board[X, Y].difficulty && X > 0 && board[X - 1, Y].difficulty == 0)
            {
                makeRoom(X - 1, Y, difficulty + 1);
            }

            if (random.Next(0, 10) > 0.8 * board[X, Y].difficulty && X < board.GetLength(0) - 1 && board[X + 1, Y].difficulty == 0)
            {
                makeRoom(X + 1, Y, difficulty + 1);
            }

            if (random.Next(0, 10) > 0.8 * board[X, Y].difficulty && Y > 0 && board[X, Y - 1].difficulty == 0)
            {
                makeRoom(X, Y - 1, difficulty + 1);
            }

            if (random.Next(0, 10) > 0.8 * board[X, Y].difficulty && Y < board.GetLength(1) - 1 && board[X, Y + 1].difficulty == 0)
            {
                makeRoom(X, Y + 1, difficulty + 1);
            }
        }


        public override string ToString()
        {
            string output = "";

            for(int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(0); j++)
                {
                    //output += board[i, j].difficulty + "    ";
                    if(i==12 && j==12)
                    {
                        output += "S ";
                    }
                    else if (board[i,j].difficulty != 0)
                    {
                        output += "R ";
                    }
                    else
                    {
                        output += "_ ";
                    }
                }

                output += "\n";
            }

            return output;
        }
    }
}

using GameAI.GamePlaying.Core;
using System;
using System.Runtime.Remoting.Messaging;

namespace GameAI.GamePlaying
{
    public class StudentAI : Behavior
    {
        public StudentAI() { }

        public ComputerMove Run(int color, Board board, int lookAheadDepth)
        {
            int I = -1, J = -1;
            int maxEval = int.MinValue;

            for (int i = 0; i < Board.Height; i++) 
                for (int j = 0; j < Board.Width; j++)
                {
                    if(!board.IsValidMove(color, i, j)) continue;

                    Board futureBoard = new Board(board);
                    futureBoard.MakeMove(color, i, j);
                    int predEval = MiniMax(futureBoard.HasAnyValidMove(color * -1) ? color * -1 : color, futureBoard, lookAheadDepth);

                    if(predEval * color > maxEval)
                    {
                        maxEval = predEval * color;
                        I = i;
                        J = j;
                    }

                }

            return new ComputerMove(I, J);
        }

        private int MiniMax(int color, Board board, int lookAheadDepth)
        {
            if(lookAheadDepth == 0 || board.IsTerminalState()) return Eval(board);
            int maxEval = int.MinValue;

            for (int i = 0; i < Board.Height; i++)
                for (int j = 0; j < Board.Width; j++)
                {
                    if (!board.IsValidMove(color, i, j)) continue;

                    Board futureBoard = new Board(board);
                    futureBoard.MakeMove(color, i, j);
                    int predEval = MiniMax(futureBoard.HasAnyValidMove(color * -1) ? color * -1 : color, futureBoard, lookAheadDepth - 1);

                    if (predEval * color > maxEval)
                        maxEval = predEval * color;
                }

            return maxEval * color;
        }

        // Calculates the Eval Score of a given board 
        private int Eval(Board board) {
            int ret = 0;
            for (int i = 0; i < Board.Height; i++) for (int j = 0; j < Board.Width; j++) ret += board.GetTile(i, j) * (i == 0 || j == 0 || i == Board.Height - 1 || j == Board.Width - 1 ? 10 : 1) * ((i == 0 && (j == 0 || j == Board.Width - 1)) || (i == Board.Height - 1 && (j == 0 || j == Board.Width - 1)) ? 10 : 1);
            return ret + (board.IsTerminalState() && board.Score != 0 ? (board.Score > 0 ? 1 : -1) * 10000 : 0);
        }
    }
}

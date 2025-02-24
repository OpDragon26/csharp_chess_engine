using System.Collections;
using Board;

namespace Node
{
    public class Node
    {
        public Board.Board board = new Board.Board();
        public Node[] ChildNodes = new Node[] {};
        public Node(Board.Board newBoard)
        {
            board = newBoard;
        }

        public Move.Move[] GenerateChildren()
        {
            ArrayList NodeList = new ArrayList();

            if (this.board.Status() == Outcome.Ongoing)
            {
                Move.Move[] MoveList = MoveFinder.Search(board, board.Side);

                for (int i = 0; i < MoveList.Length; i++)
                {
                    Board.Board MoveBoard = board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false);

                    NodeList.Add(new Node(MoveBoard));
                }
                return MoveList;
            }

            this.ChildNodes = (Node[])NodeList.ToArray(typeof(Node));

            return new Move.Move[0];
        }

        public void Branch(int Depth)
        {
            this.GenerateChildren();

            if (Depth > 0)
            {
                if (this.ChildNodes.Length > 0)
                {
                    for (int i = 0; i < this.ChildNodes.Length; i++)
                    {
                        ChildNodes[i].Branch(Depth - 1);
                    }
                }
            }
        }

        public int GetEval()
        {
            // If either side won or the game is a draw, return the respective values
            if (this.board.Status() == Outcome.Draw)
            {
                return 0;
            }
            if (this.board.Status() == Outcome.White)
            {
                return 10000;
            }
            if (this.board.Status() == Outcome.Black)
            {
                return -10000;
            }

            // If there are more child nodes, return the lowest or highest depending on whose move it is
            // If it's white's move, return the highest eval, because white would make the best move for them
            // Black's advantage is represented with a negative value, so for black, the lowest value is the best
            if (ChildNodes.Length != 0)
            {
                int[] ChildEvalList = new int[ChildNodes.Length];

                for (int i = 0; i < this.ChildNodes.Length; i++)
                {
                    ChildEvalList[i] = ChildNodes[i].GetEval();
                }
                if (this.board.Side)
                {
                    return ChildEvalList.Max();
                }
                return ChildEvalList.Min();
            }

            // If there are 0 child nodes, then calculate the eval of the board
            return this.Evaluate();
        }

        int Evaluate()
        {
            int Eval = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Eval += this.board.board[i,j].Value;
                }
            }

            return Eval;
        }
    }
}
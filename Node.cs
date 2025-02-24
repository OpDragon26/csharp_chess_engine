using System.Collections;
using Board;

namespace Node
{
    public class Node
    {
        public Board.Board board = new Board.Board();
        public Node[] ChildNodes = new Node[] {};
        public int Eval;

        public Node(Board.Board newBoard)
        {
            board = newBoard;
        }

        public void GenerateChildren()
        {
            ArrayList NodeList = new ArrayList();

            Move.Move[] MoveList = MoveFinder.Search(board, board.Side);

            for (int i = 0; i < MoveList.Length; i++)
            {
                Board.Board MoveBoard = board.DeepCopy();
                MoveBoard.MakeMove(MoveList[i], false);

                NodeList.Add(new Node(MoveBoard));
            }

            this.ChildNodes = (Node[])NodeList.ToArray(typeof(Node));
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

        public void Evaluate()
        {
            this.Eval = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    this.Eval += this.board.board[i,j].Value;
                }
            }
        }
    }    
}
using System.Collections;
using Board;

namespace Node
{
    public class Node
    {
        public Board.Board board = new Board.Board();
        public List<Node> ChildNodes = new List<Node>();
        public Node(Board.Board newBoard)
        {
            board = newBoard;
        }

        public List<Move.Move> GenerateChildNodes()
        {
            List<Node> NodeList = new List<Node>();

            if (this.board.Status() == Outcome.Ongoing)
            {
                List<Move.Move> MoveList = MoveFinder.Search(board, board.Side);

                for (int i = 0; i < MoveList.Count; i++)
                {
                    Board.Board MoveBoard = board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false);

                    NodeList.Add(new Node(MoveBoard));
                }
                this.ChildNodes = NodeList;

                return MoveList;
            }
            this.ChildNodes = new List<Node>(); 

            return new List<Move.Move>();
        }

        public void SearchBranches(int Depth, bool NewNodes)
        {
            if (NewNodes)
            {
                this.GenerateChildNodes();
            }

            if (Depth > 0)
            {
                if (this.ChildNodes.Count> 0)
                {
                    for (int i = 0; i < this.ChildNodes.Count; i++)
                    {
                        ChildNodes[i].SearchBranches(Depth - 1, true);
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
            if (ChildNodes.Count != 0)
            {
                int[] ChildEvalList = new int[ChildNodes.Count];
                for (int i = 0; i < this.ChildNodes.Count; i++)
                {
                    ChildEvalList[i] = ChildNodes[i].GetEval();
                }
                if (this.board.Side)
                {
                    return ChildEvalList.Min();
                }
                return ChildEvalList.Max();
            }

            // If there are 0 child nodes, then calculate the eval of the board
            return this.Evaluate();
        }

        public Move.Move BestMove(int Depth, bool PrintEval)
        {
            List<Move.Move> Moves = GenerateChildNodes();

            this.SearchBranches(Depth, false);

            if (ChildNodes.Count != 0)
            {
                ArrayList EvalList = new ArrayList();

                for (int i = 0; i < this.ChildNodes.Count; i++)
                {
                    EvalList.Add(ChildNodes[i].GetEval());
                }

                int[] Evals = (int[])EvalList.ToArray(typeof(int));

                if (!this.board.Side)
                {
                    var (Max, MaxIndex) = Evals.Select((n, i) => (n, i)).Max();
                    if (PrintEval)
                    {
                        Console.WriteLine(Max);
                    }
                    return Moves[MaxIndex];
                }
                else
                {
                    var (Min, MinIndex) = Evals.Select((n, i) => (n, i)).Min();
                    if (PrintEval)
                    {
                        Console.WriteLine(Min);
                    }
                    return Moves[MinIndex];
                }

            }

            return new Move.Move();
        }

        int Evaluate()
        {
            int Eval = 0;

            for (int i = 0; i < board.PiecePositions[false].Count; i++)
            {
                (int, int) coords = ((int, int))board.PiecePositions[false][i];

                Eval += this.board.board[coords.Item2,coords.Item1].Value + Weights.Weights.PieceWeights[this.board.board[coords.Item2,coords.Item1]][coords.Item2,coords.Item1] * Weights.Weights.Multipliers[false];
            }
            for (int i = 0; i < board.PiecePositions[true].Count; i++)
            {
                (int, int) coords = ((int, int))board.PiecePositions[true][i];

                Eval += this.board.board[coords.Item2,coords.Item1].Value + Weights.Weights.PieceWeights[this.board.board[coords.Item2,coords.Item1]][coords.Item2,coords.Item1] * Weights.Weights.Multipliers[true];
            }

            return Eval;
        }
    }
}
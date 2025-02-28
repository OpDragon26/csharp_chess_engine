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

        public Move.Move BestMove(int Depth, bool PrintEval)
        {
            List<Move.Move> Moves = GenerateChildNodes();

            if (ChildNodes.Count != 0)
            {
                List<int> EvalList = new List<int>();

                for (int i = 0; i < this.ChildNodes.Count; i++)
                {
                    EvalList.Add(ChildNodes[i].Minimax(Depth, -1000000, 1000000));
                }

                if (!this.board.Side)
                {
                    var (Max, MaxIndex) = EvalList.Select((n, i) => (n, i)).Max();
                    if (PrintEval)
                    {
                        Console.WriteLine(Max);
                    }
                    return Moves[MaxIndex];
                }
                else
                {
                    var (Min, MinIndex) = EvalList.Select((n, i) => (n, i)).Min();
                    if (PrintEval)
                    {
                        Console.WriteLine(Min);
                    }
                    return Moves[MinIndex];
                }

            }

            return new Move.Move(new int[] {8,8}, new int[] {8,8}, Piece.Presets.Empty);
        }

        int StaticEvaluate()
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

        // Alpha-beta pruning (no workies :( )
        public int Minimax(int Depth, int alpha, int beta)
        {
            if (Depth == 0)
            return this.StaticEvaluate();

            Outcome Status = board.Status();
            if (Status == Outcome.White)
                return 1000000;
            if (Status == Outcome.Black)
                return -1000000;
            if (Status == Outcome.Draw)
                return 0;

            List<Move.Move> MoveList = MoveFinder.Search(board, board.Side);
            if (!board.Side)
            {
                int MaxEval = -1000000;

                for (int i = 0; i < MoveList.Count; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false);
                    Node Child = new Node(MoveBoard);

                    // Finding eval
                    int Eval = Child.Minimax(Depth - 1, alpha, beta);
                    MaxEval = Math.Max(MaxEval, Eval);
                    alpha = Math.Max(alpha, Eval);

                    if (beta <= alpha) break;
                }

                return MaxEval;
            }
            else
            {
                int MinEval = 1000000;

                for (int i = 0; i < MoveList.Count; i++)
                {
                    // Generate child node
                    Board.Board MoveBoard = this.board.DeepCopy();
                    MoveBoard.MakeMove(MoveList[i], false);
                    Node Child = new Node(MoveBoard);

                    // Finding eval
                    int Eval = Child.Minimax(Depth - 1, alpha, beta);
                    MinEval= Math.Min(MinEval, Eval);
                    beta = Math.Min(beta, Eval);

                    if (beta <= alpha) break;
                }

                return MinEval;
            }
        }
    }
}
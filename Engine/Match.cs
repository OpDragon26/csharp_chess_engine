using Board;
using static Piece.Presets;
using System.Collections.Generic;

namespace Match
{
    public class Match
    {
        public bool PlayerSide = false;
        public int Depth = 2; // 0 for pvp
        public Board.Board board = Presets.StartingBoard.DeepCopy();
        static Dictionary<Outcome, string> Outcomes = new Dictionary<Outcome, string>{
            {Outcome.Black, "Black won. Game over."},
            {Outcome.White, "White won. Game over."},
            {Outcome.Draw, "Game is a draw."}
        };
        bool Debug;
        bool Notate;

        public Match(bool side, int depth, bool debug, bool notateMoves)
        {
            PlayerSide = side;
            Depth = depth;
            Debug = debug;
            Notate = notateMoves;
        }

        public bool StatusTest()
        {
            return board.Status().Item1 == Outcome.Ongoing;
        }

        public bool MakeMove(Move.Move move)
        {
            return board.MakeMove(move, true);
        }

        public Move.Move MakeBotMove()
        {
            if (StatusTest())
            {
                Node.Node node = new Node.Node(this.board);
                Move.Move BotMove = node.BestMove(this.Depth);
                board.MakeMove(BotMove, false);
                return BotMove;
            }
            return new Move.Move(new int[] {8,8},new int[] {8,8}, Empty, 0);
        }
    }
}
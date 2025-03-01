using Board;
using static Piece.Presets;

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
            return board.Status() == Outcome.Ongoing;
        }

        public void MakeMove(string move)
        {
            if (StatusTest())
            {
                try{
                    Move.Move TryMove = Move.Move.FromString(move);

                    if (board.MakeMove(TryMove, true) && this.Depth != 0)
                    {
                        if (!Debug)
                        {
                            Console.Clear();
                        }
                        board.PrintBoard(PlayerSide);
                    }
                    else
                    {
                        Console.WriteLine("Illegal move");
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid notation");
                }
            }
        }

        public bool PlayMove(Move.Move move)
        {
            return board.MakeMove(move, true);
        }

        public Move.Move MakeBotMove()
        {
            if (StatusTest())
            {
                Node.Node node = new Node.Node(this.board);
                Move.Move BotMove = node.BestMove(this.Depth, false);
                board.MakeMove(BotMove, false);
                return BotMove;
            }
            return new Move.Move(new int[] {8,8},new int[] {8,8}, Empty);

        }

        public void Play()
        {
            Move.Move BotMove = new Move.Move(new int[] {8,8},new int[] {8,8}, Empty);

            while (StatusTest())
            {
                if (board.Side == PlayerSide || this.Depth == 0)
                {
                    if (!Debug)
                    {
                        Console.Clear();
                    }
                    board.PrintBoard(PlayerSide);
                    if (Notate)
                        Console.WriteLine(BotMove.Notate());
                    Console.WriteLine("Enter your move:");
                    string MoveString = Console.ReadLine() ?? "";
                    this.MakeMove(MoveString);
                }
                else
                {
                    BotMove = MakeBotMove();
                }
            }
            if (!Debug)
            {
                Console.Clear();
            }
            board.PrintBoard(PlayerSide);
            Console.WriteLine(Outcomes[this.board.Status()]);
        }
    }
}
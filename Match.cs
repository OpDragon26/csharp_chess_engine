using Board;

namespace Match
{
    public class Match
    {
        public bool PlayerSide = false;
        public int Depth = 2; // 0 for pvp
        public Board.Board board = Board.Presets.StartingBoard.DeepCopy();
        static Dictionary<Outcome, string> Outcomes = new Dictionary<Outcome, string>{
            {Outcome.Black, "Black won. Game over."},
            {Outcome.White, "White won. Game over."},
            {Outcome.Draw, "Game is a draw."}
        };

        public Match(bool side, int depth)
        {
            PlayerSide = side;
            Depth = depth;
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
                        Console.Clear();
                        board.PrintBoard(PlayerSide);
                    }
                    else
                    {
                        Console.WriteLine("Illegal move");
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid move");
                }
            }
        }

        public void MakeBotMove()
        {
            if (StatusTest())
            {
                Node.Node node = new Node.Node(this.board);
                Move.Move BotMove = node.BestMove(this.Depth, false);
                board.MakeMove(BotMove, false);
            }
        }

        public void Play()
        {
            while (StatusTest())
            {
                if (board.Side == PlayerSide || this.Depth == 0)
                {
                    Console.Clear();
                    board.PrintBoard(PlayerSide);
                    Console.WriteLine("Enter your move:");
                    string MoveString = Console.ReadLine() ?? "";
                    this.MakeMove(MoveString);
                }
                else
                {
                    MakeBotMove();
                }
            }
            Console.Clear();
            board.PrintBoard(PlayerSide);
            Console.WriteLine(Outcomes[this.board.Status()]);
        }
    }
}
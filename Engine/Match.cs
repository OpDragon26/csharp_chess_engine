using Board;
using static Piece.Presets;
using System.Collections.Generic;
using Piece;
using Presets = Board.Presets;

namespace Match
{
    public class Match
    {
        public bool PlayerSide = false;
        public int Depth = 2; // 0 for pvp
        public Board.Board board = Presets.StartingBoard.DeepCopy();

        static Dictionary<Outcome, string> Outcomes = new Dictionary<Outcome, string>
        {
            { Outcome.Black, "Black won. Game over." },
            { Outcome.White, "White won. Game over." },
            { Outcome.Draw, "Game is a draw." }
        };

        bool Debug;
        bool Notate;

        private static List<PieceType> AllWhitePieces = new List<PieceType>();
        private static List<PieceType> AllBlackPieces = new List<PieceType>();


        public Match(bool side, int depth, bool debug, bool notateMoves)
        {
            PlayerSide = side;
            Depth = depth;
            Debug = debug;
            Notate = notateMoves;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Presets.StartingBoard.board[i, j].Role != PieceType.Empty)
                    {
                        if (!Presets.StartingBoard.board[i, j].Color)
                            AllWhitePieces.Add(Presets.StartingBoard.board[i, j].Role);
                        else
                            AllBlackPieces.Add(Presets.StartingBoard.board[i, j].Role);
                    }
                }
            }
        }

        public bool StatusTest()
        {
            return board.Status().Item1 == Outcome.Ongoing;
        }

        public bool MakeMove(Move.Move move)
        {
            return board.MakeMove(move, true, false);
        }

        public Move.Move MakeBotMove()
        {
            if (StatusTest())
            {
                Node.Node node = new Node.Node(this.board);
                Move.Move BotMove = node.BestMove(this.Depth);
                board.MakeMove(BotMove, false, false);
                return BotMove;
            }

            return new Move.Move((8, 8), (8, 8), Empty, 0);
        }
        
        public (int, List<PieceType>, List<PieceType>) GetMaterialImbalance()
        {
            List<PieceType> WhitePieces = new List<PieceType>();
            List<PieceType> BlackPieces = new List<PieceType>();
            List<PieceType> RemainingWhitePieces = new List<PieceType>(AllWhitePieces);
            List<PieceType> RemainingBlackPieces = new List<PieceType>(AllBlackPieces);
            int WhiteMaterial = 0;
            int BlackMaterial = 0;
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (this.board.board[i, j].Role != PieceType.Empty)
                    {
                        if (!Presets.StartingBoard.board[i, j].Color)
                            WhitePieces.Add(Presets.StartingBoard.board[i, j].Role);
                        else
                            BlackPieces.Add(Presets.StartingBoard.board[i, j].Role);
                    }
                }
            }

            foreach (PieceType p in WhitePieces)
            {
                RemainingWhitePieces.Remove(p);
            }

            foreach (PieceType p in BlackPieces)
            {
                RemainingBlackPieces.Remove(p);
            }


            foreach (PieceType p in RemainingWhitePieces)
            {
                WhiteMaterial += Piece.Piece.Values[p];
            }

            foreach (PieceType p in RemainingBlackPieces)
            {
                BlackMaterial += Piece.Piece.Values[p];
            }
            
            return (WhiteMaterial - BlackMaterial, RemainingWhitePieces, RemainingBlackPieces);
        }
    }
}
using Board;
using static Piece.Presets;
using System.Collections.Generic;
using Piece;
using Presets = Board.Presets;
using UnityEngine;

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
        
        private Dictionary<bool, List<PieceType>> CapturedPieces = new Dictionary<bool, List<PieceType>>
        {
            { false, new List<PieceType>() },
            { true, new List<PieceType>() }
        };

        public Match(bool side, int depth, bool debug, bool notateMoves)
        {
            PlayerSide = side;
            Depth = depth;
            Debug = debug;
            Notate = notateMoves;
        }

        bool StatusTest()
        {
            return board.Status().Item1 == Outcome.Ongoing;
        }

        private void UpdateCapturedPieces(Move.Move move)
        {
            if (board.board[move.To.Item2, move.To.Item1].Role != PieceType.Empty)
            {
                if (CapturedPieces[!board.board[move.To.Item2, move.To.Item1].Color].Contains(board.board[move.To.Item2, move.To.Item1].Role))
                    CapturedPieces[!board.board[move.To.Item2, move.To.Item1].Color].Remove(board.board[move.To.Item2, move.To.Item1].Role);
                else
                    CapturedPieces[board.board[move.To.Item2,move.To.Item1].Color].Add(board.board[move.To.Item2,move.To.Item1].Role);
            }

            if (move.Promotion != Empty)
            {
                if (CapturedPieces[!board.board[move.To.Item2, move.To.Item1].Color].Contains(move.Promotion.Role))
                    CapturedPieces[!board.board[move.To.Item2, move.To.Item1].Color].Remove(move.Promotion.Role);
                else
                    CapturedPieces[board.board[move.To.Item2,move.To.Item1].Color].Add(move.Promotion.Role);
            }
        }

        public bool MakeMove(Move.Move move)
        {
            UpdateCapturedPieces(move);
            
            return board.MakeMove(move, true, false);
        }

        public Move.Move MakeBotMove()
        {
            if (StatusTest())
            {
                Node.Node node = new Node.Node(this.board);
                Move.Move BotMove = node.BestMove(this.Depth);
                
                UpdateCapturedPieces(BotMove);
                
                board.MakeMove(BotMove, false, false);
                return BotMove;
            }

            return new Move.Move((8, 8), (8, 8), Empty, 0);
        }
        
        public (int, List<PieceType>, List<PieceType>) GetMaterialImbalance()
        {
            int imbalance = 0;
            foreach (PieceType p in CapturedPieces[false])
            {
                imbalance += Piece.Piece.Values[p];
            }

            foreach (PieceType p in CapturedPieces[true])
            {
                imbalance -= Piece.Piece.Values[p];
            }
            
            return (imbalance / 100, CapturedPieces[false], CapturedPieces[true]);
        }
    }
}
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

        private void UpdateCapturedPieces(bool color, PieceType piece, Piece.Piece promotion, bool enPassant)
        {
            if (piece != PieceType.Empty)
            {
                if (CapturedPieces[!color].Contains(piece))
                    CapturedPieces[!color].Remove(piece);
                else
                    CapturedPieces[color].Add(piece);
            }
            else if (enPassant)
            {
                if (CapturedPieces[!color].Contains(PieceType.Pawn))
                    CapturedPieces[!color].Remove(PieceType.Pawn);
                else
                    CapturedPieces[color].Add(PieceType.Pawn);
            }

            if (promotion != Empty)
            {
                if (CapturedPieces[!promotion.Color].Contains(promotion.Role))
                    CapturedPieces[!promotion.Color].Remove(promotion.Role);
                else
                    CapturedPieces[promotion.Color].Add(promotion.Role);

                if (CapturedPieces[promotion.Color].Contains(PieceType.Pawn))
                    CapturedPieces[promotion.Color].Remove(PieceType.Pawn);
                else
                    CapturedPieces[!promotion.Color].Add(PieceType.Pawn);
            }
        }

        public bool MakeMove(Move.Move move)
        {
            bool MoveColor = board.board[move.To.Item2, move.To.Item1].Color;
            PieceType MovePiece = board.board[move.To.Item2, move.To.Item1].Role;
            
            if (board.MakeMove(move, true, false))
            {
                UpdateCapturedPieces(MoveColor, MovePiece, move.Promotion, move.EnPassant);
                return true;
            }
            return false;
        }

        public Move.Move MakeBotMove()
        {
            if (StatusTest())
            {
                Node.Node node = new Node.Node(this.board);
                Move.Move BotMove = node.BestMove(this.Depth);
                
                bool MoveColor = board.board[BotMove.To.Item2, BotMove.To.Item1].Color;
                PieceType MovePiece = board.board[BotMove.To.Item2, BotMove.To.Item1].Role;
                
                UpdateCapturedPieces(MoveColor, MovePiece, BotMove.Promotion, BotMove.EnPassant);
                
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
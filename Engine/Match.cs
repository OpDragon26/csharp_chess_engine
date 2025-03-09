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

        public bool StatusTest()
        {
            return board.Status().Item1 == Outcome.Ongoing;
        }

        public bool MakeMove(Move.Move move)
        {
            if (board.board[move.To.Item2, move.To.Item1].Role != PieceType.Empty)
            {
                CapturedPieces[board.board[move.To.Item2,move.To.Item1].Color].Add(board.board[move.To.Item2,move.To.Item1].Role);
                UnityEngine.Debug.Log(board.board[move.To.Item2,move.To.Item1].Role);
            }
            
            return board.MakeMove(move, true, false);
        }

        public Move.Move MakeBotMove()
        {
            if (StatusTest())
            {
                Node.Node node = new Node.Node(this.board);
                Move.Move BotMove = node.BestMove(this.Depth);
                
                if (board.board[BotMove.To.Item2, BotMove.To.Item1].Role != PieceType.Empty)
                {
                    if (CapturedPieces[!board.board[BotMove.To.Item2, BotMove.To.Item1].Color].Contains(board.board[BotMove.To.Item2, BotMove.To.Item1].Role))
                        CapturedPieces[!board.board[BotMove.To.Item2, BotMove.To.Item1].Color].Remove(board.board[BotMove.To.Item2, BotMove.To.Item1].Role);
                    else
                        CapturedPieces[board.board[BotMove.To.Item2,BotMove.To.Item1].Color].Add(board.board[BotMove.To.Item2,BotMove.To.Item1].Role);
                }
                
                board.MakeMove(BotMove, false, false);
                return BotMove;
            }

            return new Move.Move((8, 8), (8, 8), Empty, 0);
        }
        
        public (int, List<PieceType>, List<PieceType>) GetMaterialImbalance()
        {
            // remove duplicate pieces
            /*
            for (int i = CapturedPieces[false].Count - 1; i >= 0; i--)
            {
                CapturedPieces[true].Remove(CapturedPieces[false][i]);
                CapturedPieces[false].RemoveAt(i);
            }
            */
            return (0, CapturedPieces[false], CapturedPieces[true]);
        }
    }
}
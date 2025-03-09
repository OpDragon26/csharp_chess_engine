using Piece;
using static Piece.Presets;
using System.Collections.Generic;
using System.Linq;

namespace Board
{
    public static class MoveFinder
    {
        public static List<Move.Move> Search(Board board, bool color, bool ordering)
        {
            List<Move.Move> MoveList = new List<Move.Move>();
            List<(int, int)> Pieces = board.PiecePositions[color];
            int l = Pieces.Count;

            for (int i = 0; i < l; i++)
            {
                (int, int) coords = Pieces[i];
                Piece.Piece Selected = board.board[coords.Item2, coords.Item1];

                if (Selected.Color == color && Selected.Role != PieceType.Empty)
                {
                    MoveList.AddRange(SearchPiece(board, Selected.Role, color, new[] {coords.Item1,coords.Item2}));
                }
            }
            
            Board MoveBoard = board.DeepCopy();
            l = MoveList.Count;
            for (int i = l - 1; i >= 0; i--)
            {
                MoveBoard.MakeMove(MoveList[i], false, true);
                if (MoveBoard.KingInCheck(color))
                    MoveList.RemoveAt(i);
                
                MoveBoard.UnmakeMove();
            }
            
            if (ordering)
                MoveList.Sort((x,y) => x.Value(board).CompareTo(y.Value(board))); // Sorts the moves based on the value that has been attributed to the move
            return MoveList;
        }

        private static List<Move.Move> SearchPiece(Board board, PieceType role, bool color, int[] pos)
        {
           List<Move.Move> MoveList = new List<Move.Move>();

            if (role != PieceType.Pawn) 
            {
                Pattern PiecePattern = Patterns.PiecePatterns[role];

                if (PiecePattern.Repeat)
                {
                    int l = PiecePattern.MovePattern.Length;
                    for (int i = 0; i < l; i++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            (int,int) Target = (pos[0] + PiecePattern.MovePattern[i].Item1 * (j + 1),  pos[1] + PiecePattern.MovePattern[i].Item2 * (j + 1));
                            if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) 
                            {
                                int[] TargetSquare = {Target.Item1, Target.Item2};
                                Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                                if (TargetPiece.Role == PieceType.Empty)
                                {
                                    MoveList.Add(new Move.Move(pos, TargetSquare, Empty, PiecePattern.Importance));
                                }
                                else if (TargetPiece.Color != color)
                                {
                                    MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 5 - PiecePattern.Importance));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    int l  = PiecePattern.MovePattern.Length;
                    for (int i = 0; i < l; i++)
                    {
                        (int, int) Target = (pos[0] + PiecePattern.MovePattern[i].Item1, pos[1] + PiecePattern.MovePattern[i].Item2);
                        if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) {
                            int[] TargetSquare = {Target.Item1, Target.Item2};
                            Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                            if (TargetPiece == Empty) 
                                MoveList.Add(new Move.Move(pos, TargetSquare, Empty, PiecePattern.Importance));
                            else if (TargetPiece.Color != color) 
                                MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 5 - PiecePattern.Importance));
                        }
                    }

                    if (role == PieceType.King)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int[] TargetSquare = new int[] {pos[0] + Patterns.CastlingPattern.MovePattern[i].Item1, pos[1] + Patterns.CastlingPattern.MovePattern[i].Item2};
                            int[] SkipSquare = new int[] {pos[0] + Patterns.SkipPattern.MovePattern[i].Item1, pos[1] + Patterns.SkipPattern.MovePattern[i].Item2};
                            bool Castling = board.Castling[color][i] && board.board[TargetSquare[1],TargetSquare[0]] == Empty && board.board[SkipSquare[1],SkipSquare[0]] == Empty;

                            if (i == 1)
                            {
                                int[] LongCastleSkip = {pos[0] + Patterns.LongCastleSkip[0], pos[1] + Patterns.LongCastleSkip[1]};
                                Castling = Castling && board.board[LongCastleSkip[1],LongCastleSkip[0]] == Empty && !Attacked(board, LongCastleSkip, !color);
                            }

                            if (Castling && !board.KingInCheck(color) && !Attacked(board, SkipSquare, !color))
                            {
                                MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 2));
                            }
                        }
                    }
                }
            }
            else
            {
                PawnPattern pawnPattern = Patterns.PawnPatterns[color];

                // forward moves
                int[] TargetSquare = new int[] {pos[0] + pawnPattern.MovePattern[0,0], pos[1] + pawnPattern.MovePattern[0,1]};
                Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];
                bool SingleMove = false;

                if (TargetPiece.Role == PieceType.Empty)
                {
                    if (TargetSquare[1] == 0 || TargetSquare[1] == 7) // promotion
                    { 
                        if (color)
                        {
                            MoveList.Add(new Move.Move(pos, TargetSquare, B_Queen, 6));
                            MoveList.Add(new Move.Move(pos, TargetSquare, B_Rook, -3));
                            MoveList.Add(new Move.Move(pos, TargetSquare, B_Knight, -3));
                            MoveList.Add(new Move.Move(pos, TargetSquare, B_Bishop, -3));
                        }
                        else
                        {
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Queen, 6));
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Rook, -3));
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Knight, -3));
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Bishop, -3));
                        }
                    }
                    else // simple move
                    {
                        MoveList.Add(new Move.Move(pos, TargetSquare, Empty, -1));
                        
                        SingleMove = true;
                    }
                }
                // double move
                if (pos[1] == PawnPattern.DoubleMoveRanks[color] && SingleMove)
                {
                    TargetSquare = new [] {pos[0] + pawnPattern.MovePattern[0,0] * 2, pos[1] + pawnPattern.MovePattern[0,1] * 2};
                    TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                    if (TargetPiece == Empty)
                    {
                        MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 1));
                    }
                }
                // captures
                for (int i= 0; i < 2; i++)
                {
                    TargetSquare = new[] {pos[0] + pawnPattern.CapturePattern[i,1], pos[1] + pawnPattern.CapturePattern[i,0]};
                    if (ValidIndex(TargetSquare[0]) && ValidIndex(TargetSquare[1]))
                    {
                        TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                        if (TargetPiece.Role != PieceType.Empty && TargetPiece.Color != color || Enumerable.SequenceEqual(TargetSquare, board.EnpassantSquare))
                        {
                            if (TargetSquare[1] == 0 || TargetSquare[1] == 7) // promotion
                            { 
                                if (color)
                                {
                                    MoveList.Add(new Move.Move(pos, TargetSquare, B_Queen, 8));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, B_Rook, -3));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, B_Knight, -3));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, B_Bishop, -3));
                                }
                                else
                                {
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Queen, 8));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Rook, -3));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Knight, -3));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Bishop, -3));
                                }
                            }
                            else // simple move
                            {
                                MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 6));
                            }
                        }
                    }
                }
            }
            return MoveList;
        }

        public static bool Attacked(Board board, int[] pos, bool color) // color refers to the color that is attacking the square
        {
            // check for pieces
            for (int i = 0; i < 5; i++) 
            {
                Pattern PiecePattern = Patterns.PiecePatterns[Patterns.CheckPiecess[i]];

                if (PiecePattern.Repeat)
                {
                    int l = PiecePattern.MovePattern.Length;
                    for (int k = 0; k < l; k++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            (int, int) Target = (pos[0] + PiecePattern.MovePattern[k].Item1 * (j + 1), pos[1] + PiecePattern.MovePattern[k].Item1 * (j + 1));
                            if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) 
                            {
                                Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                                if (TargetPiece.Color == color && TargetPiece.Role == Patterns.CheckPiecess[i])
                                {
                                    return true;
                                }
                                if (TargetPiece.Role != PieceType.Empty)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    int l =  PiecePattern.MovePattern.Length;
                    for (int j = 0; j < l; j++)
                    {
                        (int, int) Target = (pos[0] + PiecePattern.MovePattern[j].Item1, pos[1] + PiecePattern.MovePattern[j].Item2);
                        if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) 
                        {
                            Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                            if (TargetPiece.Color == color && TargetPiece.Role == Patterns.CheckPiecess[i])
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // check for pawns

            int[,] CheckPattern = Patterns.PawnPatterns[!color].CapturePattern; // opposite pattern used for backwards direction
            for (int i = 0; i < 2; i++)
            {
                int[] TargetSquare = new int[] {pos[0] + CheckPattern[i,1], pos[1] + CheckPattern[i,0]};
                if (ValidIndex(TargetSquare[0]) && ValidIndex(TargetSquare[1]))
                {
                    Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                    if (TargetPiece.Role == PieceType.Pawn && TargetPiece.Color == color)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        private static bool ValidIndex(int index)
        {
            return index >= 0 && index < 8;
        }
    }

    internal class Pattern
    {
        public (int,int)[] MovePattern; // file, rank
        public bool Repeat;
        public int Importance;

        public Pattern((int,int)[] pattern, bool repeat, int  importance)
        {
            MovePattern = pattern;
            Repeat = repeat;
            Importance = importance;
        }
    }

    internal class PawnPattern
    {
        public int[,] MovePattern;
        public int[,] CapturePattern;

        public PawnPattern(int[,] movePattern, int[,] capturePattern)
        {
            MovePattern = movePattern;
            CapturePattern = capturePattern;
        }

        public static Dictionary<bool, int> DoubleMoveRanks = new Dictionary<bool, int>{
            {false, 1},
            {true, 6},
        };
    }

    public static class Patterns
    {
        internal static Dictionary<PieceType, Pattern> PiecePatterns = new Dictionary<PieceType, Pattern>{
            {PieceType.Knight, new Pattern(
                new[] {
                    (2, 1),
                    (2, -1),
                    (-2,1),
                    (-2,-1),
                    (1,2),
                    (1,-2),
                    (-1,2),
                    (-1,-2),
                },
                false,
                2
            )},
            {PieceType.Rook, new Pattern(
                new[] {
                    (0,1),
                    (0,-1),
                    (1,0),
                    (-1,0),
                },
                true,
                0
            )},
            {PieceType.Bishop, new Pattern(
                new[] {
                    (1,-1),
                    (1,1),
                    (-1,-1),
                    (-1,1),
                },
                true,
                2
            )},
            {PieceType.Queen, new Pattern(
                new[] {
                    (0,1),
                    (0,-1),
                    (1,0),
                    (-1,0),
                    (1,-1),
                    (1,1),
                    (-1,-1),
                    (-1,1),
                },
                true,
                3
            )},
            {PieceType.King, new Pattern(
                new[] {
                    (0,1),
                    (0,-1),
                    (1,0),
                    (-1,0),
                    (1,-1),
                    (1,1),
                    (-1,-1),
                    (-1,1),
                },
                false,
                -1
            )},
        };

        internal static Pattern CastlingPattern = new Pattern(
            new[] {
                (2,0),
                (-2,0),
            },
            false,
            2
        );
        internal static Pattern SkipPattern = new Pattern(
            new[] {
                (1,0),
                (-1,0),
            },
            false,
            0
        );

        internal static int[] LongCastleSkip = {-3,0};

        internal static Dictionary<bool, PawnPattern> PawnPatterns = new Dictionary<bool, PawnPattern>{
            {false,
                new PawnPattern(new[,] {
                    {0,1}
                }, new[,] {
                    {1,1},
                    {1,-1}
                })
            },
            {true,
                new PawnPattern(new[,] {
                    {0,-1}
                }, new[,] {
                    {-1,1},
                    {-1,-1}
                })
            },
        };

        internal static PieceType[] CheckPiecess = {
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.Queen,
            PieceType.Rook,
            PieceType.King
        };
    }
}
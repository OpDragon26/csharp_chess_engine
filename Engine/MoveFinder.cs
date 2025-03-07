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

            for (int i = 0; i < board.PiecePositions[color].Count; i++)
            {
                (int, int) coords = board.PiecePositions[color][i];

                if (board.board[coords.Item2,coords.Item1].Color == color && board.board[coords.Item2,coords.Item1].Role != PieceType.Empty)
                {
                    MoveList.AddRange(SearchPiece(board, board.board[coords.Item2,coords.Item1].Role, color, new[] {coords.Item1,coords.Item2}));
                }
            }
            
            if (ordering)
                MoveList.Sort((x,y) => x.Value(board).CompareTo(y.Value(board))); // Sorts the moves based on the value that has been attributed to the move
            return MoveList;
        }

        public static List<Move.Move> SearchPiece(Board board, PieceType role, bool color, int[] pos)
        {
           List<Move.Move> MoveList = new List<Move.Move>();

            if (role != PieceType.Pawn) 
            {
                Pattern PiecePattern = Patterns.PiecePatterns[role];

                if (PiecePattern.Repeat)
                {
                    for (int i = 0; i < PiecePattern.MovePattern.Length / 2; i++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            if (ValidIndex(pos[0] + PiecePattern.MovePattern[i,0] * (j + 1)) && ValidIndex(pos[1] + PiecePattern.MovePattern[i,1] * (j + 1))) 
                            {
                                int[] TargetSquare = new int[] {pos[0] + PiecePattern.MovePattern[i,0] * (j + 1), pos[1] + PiecePattern.MovePattern[i,1] * (j + 1)};
                                Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                                if (TargetPiece.Role == PieceType.Empty)
                                {
                                    MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 0));
                                }
                                else if (TargetPiece.Color != color)
                                {
                                    MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 5));
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
                    for (int i = 0; i < PiecePattern.MovePattern.Length / 2; i++)
                    {
                        if (ValidIndex(pos[0] + PiecePattern.MovePattern[i,0]) && ValidIndex(pos[1] + PiecePattern.MovePattern[i,1])) {
                            int[] TargetSquare = new[] {pos[0] + PiecePattern.MovePattern[i,0], pos[1] + PiecePattern.MovePattern[i,1]};
                            Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                            if (TargetPiece == Empty) 
                                MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 0));
                            
                            else if (TargetPiece.Color != color) 
                                MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 5));

                            
                        }
                    }

                    if (role == PieceType.King)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int[] TargetSquare = new int[] {pos[0] + Patterns.CastlingPattern.MovePattern[i,0], pos[1] + Patterns.CastlingPattern.MovePattern[i,1]};
                            int[] SkipSquare = new int[] {pos[0] + Patterns.SkipPattern.MovePattern[i,0], pos[1] + Patterns.SkipPattern.MovePattern[i,1]};
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
                            MoveList.Add(new Move.Move(pos, TargetSquare, B_Rook, -1));
                            MoveList.Add(new Move.Move(pos, TargetSquare, B_Knight, -1));
                            MoveList.Add(new Move.Move(pos, TargetSquare, B_Bishop, -1));
                        }
                        else
                        {
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Queen, 6));
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Rook, -1));
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Knight, -1));
                            MoveList.Add(new Move.Move(pos, TargetSquare, W_Bishop, -1));
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
                                    MoveList.Add(new Move.Move(pos, TargetSquare, B_Rook, 0));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, B_Knight, 0));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, B_Bishop, 0));
                                }
                                else
                                {
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Queen, 8));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Rook, 0));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Knight, 0));
                                    MoveList.Add(new Move.Move(pos, TargetSquare, W_Bishop, 0));
                                }
                            }
                            else // simple move
                            {
                                MoveList.Add(new Move.Move(pos, TargetSquare, Empty, 5));
                            }
                        }
                    }
                }
            }

            for (int i = MoveList.Count - 1; i >= 0; i--)
            {
                Board MoveBoard = board.DeepCopy();
                MoveBoard.MakeMove(MoveList[i], false);

                if (MoveBoard.KingInCheck(color))
                {
                    MoveList.RemoveAt(i);
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
                    for (int k = 0; k < PiecePattern.MovePattern.Length / 2; k++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (ValidIndex(pos[0] + PiecePattern.MovePattern[k,0] * (j + 1)) && ValidIndex(pos[1] + PiecePattern.MovePattern[k,1] * (j + 1))) 
                            {
                                int[] TargetSquare = new int[] {pos[0] + PiecePattern.MovePattern[k,0] * (j + 1), pos[1] + PiecePattern.MovePattern[k,1] * (j + 1)};
                                Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                                if (TargetPiece.Color == color && TargetPiece.Role == Patterns.CheckPiecess[i])
                                {
                                    return true;
                                }
                                else if (TargetPiece.Role != PieceType.Empty)
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
                    for (int j = 0; j < PiecePattern.MovePattern.Length / 2; j++)
                    {
                        if (ValidIndex(pos[0] + PiecePattern.MovePattern[j,0]) && ValidIndex(pos[1] + PiecePattern.MovePattern[j,1])) {
                            int[] TargetSquare = new int[] {pos[0] + PiecePattern.MovePattern[j,0], pos[1] + PiecePattern.MovePattern[j,1]};
                            Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

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

        public static bool ValidIndex(int index)
        {
            return index >= 0 && index < 8;
        }
    }

    internal class Pattern
    {
        public int[,] MovePattern; // file, rank
        public bool Repeat;

        public Pattern(int[,] pattern, bool repeat)
        {
            MovePattern = pattern;
            Repeat = repeat;
        }
    }

    internal class PawnPattern
    {
        public int[,] MovePattern = new int[,] {};
        public int[,] CapturePattern = new int[,] {};

        public static PawnPattern Constructor(int[,] movePattern, int[,] capturePattern)
        {
            PawnPattern NewPattern = new PawnPattern();
            NewPattern.MovePattern = movePattern;
            NewPattern.CapturePattern = capturePattern;
            return NewPattern;
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
                new[,] {
                    {2,1},
                    {2,-1},
                    {-2,1},
                    {-2,-1},
                    {1,2},
                    {1,-2},
                    {-1,2},
                    {-1,-2},
                },
                false
            )},
            {PieceType.Rook, new Pattern(
                new[,] {
                    {0,1},
                    {0,-1},
                    {1,0},
                    {-1,0},
                },
                true
            )},
            {PieceType.Bishop, new Pattern(
                new[,] {
                    {1,-1},
                    {1,1},
                    {-1,-1},
                    {-1,1},
                },
                true
            )},
            {PieceType.Queen, new Pattern(
                new[,] {
                    {0,1},
                    {0,-1},
                    {1,0},
                    {-1,0},
                    {1,-1},
                    {1,1},
                    {-1,-1},
                    {-1,1},
                },
                true
            )},
            {PieceType.King, new Pattern(
                new[,] {
                    {0,1},
                    {0,-1},
                    {1,0},
                    {-1,0},
                    {1,-1},
                    {1,1},
                    {-1,-1},
                    {-1,1},
                },
                false
            )},
        };

        internal static Pattern CastlingPattern = new Pattern(
            new[,] {
                {2,0},
                {-2,0},
            },
            false
        );
        internal static Pattern SkipPattern = new Pattern(
            new[,] {
                {1,0},
                {-1,0},
            },
            false
        );

        internal static int[] LongCastleSkip = {-3,0};

        internal static Dictionary<bool, PawnPattern> PawnPatterns = new Dictionary<bool, PawnPattern>{
            {false,
                PawnPattern.Constructor(new[,] {
                    {0,1}
                }, new[,] {
                    {1,1},
                    {1,-1}
                })
            },
            {true,
                PawnPattern.Constructor(new[,] {
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
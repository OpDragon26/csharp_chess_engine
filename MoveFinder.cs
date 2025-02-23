using System.Collections;
using Piece;
using static Piece.Presets;

namespace Board
{
    public static class MoveFinder
    {
        public static Move.Move[] Search(Board board, bool color)
        {
            ArrayList MoveList = new ArrayList();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board.board[i,j].Color == color && board.board[i,j].Role != PieceType.Empty)
                    {
                        Move.Move[] PieceMoves = SearchPiece(board, board.board[i,j].Role, color, new int[] {j,i});

                        for (int k = 0; k < PieceMoves.Length; k++)
                        {
                            MoveList.Add(PieceMoves[k]);
                        }
                    }
                }
            }

            Move.Move[] Moves = (Move.Move[])MoveList.ToArray(typeof(Move.Move));
            return Moves;
        }

        public static Move.Move[] SearchPiece(Board board, PieceType role, bool color, int[] pos)
        {
            ArrayList MoveList = new ArrayList();

            if (role != PieceType.Pawn) {
                Pattern PiecePattern = Patterns.PiecePatterns[role];

                if (PiecePattern.Repeat)
                {
                    for (int i = 0; i < PiecePattern.pattern.Length / 2; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (ValidIndex(pos[0] + PiecePattern.pattern[i,0] * (j + 1)) && ValidIndex(pos[1] + PiecePattern.pattern[i,1] * (j + 1))) 
                            {
                                int[] TargetSquare = new int[] {pos[0] + PiecePattern.pattern[i,0] * (j + 1), pos[1] + PiecePattern.pattern[i,1] * (j + 1)};
                                Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                                if (TargetPiece.Role == PieceType.Empty)
                                {
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, Empty));
                                }
                                else if (TargetPiece.Color != color)
                                {
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, Empty));
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
                    for (int i = 0; i < PiecePattern.pattern.Length / 2; i++)
                    {
                        if (ValidIndex(pos[0] + PiecePattern.pattern[i,0]) && ValidIndex(pos[1] + PiecePattern.pattern[i,1])) {
                            int[] TargetSquare = new int[] {pos[0] + PiecePattern.pattern[i,0], pos[1] + PiecePattern.pattern[i,1]};
                            Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                            if (TargetPiece == Empty || TargetPiece.Color != color)
                            {
                                MoveList.Add(Move.Move.Constructor(pos, TargetSquare, Empty));
                            }
                        }
                    }

                    if (role == PieceType.King)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            int[] TargetSquare = new int[] {pos[0] + Patterns.CastlingPattern.pattern[i,0], pos[1] + Patterns.CastlingPattern.pattern[i,1]};
                            int[] SkipSquare = new int[] {pos[0] + Patterns.SkipPattern.pattern[i,0], pos[1] + Patterns.SkipPattern.pattern[i,1]};
                            bool Castling = board.board[TargetSquare[1],TargetSquare[0]] == Empty && board.board[SkipSquare[1],SkipSquare[0]] == Empty && board.Castling[color][i];
                            

                            if (i == 1)
                            {
                                int[] LongCastleSkip = {pos[0] + Patterns.LongCastleSkip[0], pos[1] + Patterns.LongCastleSkip[1]};
                                Castling = Castling && board.board[LongCastleSkip[1],LongCastleSkip[0]] == Empty;
                            }

                            if (Castling)
                            {
                                MoveList.Add(Move.Move.Constructor(pos, TargetSquare, Empty));
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
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Queen));
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Rook));
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Knight));
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Bishop));
                        }
                        else
                        {
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Queen));
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Rook));
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Knight));
                            MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Bishop));
                        }
                    }
                    else // simple move
                    {
                        MoveList.Add(Move.Move.Constructor(pos, TargetSquare, Empty));
                        SingleMove = true;
                    }
                }
                // double move
                if (pos[1] == PawnPattern.DoubleMoveRanks[color] && SingleMove)
                {
                    TargetSquare = new int[] {pos[0] + pawnPattern.MovePattern[0,0] * 2, pos[1] + pawnPattern.MovePattern[0,1] * 2};
                    TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                    if (TargetPiece == Empty)
                    {
                        MoveList.Add(Move.Move.Constructor(pos, TargetSquare, Empty));
                    }
                }
                // captures
                for (int i= 0; i < 2; i++)
                {
                    TargetSquare = new int[] {pos[0] + pawnPattern.CapturePattern[i,0], pos[1] + pawnPattern.CapturePattern[i,1]};
                    if (ValidIndex(pos[0] + pawnPattern.CapturePattern[i,0]) && ValidIndex(pos[1] + pawnPattern.CapturePattern[i,1]))
                    {
                        TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                        if (TargetPiece.Role != PieceType.Empty && TargetPiece.Color != color || Enumerable.SequenceEqual(TargetSquare, board.EnpassantSquare))
                        {
                            if (TargetSquare[1] == 0 || TargetSquare[1] == 7) // promotion
                            { 
                                if (color)
                                {
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Queen));
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Rook));
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Knight));
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, B_Bishop));
                                }
                                else
                                {
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Queen));
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Rook));
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Knight));
                                    MoveList.Add(Move.Move.Constructor(pos, TargetSquare, W_Bishop));
                                }
                            }
                            else // simple move
                            {
                                MoveList.Add(Move.Move.Constructor(pos, TargetSquare, Empty));
                            }
                        }
                    }
                }
            }

            Move.Move[] Moves = (Move.Move[])MoveList.ToArray(typeof(Move.Move));
            return Moves;
        }

        public static bool Attacked(Board board, int[] pos, bool color) // color refers to the color that is attacking the square
        {
            // check for pieces
            for (int i = 0; i < 5; i++) 
            {
                Pattern PiecePattern = Patterns.PiecePatterns[Patterns.CheckPiecess[i]];

                if (PiecePattern.Repeat)
                {
                    for (int k = 0; k < PiecePattern.pattern.Length / 2; k++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (ValidIndex(pos[0] + PiecePattern.pattern[k,0] * (j + 1)) && ValidIndex(pos[1] + PiecePattern.pattern[k,1] * (j + 1))) 
                            {
                                int[] TargetSquare = new int[] {pos[0] + PiecePattern.pattern[k,0] * (j + 1), pos[1] + PiecePattern.pattern[k,1] * (j + 1)};
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
                    for (int j = 0; j < PiecePattern.pattern.Length / 2; j++)
                    {
                        if (ValidIndex(pos[0] + PiecePattern.pattern[j,0]) && ValidIndex(pos[1] + PiecePattern.pattern[j,1])) {
                            int[] TargetSquare = new int[] {pos[0] + PiecePattern.pattern[j,0], pos[1] + PiecePattern.pattern[j,1]};
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
                Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                if (TargetPiece.Role == PieceType.Pawn && TargetPiece.Color == color)
                {
                    return true;
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
        public int[,] pattern = new int[,] {}; // file, rank
        public bool Repeat = true;

        public static Pattern Constructor(int[,] pattern, bool repeat)
        {
            Pattern NewPattern = new Pattern();
            NewPattern.pattern = pattern;
            NewPattern.Repeat = repeat;
            return NewPattern;
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

    internal static class Patterns
    {
        internal static Dictionary<PieceType, Pattern> PiecePatterns = new Dictionary<PieceType, Pattern>{
            {PieceType.Knight, Pattern.Constructor(
                new int[,] {
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
            {PieceType.Rook, Pattern.Constructor(
                new int[,] {
                    {0,1},
                    {0,-1},
                    {1,0},
                    {-1,0},
                },
                true
            )},
            {PieceType.Bishop, Pattern.Constructor(
                new int[,] {
                    {1,-1},
                    {1,1},
                    {-1,-1},
                    {-1,1},
                },
                true
            )},
            {PieceType.Queen, Pattern.Constructor(
                new int[,] {
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
            {PieceType.King, Pattern.Constructor(
                new int[,] {
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

        internal static Pattern CastlingPattern = Pattern.Constructor(
            new int[,] {
                {2,0},
                {-2,0},
            },
            false
        );
        internal static Pattern SkipPattern = Pattern.Constructor(
            new int[,] {
                {1,0},
                {-1,0},
            },
            false
        );

        internal static int[] LongCastleSkip = {-3,0};

        internal static Dictionary<bool, PawnPattern> PawnPatterns = new Dictionary<bool, PawnPattern>{
            {false,
                PawnPattern.Constructor(new int[,] {
                    {0,1}
                }, new int[,] {
                    {1,1},
                    {1,-1}
                })
            },
            {true,
                PawnPattern.Constructor(new int[,] {
                    {0,-1}
                }, new int[,] {
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
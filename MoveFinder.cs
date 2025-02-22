using System.Collections;
using Move;
using Piece;
using static Piece.Presets;

namespace Board
{
    public static class MoveFinder
    {
        public static Move.Move[] Search(Board board, bool color)
        {
            ArrayList MoveList = new ArrayList();

            Move.Move[] Moves = new Move.Move[] {}; // arraylist.ToArray()
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
                            if (ValidIndex(pos[0] + PiecePattern.pattern[i,0] * (j + 1)) & ValidIndex(pos[1] + PiecePattern.pattern[i,1] * (j + 1))) 
                            {
                                int[] TargetSquare = new int[] {pos[0] + PiecePattern.pattern[i,0] * (j + 1), pos[1] + PiecePattern.pattern[i,1] * (j + 1)};
                                Piece.Piece TargetPiece = board.board[TargetSquare[1],TargetSquare[0]];

                                if (TargetPiece == Empty)
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
                        if (ValidIndex(pos[0] + PiecePattern.pattern[i,0]) & ValidIndex(pos[1] + PiecePattern.pattern[i,1])) {
                            int[] TargetSquare = new int[] {pos[0] + PiecePattern.pattern[i,0], pos[1] + PiecePattern.pattern[i,1]};
                            Piece.Piece TargetPiece = board.board[TargetSquare[0],TargetSquare[1]];

                            if (TargetPiece == Empty | TargetPiece.Color != color)
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

        public static bool ValidIndex(int index)
        {
            return index >= 0 & index < 8;
        }
    }

    internal class Pattern
    {
        public int[,] pattern = new int[,] {};
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
    }
}
using Piece;
using static Piece.Presets;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

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
                    MoveList.AddRange(SearchPiece(board, Selected.Role, color, coords));
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

        private static List<Move.Move> SearchPiece(Board board, PieceType role, bool color, (int,int) pos)
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
                            (int,int) Target = (pos.Item1 + PiecePattern.MovePattern[i].Item1 * (j + 1),  pos.Item2 + PiecePattern.MovePattern[i].Item2 * (j + 1));
                            if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) 
                            {
                                Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                                if (TargetPiece.Role == PieceType.Empty)
                                {
                                    MoveList.Add(new Move.Move(pos, Target, Empty, PiecePattern.Importance));
                                }
                                else if (TargetPiece.Color != color)
                                {
                                    MoveList.Add(new Move.Move(pos, Target, Empty, 5 - PiecePattern.Importance));
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
                        (int, int) Target = (pos.Item1 + PiecePattern.MovePattern[i].Item1, pos.Item2 + PiecePattern.MovePattern[i].Item2);
                        if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) {
                            Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                            if (TargetPiece == Empty) 
                                MoveList.Add(new Move.Move(pos, Target, Empty, PiecePattern.Importance));
                            else if (TargetPiece.Color != color) 
                                MoveList.Add(new Move.Move(pos, Target, Empty, 5 - PiecePattern.Importance));
                        }
                    }

                    if (role == PieceType.King)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            (int, int) Target = (pos.Item1 + Patterns.CastlingPattern.MovePattern[i].Item1, pos.Item2 + Patterns.CastlingPattern.MovePattern[i].Item2);
                            (int, int) SkipSquare = (pos.Item1 + Patterns.SkipPattern.MovePattern[i].Item1, pos.Item2 + Patterns.SkipPattern.MovePattern[i].Item2);
                            bool Castling = board.Castling[color][i] && board.board[Target.Item2,Target.Item1] == Empty && board.board[SkipSquare.Item2,SkipSquare.Item1] == Empty;

                            if (i == 1)
                            {
                                (int,int) LongCastleSkip = (pos.Item1 + Patterns.LongCastleSkip[0], pos.Item2 + Patterns.LongCastleSkip[1]);
                                Castling = Castling && board.board[LongCastleSkip.Item2,LongCastleSkip.Item1] == Empty && !Attacked(board, LongCastleSkip, !color);
                            }

                            if (Castling && !board.KingInCheck(color) && !Attacked(board, SkipSquare, !color))
                            {
                                MoveList.Add(new Move.Move(pos, Target, Empty, 2));
                            }
                        }
                    }
                }
            }
            else
            {
                PawnPattern pawnPattern = Patterns.PawnPatterns[color];

                // forward moves
                (int, int) Target = (pos.Item1 + pawnPattern.MovePattern[0].Item1, pos.Item2 + pawnPattern.MovePattern[0].Item2);
                Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];
                bool SingleMove = false;

                if (TargetPiece.Role == PieceType.Empty)
                {
                    if (Target.Item2 == 0 || Target.Item2 == 7) // promotion
                    { 
                        if (color)
                        {
                            MoveList.Add(new Move.Move(pos, Target, B_Queen, 6));
                            MoveList.Add(new Move.Move(pos, Target, B_Rook, -3));
                            MoveList.Add(new Move.Move(pos, Target, B_Knight, -3));
                            MoveList.Add(new Move.Move(pos, Target, B_Bishop, -3));
                        }
                        else
                        {
                            MoveList.Add(new Move.Move(pos, Target, W_Queen, 6));
                            MoveList.Add(new Move.Move(pos, Target, W_Rook, -3));
                            MoveList.Add(new Move.Move(pos, Target, W_Knight, -3));
                            MoveList.Add(new Move.Move(pos, Target, W_Bishop, -3));
                        }
                    }
                    else // simple move
                    {
                        MoveList.Add(new Move.Move(pos, Target, Empty, -1));
                        
                        SingleMove = true;
                    }
                }
                // double move
                if (pos.Item2 == PawnPattern.DoubleMoveRanks[color] && SingleMove)
                {
                    Target = (pos.Item1 + pawnPattern.MovePattern[0].Item1 * 2, pos.Item2 + pawnPattern.MovePattern[0].Item2 * 2);
                    TargetPiece = board.board[Target.Item2,Target.Item1];

                    if (TargetPiece == Empty)
                    {
                        MoveList.Add(new Move.Move(pos, Target, Empty, 1));
                    }
                }
                // captures
                for (int i= 0; i < 2; i++)
                {
                    Target = (pos.Item1 + pawnPattern.CapturePattern[i].Item2, pos.Item2 + pawnPattern.CapturePattern[i].Item1);
                    if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2))
                    {
                        TargetPiece = board.board[Target.Item2,Target.Item1];

                        if (TargetPiece.Role != PieceType.Empty && TargetPiece.Color != color || Target == board.EnpassantSquare)
                        {
                            if (Target.Item2 == 0 || Target.Item2 == 7) // promotion
                            { 
                                if (color)
                                {
                                    MoveList.Add(new Move.Move(pos, Target, B_Queen, 8));
                                    MoveList.Add(new Move.Move(pos, Target, B_Rook, -3));
                                    MoveList.Add(new Move.Move(pos, Target, B_Knight, -3));
                                    MoveList.Add(new Move.Move(pos, Target, B_Bishop, -3));
                                }
                                else
                                {
                                    MoveList.Add(new Move.Move(pos, Target, W_Queen, 8));
                                    MoveList.Add(new Move.Move(pos, Target, W_Rook, -3));
                                    MoveList.Add(new Move.Move(pos, Target, W_Knight, -3));
                                    MoveList.Add(new Move.Move(pos, Target, W_Bishop, -3));
                                }
                            }
                            else // simple move
                            {
                                MoveList.Add(new Move.Move(pos, Target, Empty, 6));
                            }
                        }
                    }
                }
            }
            return MoveList;
        }

        public static bool Attacked(Board board, (int,int) pos, bool color) // color refers to the color that is attacking the square
        {
            // check for pieces
            for (int i = 0; i < 5; i++) 
            {
                Pattern PiecePattern = Patterns.PiecePatterns[Patterns.CheckPieces[i]];

                if (PiecePattern.Repeat)
                {
                    int l = PiecePattern.MovePattern.Length;
                    for (int k = 0; k < l; k++)
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            (int, int) Target = (pos.Item1 + PiecePattern.MovePattern[k].Item1 * (j + 1), pos.Item2 + PiecePattern.MovePattern[k].Item1 * (j + 1));
                            if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) 
                            {
                                Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                                if (TargetPiece.Color == color && TargetPiece.Role == Patterns.CheckPieces[i])
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
                        (int, int) Target = (pos.Item1 + PiecePattern.MovePattern[j].Item1, pos.Item2 + PiecePattern.MovePattern[j].Item2);
                        if (ValidIndex(Target.Item1) && ValidIndex(Target.Item2)) 
                        {
                            Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                            if (TargetPiece.Color == color && TargetPiece.Role == Patterns.CheckPieces[i])
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            // check for pawns
            (int,int)[] CheckPattern = Patterns.PawnPatterns[!color].CapturePattern; // opposite pattern used for backwards direction
            for (int i = 0; i < 2; i++)
            {
                int[] TargetSquare = {pos.Item1 + CheckPattern[i].Item2, pos.Item2 + CheckPattern[i].Item1};
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
        public PatternIterator Iterator;

        public Pattern((int,int)[] pattern, bool repeat, int  importance)
        {
            MovePattern = pattern;
            Repeat = repeat;
            Importance = importance;

            if (repeat)
                Iterator = new PatternIterator(MovePattern);
            
        }
    }

    internal class PawnPattern
    {
        public readonly (int,int)[] MovePattern;
        public readonly (int,int)[] CapturePattern;

        public PawnPattern((int,int)[] movePattern, (int,int)[] capturePattern)
        {
            MovePattern = movePattern;
            CapturePattern = capturePattern;
        }

        public static readonly Dictionary<bool, int> DoubleMoveRanks = new Dictionary<bool, int>{
            {false, 1},
            {true, 6},
        };
    }

    public static class Patterns
    {
        internal static readonly Dictionary<PieceType, Pattern> PiecePatterns = new Dictionary<PieceType, Pattern>{
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
                new PawnPattern(new[] {
                    (0,1)
                }, new[] {
                    (1,1),
                    (1,-1)
                })
            },
            {true,
                new PawnPattern(new[] {
                    (0,-1)
                }, new[] {
                    (-1,1),
                    (-1,-1)
                })
            },
        };

        internal static PieceType[] CheckPieces = {
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.Queen,
            PieceType.Rook,
            PieceType.King
        };
    }

    class PatternIterator
    {
        private (PatternState, PatternState)[] PatternStates;

        public PatternIterator((int,int)[] pattern)
        {
            (PatternState, PatternState)[] newStates = new (PatternState, PatternState)[pattern.Length];
            
            for (int i = 0; i < pattern.Length; i++)
            {
                PatternState first;
                PatternState second;
                
                if (pattern[i].Item1 == 0)
                    first = PatternState.Zero;
                else if (pattern[i].Item1 > 0)
                    first = PatternState.Positive;
                else
                    first = PatternState.Negative;
                
                if (pattern[i].Item2 == 0)
                    second = PatternState.Zero;
                else if (pattern[i].Item2 > 0)
                    second = PatternState.Positive;
                else
                    second = PatternState.Negative;
                
                newStates[i] = (first, second);
            }
            PatternStates = newStates;
            
        }

        public int[] GetIterators((int, int) pos)
        {
            int l = PatternStates.Length;
            
            int[] iterators = new int[l];
            
            for (int i = 0; i < l; i++)
            {
                switch (PatternStates[i])
                {
                    case ((PatternState.Positive, PatternState.Zero)):
                        iterators[i] = 7 - pos.Item1;
                    break;
                    case ((PatternState.Negative, PatternState.Zero)):
                        iterators[i] = pos.Item1;
                    break;
                    case ((PatternState.Zero, PatternState.Positive)):
                        iterators[i] = 7 - pos.Item2;
                    break;
                    case ((PatternState.Zero, PatternState.Negative)):
                        iterators[i] = pos.Item2;
                    break;
                    case  ((PatternState.Positive, PatternState.Positive)):
                        iterators[i] = math.min(7 - pos.Item1, 7 - pos.Item2);
                    break;
                    case  ((PatternState.Positive, PatternState.Negative)):
                        iterators[i] = math.min(7 - pos.Item1, pos.Item2);
                    break;
                    case  ((PatternState.Negative, PatternState.Positive)):
                        iterators[i] = math.min(pos.Item1, 7 - pos.Item2);
                    break;
                    case  ((PatternState.Negative, PatternState.Negative)):
                        iterators[i] = math.min(pos.Item1, pos.Item2);
                    break;
                }
            }
            
            return iterators;
        }
    }

    enum PatternState
    {
        Positive,
        Negative,
        Zero
    }
}
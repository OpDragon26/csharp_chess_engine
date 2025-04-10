using System;
using Piece;
using static Piece.Presets;
using System.Collections.Generic;
using Unity.Mathematics;
using static Bitboards.Bitboards;
using static MagicNumbers.MagicNumbers;
using System.Linq;
using NUnit.Framework;

namespace Board
{
    public static class MoveFinder
    {
        public static Move.Move[] Search(Board board, bool color, bool ordering)
        {
            Move.Move[] MoveArray = new Move.Move[245];
            
            ulong Pieces = board.SideBitboards[color];
            
            int index = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((Pieces & SquareBitboards[i, j]) != 0)
                    {
                        index += SearchPieces(board, board.board[i, j].Role, color, (j,i), new Span<Move.Move>(MoveArray, index, 27));
                    }
                }
            }
            
            Move.Move[] MoveList = new Span<Move.Move>(MoveArray, 0, index).ToArray();
            
            if (ordering)
                Array.Sort(MoveList, (x,y) => x.Importance.CompareTo(y.Importance)); // Sorts the moves based on the value that has been attributed to the move
            return MoveList;
        }

        public static List<Move.Move> FilteredSearch(Board board, bool color, bool ordering)
        {
            
            List<Move.Move> MoveList = new List<Move.Move>();
            ulong Pieces = board.SideBitboards[color];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((Pieces & SquareBitboards[i, j]) != 0)
                    {
                        MoveList.AddRange(SearchPieceList(board, board.board[i, j].Role, color, (j,i)));
                    }
                }
            }
            
            return FilterChecks(MoveList, board, color);
        }

        public static int SearchPieces(Board board, PieceType role, bool color, (int,int) pos, Span<Move.Move> moveSpan)
        {
            // look up bitboards
            if (role == PieceType.Bishop)
            {
                (ulong number, int push, ulong highest) magicNumber = BishopNumbers[pos.Item1, pos.Item2];
                
                ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                ulong blockers = allPieces & BishopMask[pos.Item1, pos.Item2];
                
                ulong moves = BishopLookup[pos.Item1, pos.Item2][(blockers * magicNumber.number) >> magicNumber.push];
                return GetMovesFromBitboard(moves & ~board.SideBitboards[color], ref board.board, pos, 4, moveSpan);
                //return GetMovesFromBitboard(BishopDict[(pos, BishopMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color], pos);
            }
            
            if (role == PieceType.Rook)
            {
                (ulong number, int push, ulong highest) magicNumber = RookNumbers[pos.Item1, pos.Item2];
                
                ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                ulong blockers = allPieces & RookMask[pos.Item1, pos.Item2];
                
                ulong moves = RookLookup[pos.Item1, pos.Item2][(blockers * magicNumber.number) >> magicNumber.push];
                return GetMovesFromBitboard(moves & ~board.SideBitboards[color], ref board.board, pos, 3, moveSpan);
                //return GetMovesFromBitboard(RookDict[(pos, RookMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color], pos);
            }

            if (role == PieceType.Queen)
            {
                (ulong number, int push, ulong highest) rookMagicNumber = RookNumbers[pos.Item1, pos.Item2];
                (ulong number, int push, ulong highest) bishopMagicNumber = BishopNumbers[pos.Item1, pos.Item2];
                
                ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];

                ulong rookBlockers = allPieces & RookMask[pos.Item1, pos.Item2];
                ulong bishopBlockers = allPieces & BishopMask[pos.Item1, pos.Item2];
                
                ulong rookMoves = RookLookup[pos.Item1, pos.Item2][(rookBlockers * rookMagicNumber.number) >> rookMagicNumber.push];
                ulong bishopMoves = BishopLookup[pos.Item1, pos.Item2][(bishopBlockers * bishopMagicNumber.number) >> bishopMagicNumber.push];
                ulong allMoves =  rookMoves | bishopMoves;
                return GetMovesFromBitboard(allMoves & ~board.SideBitboards[color], ref board.board, pos, 1, moveSpan);
                //return GetMovesFromBitboard((RookDict[(pos, RookMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color]) | (BishopDict[(pos, BishopMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color]), pos);
            }

            if (role == PieceType.Knight)
                return GetMovesFromBitboard(KnightMask[pos.Item2, pos.Item1] & ~board.SideBitboards[color], ref board.board, pos, 2, moveSpan);
            
            if (role == PieceType.King)
            {
                int moves = 0;
                
                for (int i = 0; i < 2; i++)
                {
                    if (board.Castling[color][i])
                    {
                        (int, int) Target = (pos.Item1 + Patterns.CastlingPattern.MovePattern[i].Item1, pos.Item2 + Patterns.CastlingPattern.MovePattern[i].Item2);
                        (int, int) SkipSquare = (pos.Item1 + Patterns.SkipPattern.MovePattern[i].Item1, pos.Item2 + Patterns.SkipPattern.MovePattern[i].Item2);
                        bool Castling = board.board[Target.Item2,Target.Item1] == Empty && board.board[SkipSquare.Item2,SkipSquare.Item1] == Empty;

                        if (Castling && !board.KingInCheck(color) && !Attacked(board, SkipSquare, !color))
                        {
                            moveSpan[moves] = new Move.Move(pos, Target, Empty, 9);
                            moves++;
                        }
                    }
                }

                return GetMovesFromBitboard(KingMask[pos.Item2, pos.Item1] & ~board.SideBitboards[color], ref board.board, pos, -1, moveSpan, moves);
            }

            if (role == PieceType.Pawn)
            {
                if (color)
                {
                    ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                    ulong moves = BlackPawnMask[pos.Item2, pos.Item1] & ~(allPieces | ((allPieces & ~SquareBitboards[pos.Item2, pos.Item1]) << 8));
                    ulong enPassant = board.EnpassantSquare.Item1 != 8 ? SquareBitboards[board.EnpassantSquare.Item2, board.EnpassantSquare.Item1] : 0;
                    ulong captures = BlackPawnCaptureMask[pos.Item2, pos.Item1] & (board.SideBitboards[false] | enPassant);
                    
                    return GetPawnMovesFromBitboard(moves | captures, pos, moveSpan);
                }
                else
                {
                    ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                    ulong moves = WhitePawnMask[pos.Item2, pos.Item1] & ~(allPieces | ((allPieces & ~SquareBitboards[pos.Item2, pos.Item1]) >> 8));
                    ulong enPassant = board.EnpassantSquare.Item1 != 8 ? SquareBitboards[board.EnpassantSquare.Item2, board.EnpassantSquare.Item1] : 0;
                    ulong captures = WhitePawnCaptureMask[pos.Item2, pos.Item1] & (board.SideBitboards[true] | enPassant);
                    
                    return GetPawnMovesFromBitboard(moves | captures, pos, moveSpan);
                }
            }
            
            return 0;
        }
        
        public static List<Move.Move> SearchPieceList(Board board, PieceType role, bool color, (int,int) pos)
        {
            // look up bitboards
            if (role == PieceType.Bishop)
            {
                (ulong number, int push, ulong highest) magicNumber = BishopNumbers[pos.Item1, pos.Item2];
                
                ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                ulong blockers = allPieces & BishopMask[pos.Item1, pos.Item2];
                
                ulong moves = BishopLookup[pos.Item1, pos.Item2][(blockers * magicNumber.number) >> magicNumber.push];
                return GetMovesFromBitboard(moves & ~board.SideBitboards[color], ref board.board, pos, 4);
                //return GetMovesFromBitboard(BishopDict[(pos, BishopMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color], pos);
            }
            
            if (role == PieceType.Rook)
            {
                (ulong number, int push, ulong highest) magicNumber = RookNumbers[pos.Item1, pos.Item2];
                
                ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                ulong blockers = allPieces & RookMask[pos.Item1, pos.Item2];
                
                ulong moves = RookLookup[pos.Item1, pos.Item2][(blockers * magicNumber.number) >> magicNumber.push];
                return GetMovesFromBitboard(moves & ~board.SideBitboards[color], ref board.board, pos, 3);
                //return GetMovesFromBitboard(RookDict[(pos, RookMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color], pos);
            }

            if (role == PieceType.Queen)
            {
                (ulong number, int push, ulong highest) rookMagicNumber = RookNumbers[pos.Item1, pos.Item2];
                (ulong number, int push, ulong highest) bishopMagicNumber = BishopNumbers[pos.Item1, pos.Item2];
                
                ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];

                ulong rookBlockers = allPieces & RookMask[pos.Item1, pos.Item2];
                ulong bishopBlockers = allPieces & BishopMask[pos.Item1, pos.Item2];
                
                ulong rookMoves = RookLookup[pos.Item1, pos.Item2][(rookBlockers * rookMagicNumber.number) >> rookMagicNumber.push];
                ulong bishopMoves = BishopLookup[pos.Item1, pos.Item2][(bishopBlockers * bishopMagicNumber.number) >> bishopMagicNumber.push];
                ulong allMoves =  rookMoves | bishopMoves;
                return GetMovesFromBitboard(allMoves & ~board.SideBitboards[color], ref board.board, pos, 1);
                //return GetMovesFromBitboard((RookDict[(pos, RookMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color]) | (BishopDict[(pos, BishopMask[pos.Item1, pos.Item2] & (board.SideBitboards[false] | board.SideBitboards[true]))] & ~board.SideBitboards[color]), pos);
            }

            if (role == PieceType.Knight)
                return GetMovesFromBitboard(KnightMask[pos.Item2, pos.Item1] & ~board.SideBitboards[color], ref board.board, pos, 2);
            
            if (role == PieceType.King)
            {
                List<Move.Move> MoveList = new List<Move.Move>();

                for (int i = 0; i < 2; i++)
                {
                    if (board.Castling[color][i])
                    {
                        (int, int) Target = (pos.Item1 + Patterns.CastlingPattern.MovePattern[i].Item1, pos.Item2 + Patterns.CastlingPattern.MovePattern[i].Item2);
                        (int, int) SkipSquare = (pos.Item1 + Patterns.SkipPattern.MovePattern[i].Item1, pos.Item2 + Patterns.SkipPattern.MovePattern[i].Item2);
                        bool Castling = board.board[Target.Item2,Target.Item1] == Empty && board.board[SkipSquare.Item2,SkipSquare.Item1] == Empty;

                        if (Castling && !board.KingInCheck(color) && !Attacked(board, SkipSquare, !color))
                        {
                            MoveList.Add(new Move.Move(pos, Target, Empty, 9));
                        }
                    }
                }

                List<Move.Move> Moves = GetMovesFromBitboard(KingMask[pos.Item2, pos.Item1] & ~board.SideBitboards[color], ref board.board, pos, -1);
                Moves.AddRange(MoveList);
                return Moves;
            }

            if (role == PieceType.Pawn)
            {
                if (color)
                {
                    ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                    ulong moves = BlackPawnMask[pos.Item2, pos.Item1] & ~(allPieces | ((allPieces & ~SquareBitboards[pos.Item2, pos.Item1]) << 8));
                    ulong enPassant = board.EnpassantSquare.Item1 != 8 ? SquareBitboards[board.EnpassantSquare.Item2, board.EnpassantSquare.Item1] : 0;
                    ulong captures = BlackPawnCaptureMask[pos.Item2, pos.Item1] & (board.SideBitboards[false] | enPassant);
                    
                    return GetPawnMovesFromBitboard(moves | captures, pos);
                }
                else
                {
                    ulong allPieces = board.SideBitboards[false] | board.SideBitboards[true];
                    ulong moves = WhitePawnMask[pos.Item2, pos.Item1] & ~(allPieces | ((allPieces & ~SquareBitboards[pos.Item2, pos.Item1]) >> 8));
                    ulong enPassant = board.EnpassantSquare.Item1 != 8 ? SquareBitboards[board.EnpassantSquare.Item2, board.EnpassantSquare.Item1] : 0;
                    ulong captures = WhitePawnCaptureMask[pos.Item2, pos.Item1] & (board.SideBitboards[true] | enPassant);
                    
                    return GetPawnMovesFromBitboard(moves | captures, pos);
                }
            }
            
            return new List<Move.Move>();
        }
        
        public static List<Move.Move> FilterChecks(List<Move.Move> moves, Board board, bool color)
        {
            int l = moves.Count;
            for (int i = l - 1; i >= 0; i--)
            {
                Board MoveBoard = board.DeepCopy();
                MoveBoard.MakeMove(moves[i]);
                if (MoveBoard.KingInCheck(color))
                    moves.RemoveAt(i);
            }

            return moves;
        }

        private static int GetMovesFromBitboard(ulong bitboard, ref Piece.Piece[,] board, (int, int) pos, int importance, Span<Move.Move> moveSpan, int startAt=0) // overload that fills the span
        {
            int moves = startAt;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((bitboard & SquareBitboards[j, i]) != 0) // if the square isn't empty in the bitboard
                    {
                        moveSpan[moves] = new Move.Move(pos, (i,j), Empty, importance + board[j, i].LocalValue);
                        moves++;
                    }
                }
            }

            return moves;
        }
        
        
        private static List<Move.Move> GetMovesFromBitboard(ulong bitboard, ref Piece.Piece[,] board, (int, int) pos, int importance) // overload that returns a list
        {
            List<Move.Move> moves = new List<Move.Move>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((bitboard & SquareBitboards[j, i]) != 0) // if the square isn't empty in the bitboard
                        moves.Add(new Move.Move(pos, (i,j), Empty, importance + board[j, i].LocalValue));
                }
            }

            return moves;
        }
        
        private static int GetPawnMovesFromBitboard(ulong bitboard, (int, int) pos, Span<Move.Move> moveSpan) // overload that fills the span
        {
            int moves = 0;

            for (int i = 0; i < 8; i++) // for every file
            {
                for (int j = 1; j < 7; j++) // skip the 1st and 7th ranks as those are for promotions
                {
                    if ((bitboard & SquareBitboards[j, i]) != 0) // if the square isn't empty in the bitboard
                    {
                        moveSpan[moves] = new Move.Move(pos, (i,j), Empty);
                        moves++;
                    }
                }
                
                // assume j = 0, so always a promotion for black
                if ((bitboard & SquareBitboards[0, i]) != 0) // if the square isn't empty in the bitboard
                {
                    moveSpan[moves] = new Move.Move(pos, (i,0), B_Queen, 5);
                    moves++;
                    moveSpan[moves] = new Move.Move(pos, (i,0), B_Rook, -2);
                    moves++;
                    moveSpan[moves] = new Move.Move(pos, (i,0), B_Knight, -3);
                    moves++;
                    moveSpan[moves] = new Move.Move(pos, (i,0), B_Bishop, -3);
                    moves++;
                }
                // assume j = 7, so always a promotion for white
                if ((bitboard & SquareBitboards[7, i]) != 0) // if the square isn't empty in the bitboard
                {
                    moveSpan[moves] = new Move.Move(pos, (i,7), W_Queen, 5);
                    moves++;
                    moveSpan[moves] = new Move.Move(pos, (i,7), W_Rook, -2);
                    moves++;
                    moveSpan[moves] = new Move.Move(pos, (i,7), W_Knight, -3);
                    moves++;
                    moveSpan[moves] = new Move.Move(pos, (i,7), W_Bishop, -3);
                    moves++;
                }
            }

            return moves;
        }
        
        private static List<Move.Move> GetPawnMovesFromBitboard(ulong bitboard, (int, int) pos) // overload that returns a list
        {
            List<Move.Move> moves = new List<Move.Move>();

            for (int i = 0; i < 8; i++) // for every file
            {
                for (int j = 1; j < 7; j++) // skip the 1st and 7th ranks as those are for promotions
                {
                    if ((bitboard & SquareBitboards[j, i]) != 0) // if the square isn't empty in the bitboard
                        moves.Add(new Move.Move(pos, (i,j), Empty));
                }
                
                // assume j = 0, so always a promotion for black
                if ((bitboard & SquareBitboards[0, i]) != 0) // if the square isn't empty in the bitboard
                {
                    moves.Add(new Move.Move(pos, (i,0), B_Queen, 5));
                    moves.Add(new Move.Move(pos, (i,0), B_Rook, -2));
                    moves.Add(new Move.Move(pos, (i,0), B_Knight, -3));
                    moves.Add(new Move.Move(pos, (i,0), B_Bishop, -3));
                }
                // assume j = 7, so always a promotion for white
                if ((bitboard & SquareBitboards[7, i]) != 0) // if the square isn't empty in the bitboard
                {
                    moves.Add(new Move.Move(pos, (i,7), W_Queen, 5));
                    moves.Add(new Move.Move(pos, (i,7), W_Rook, -2));
                    moves.Add(new Move.Move(pos, (i,7), W_Knight, -3));
                    moves.Add(new Move.Move(pos, (i,7), W_Bishop, -3));
                }
            }

            return moves;
        }

        public static bool Attacked(Board board, (int,int) pos, bool color) // color refers to the color that is attacking the square
        {
            // check for pieces
            for (int i = 0; i < 5; i++) 
            {
                Pattern PiecePattern = Patterns.PiecePatterns[Patterns.CheckPieces[i]];

                if (PiecePattern.Repeat)
                {
                    int[] iterators = PiecePattern.Iterator.GetIterators(pos);
                    int l = PiecePattern.MovePattern.Length;
                    for (int k = 0; k < l; k++)
                    {
                        int it = iterators[k];
                        for (int j = 0; j < it; j++)
                        {
                            (int, int) Target = (pos.Item1 + PiecePattern.MovePattern[k].Item1 * (j + 1), pos.Item2 + PiecePattern.MovePattern[k].Item2 * (j + 1));

                            Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                            if (TargetPiece.Color == color && TargetPiece.Role == Patterns.CheckPieces[i])
                                return true;
                            if (TargetPiece.Role != PieceType.Empty)
                                break;
                        }
                    }
                }
                else
                {
                    int l =  PiecePattern.MovePattern.Length;
                    for (int j = 0; j < l; j++)
                    {
                        (int, int) Target = (pos.Item1 + PiecePattern.MovePattern[j].Item1, pos.Item2 + PiecePattern.MovePattern[j].Item2);
                        if (PiecePattern.Validator.Validators[j](Target)) 
                        {
                            Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];

                            if (TargetPiece.Color == color && TargetPiece.Role == Patterns.CheckPieces[i])
                                return true;
                        }
                    }
                }
            }

            // check for pawns
            PawnPattern CheckPattern = Patterns.PawnPatterns[!color]; // opposite pattern used for backwards direction
            for (int i = 0; i < 2; i++)
            {
                (int, int) Target = (pos.Item1 + CheckPattern.CapturePattern[i].Item2, pos.Item2 + CheckPattern.CapturePattern[i].Item1);
                
                if (CheckPattern.Validator.CheckValidators[i](Target)) 
                {
                    Piece.Piece TargetPiece = board.board[Target.Item2,Target.Item1];
                    
                    if (TargetPiece.Role == PieceType.Pawn && TargetPiece.Color == color)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }

    internal class Pattern
    {
        public readonly (int,int)[] MovePattern; // file, rank
        public readonly bool Repeat;
        public readonly PatternIterator Iterator;
        public readonly PatternValidator Validator;

        public Pattern((int,int)[] pattern, bool repeat)
        {
            MovePattern = pattern;
            Repeat = repeat;

            if (repeat)
                Iterator = new PatternIterator(MovePattern);
            else
                Validator = new PatternValidator(MovePattern);
        }
    }

    internal class PawnPattern
    {
        public readonly (int,int)[] CapturePattern;
        public readonly PawnPatternValidator Validator;

        public PawnPattern((int,int)[] capturePattern)
        {
            CapturePattern = capturePattern;
            Validator = new PawnPatternValidator(CapturePattern);
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
                false
            )},
            {PieceType.Rook, new Pattern(
                new[] {
                    (0,1),
                    (0,-1),
                    (1,0),
                    (-1,0),
                },
                true
            )},
            {PieceType.Bishop, new Pattern(
                new[] {
                    (1,-1),
                    (1,1),
                    (-1,-1),
                    (-1,1),
                },
                true
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
                true
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
                false
            )},
        };

        internal static readonly Pattern CastlingPattern = new(
            new[] {
                (2,0),
                (-2,0),
            },
            false
        );
        internal static readonly Pattern SkipPattern = new(
            new[] {
                (1,0),
                (-1,0),
            },
            false
        );
        
        internal static readonly Dictionary<bool, PawnPattern> PawnPatterns = new Dictionary<bool, PawnPattern>{
            {false,
                new PawnPattern(new[] {
                    (1,1),
                    (1,-1)
                })
            },
            {true,
                new PawnPattern(new[] {
                    (-1,1),
                    (-1,-1)
                })
            },
        };

        internal static readonly PieceType[] CheckPieces = {
            PieceType.Knight,
            PieceType.Bishop,
            PieceType.Queen,
            PieceType.Rook,
            PieceType.King
        };
    }

    class PatternIterator
    {
        private readonly (PatternState, PatternState)[] PatternStates;
        private readonly List<Func<(int,int) ,int>> IteratorCalculators  = new List<Func<(int,int),int>>();

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
                
                switch (newStates[i])
                {
                    case ((PatternState.Positive, PatternState.Zero)):
                        IteratorCalculators.Add(( pos) =>  7 - pos.Item1);
                    break;
                    case ((PatternState.Negative, PatternState.Zero)):
                        IteratorCalculators.Add(( pos) =>  pos.Item1);
                    break;
                    case ((PatternState.Zero, PatternState.Positive)):
                        IteratorCalculators.Add(( pos) =>  7 - pos.Item2);
                    break;
                    case ((PatternState.Zero, PatternState.Negative)):
                        IteratorCalculators.Add(( pos) =>  pos.Item2);
                    break;
                    case  ((PatternState.Positive, PatternState.Positive)):
                        IteratorCalculators.Add(( pos) =>  math.min(7 - pos.Item1, 7 - pos.Item2));
                    break;
                    case  ((PatternState.Positive, PatternState.Negative)):
                        IteratorCalculators.Add(( pos) =>  math.min(7 - pos.Item1, pos.Item2));
                    break;
                    case  ((PatternState.Negative, PatternState.Positive)):
                        IteratorCalculators.Add(( pos) =>  math.min(pos.Item1, 7 - pos.Item2));
                    break;
                    case  ((PatternState.Negative, PatternState.Negative)):
                        IteratorCalculators.Add(( pos) =>  math.min(pos.Item1, pos.Item2));
                    break;
                }
            }
            PatternStates = newStates;
            
        }

        public int[] GetIterators((int, int) pos)
        {
            int l = PatternStates.Length;
            
            int[] iterators = new int[l];
            
            for (int i = 0; i < l; i++)
            {
                iterators[i] = IteratorCalculators[i](pos);
            }
            
            return iterators;
        }
    }

    class PatternValidator
    {
        public readonly List<Func<(int, int), bool>> Validators = new List<Func<(int, int), bool>>();

        public PatternValidator((int, int)[] pattern)
        {
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
                
                switch ((first, second))
                {
                    case ((PatternState.Positive, PatternState.Zero)):
                        Validators.Add((target) => target.Item1 < 8);
                    break;
                    case ((PatternState.Negative, PatternState.Zero)):
                        Validators.Add((target) => target.Item1 >= 0);
                    break;
                    case ((PatternState.Zero, PatternState.Positive)):
                        Validators.Add((target) => target.Item2 < 8);
                    break;
                    case ((PatternState.Zero, PatternState.Negative)):
                        Validators.Add((target) => target.Item2 >= 0);
                    break;
                    case ((PatternState.Positive, PatternState.Positive)):
                        Validators.Add((target) => target.Item1 < 8 && target.Item2 < 8);
                    break;
                    case ((PatternState.Positive, PatternState.Negative)):
                        Validators.Add((target) => target.Item1 < 8 && target.Item2 >= 0);
                    break;
                    case ((PatternState.Negative, PatternState.Positive)):
                        Validators.Add((target) => target.Item1 >= 0 && target.Item2 < 8);
                    break;
                    case ((PatternState.Negative, PatternState.Negative)):
                        Validators.Add((target) => target.Item1 >= 0 && target.Item2 >= 0);
                    break;
                }
            }
        }
    }

    class PawnPatternValidator
    {
        public List<Func<(int,int), bool>> CheckValidators = new List<Func<(int,int), bool>>();

        public PawnPatternValidator((int, int)[] pattern)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                PatternState second;
                PatternState first;
                    
                if (pattern[i].Item2 > 0)
                    second = PatternState.Positive;
                else
                    second = PatternState.Negative;
                
                if (pattern[i].Item1 > 0)
                    first = PatternState.Positive;
                else
                    first = PatternState.Negative;

                switch ((second, first))
                {
                    case ((PatternState.Positive, PatternState.Positive)):
                        CheckValidators.Add((target) =>  target.Item1 < 8 && target.Item2 < 8);
                    break;
                    case ((PatternState.Positive, PatternState.Negative)):
                        CheckValidators.Add((target) =>  target.Item1 < 8 && target.Item2 >= 0);
                    break;
                    case ((PatternState.Negative, PatternState.Positive)):
                        CheckValidators.Add((target) =>  target.Item1 >= 0 && target.Item2 < 8);
                    break;
                    case (PatternState.Negative, PatternState.Negative):
                        CheckValidators.Add((target) =>  target.Item1 >= 0 && target.Item2 >= 0);
                    break;
                }
            }
        }
    }
    

    enum PatternState
    {
        Positive,
        Negative,
        Zero
    }
}
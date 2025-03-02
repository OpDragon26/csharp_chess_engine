using static HashCodeHelper.HashCodeHelper;
using Piece;
using static Piece.Presets;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

namespace Board
{
    public class Board
    {
        public Piece.Piece[,] board = new Piece.Piece[8,8];
        public Dictionary<bool, bool[]> Castling = new Dictionary<bool, bool[]> {
            {false, new bool[] {true, true}}, // Short, Long
            {true, new bool[] {true, true}},
        };

        public int[] EnpassantSquare = {8,8}; // file, rank  8,8 for no en passant
        public bool Side = false;

        // 50 move rule
        public int MoveChain = 0;
        public Dictionary<Board, int> Repetition = new Dictionary<Board, int>{};
        public Outcome DeclaredOutcome = Outcome.Ongoing;
        public Dictionary<bool, int[]> KingPos = new Dictionary<bool, int[]>{
            {true, new int[] {8,8}},
            {false, new int[] {8,8}},
        };
        public Dictionary<bool, List<(int,int)>> PiecePositions = new Dictionary<bool, List<(int,int)>>{
            {false, new List<(int,int)>()},
            {true, new List<(int,int)>()},
        };

        public bool MakeMove(Move.Move move, bool filter)
        {
            if (filter)
            {
                List<Move.Move> LegalMoves = MoveFinder.Search(this, this.Side);
                if (!move.InMovelist(LegalMoves))
                {
                    return false;
                }
            }

            if (this.board[move.To[1],move.To[0]].Role != PieceType.Empty)
            {
                this.PiecePositions[this.board[move.To[1],move.To[0]].Color].Remove((move.To[0],move.To[1]));
            }
            this.PiecePositions[this.board[move.From[1],move.From[0]].Color].Remove((move.From[0],move.From[1]));
            this.PiecePositions[this.board[move.From[1],move.From[0]].Color].Add((move.To[0],move.To[1]));

            if (this.board[move.From[1],move.From[0]].Role == PieceType.Pawn || this.board[move.To[1],move.To[0]].Role != PieceType.Empty)
            {
                this.MoveChain = 0;
            }
            else if (this.Side == true)
            {
                this.MoveChain++;
            }

            if (move.Promotion != Empty)
            {
                this.board[move.To[1],move.To[0]] = move.Promotion;
            }
            else
            {
                this.board[move.To[1],move.To[0]] = this.board[move.From[1],move.From[0]];
            }
            this.board[move.From[1],move.From[0]] = Empty;
            
            // castling
            if (Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.WKStartPos) && this.board[move.To[1],move.To[0]].Role == PieceType.King)
            {
                if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WKShortCastlePos) && this.Castling[false][0])
                {
                    this.board[Presets.WRShortCastlePos[0],Presets.WRShortCastlePos[1]] = Empty;
                    this.board[Presets.WRShortCastleDest[0],Presets.WRShortCastleDest[1]] = W_Rook;
                    this.PiecePositions[false].Add((Presets.WRShortCastleDest[1],Presets.WRShortCastleDest[0]));
                    this.PiecePositions[false].Remove((Presets.WRShortCastlePos[1],Presets.WRShortCastlePos[0]));
                    this.Castling[false] = new bool[] {false, false};

                } 
                else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WKLongCastlePos) && this.Castling[false][1])
                {
                    this.board[Presets.WRLongCastlePos[0],Presets.WRLongCastlePos[1]] = Empty;
                    this.board[Presets.WRLongCastleDest[0],Presets.WRLongCastleDest[1]] = W_Rook;
                    this.PiecePositions[false].Add((Presets.WRLongCastleDest[1],Presets.WRLongCastleDest[0]));
                    this.PiecePositions[false].Remove((Presets.WRLongCastlePos[1],Presets.WRLongCastlePos[0]));
                    this.Castling[false] = new bool[] {false, false};
                }
            } 
            else if (Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.BKStartPos) && this.board[move.To[1],move.To[0]].Role == PieceType.King)
            {
                if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BKShortCastlePos) && this.Castling[true][0])
                {
                    this.board[Presets.BRShortCastlePos[0],Presets.BRShortCastlePos[1]] = Empty;
                    this.board[Presets.BRShortCastleDest[0],Presets.BRShortCastleDest[1]] = B_Rook;
                    this.PiecePositions[false].Add((Presets.BRShortCastleDest[1],Presets.BRShortCastleDest[0]));
                    this.PiecePositions[false].Remove((Presets.BRShortCastlePos[1],Presets.BRShortCastlePos[0]));
                    this.Castling[true] = new bool[] {false, false};
                } 
                else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BKLongCastlePos) && this.Castling[true][1])
                {
                    this.board[Presets.BRLongCastlePos[0],Presets.BRLongCastlePos[1]] = Empty;
                    this.board[Presets.BRLongCastleDest[0],Presets.BRLongCastleDest[1]] = B_Rook;
                    this.PiecePositions[false].Add((Presets.BRLongCastleDest[1],Presets.BRLongCastleDest[0]));
                    this.PiecePositions[false].Remove((Presets.BRLongCastlePos[1],Presets.BRLongCastlePos[0]));
                    this.Castling[true] = new bool[] {false, false};
                }
            }
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WhiteRookHPos) || Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.WhiteRookHPos))
            {
                this.Castling[false][0] = false;
            }
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WhiteRookAPos) || Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.WhiteRookAPos))
            {
                this.Castling[false][1] = false;
            }
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BlackRookHPos) || Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.BlackRookHPos))
            {
                this.Castling[true][0] = false;
            }
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BlackRookAPos) || Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.BlackRookAPos))
            {
                this.Castling[true][1] = false;
            }

            // en passant (Holy Hell!)
            if (this.board[move.To[1],move.To[0]].Role == PieceType.Pawn)
            {
                int RankDistance = move.From[1] - move.To[1];

                if (Enumerable.SequenceEqual(move.To, this.EnpassantSquare))
                {
                    if (this.board[move.To[1] + 1,move.To[0]].Role == PieceType.Pawn)
                    {
                        this.PiecePositions[this.board[move.To[1] + 1,move.To[0]].Color].Remove((move.To[0],move.To[1] + 1));
                        this.board[move.To[1] + 1,move.To[0]] = Empty;
                    }
                    else if (this.board[move.To[1] - 1,move.To[0]].Role == PieceType.Pawn)
                    {
                        this.PiecePositions[this.board[move.To[1] - 1,move.To[0]].Color].Remove((move.To[0],move.To[1] - 1));
                        this.board[move.To[1] - 1,move.To[0]] = Empty;
                    }
                }
                else if (RankDistance == 2 || RankDistance == -2)
                {
                    this.EnpassantSquare = new int[] {move.From[0], move.To[1] + (RankDistance / 2)};
                }
                else
                {
                    this.EnpassantSquare = new int[] {8,8};
                }
            }
            else if (this.board[move.To[1],move.To[0]].Role == PieceType.King)
            {
                this.KingPos[this.board[move.To[1],move.To[0]].Color] = new int[] {move.To[0],move.To[1]};
                this.Castling[this.board[move.To[1],move.To[0]].Color] = new bool[] {false, false};
                this.EnpassantSquare = new int[] {8,8};
            }
            else
            {
                this.EnpassantSquare = new int[] {8,8};
            }

            // changing kingpos


            this.Side = !this.Side;
            
            this.AddSelf();

            return true;
        }

        public static Board Constructor(Piece.Piece[,] board, bool side, bool[] whiteCastle, bool[] blackCastle, int[] enpassantSquare, int moveChain)
        {
            Board NewBoard = new Board();
            NewBoard.board = board;
            NewBoard.Castling = new Dictionary<bool, bool[]> {{false, blackCastle},{true, whiteCastle}};
            NewBoard.EnpassantSquare = enpassantSquare;
            NewBoard.Side = side;
            NewBoard.MoveChain = moveChain;
            NewBoard.KingPos[false] = NewBoard.GetKingPos(false);
            NewBoard.KingPos[true] = NewBoard.GetKingPos(true);
            NewBoard.PiecePositions[false] = NewBoard.GetPiecePositions(false);
            NewBoard.PiecePositions[true] = NewBoard.GetPiecePositions(true);
            return NewBoard;
        }

        public Board DeepCopy()
        {
            Board Clone = new Board();

            Piece.Piece[,] CloneBoard = new Piece.Piece[8,8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    CloneBoard[i,j] = Piece.Presets.Clone[this.board[i,j]];
                }
            }
            Clone.board = CloneBoard;
            Clone.Castling = new Dictionary<bool, bool[]> {
                {false, new bool[] {this.Castling[false][0], this.Castling[false][1]}},
                {true, new bool[] {this.Castling[true][0], this.Castling[true][1]}}};

            Clone.EnpassantSquare = new int[] {this.EnpassantSquare[0], this.EnpassantSquare[1]};
            Clone.Side = this.Side == true;
            Clone.MoveChain = this.MoveChain;

            Clone.PiecePositions[false] = new List<(int, int)>(this.PiecePositions[false]);
            Clone.PiecePositions[true] = new List<(int, int)>(this.PiecePositions[true]);
            
            Clone.KingPos[false] = new int[] {this.KingPos[false][0],this.KingPos[false][1]};
            Clone.KingPos[true] = new int[] {this.KingPos[true][0],this.KingPos[true][1]};
            return Clone;
        }

        public bool KingInCheck(bool color)
        {
            return MoveFinder.Attacked(this, this.GetKingPos(color), !color);
        }

        public int[] GetKingPos(bool color)
        {
            if (this.KingPos[color][0] != 8)
            {
                return this.KingPos[color];
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (this.board[i,j].Role == PieceType.King && this.board[i,j].Color == color)
                    {
                        this.KingPos[color] = new int[] {j,i};
                        return new int[] {j,i};
                    }
                }
            }
            return new int[] {8,8};
        }

        public List<(int,int)> GetPiecePositions(bool Side)
        {
            if (this.PiecePositions[Side].Count == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (this.board[i,j].Color == Side && this.board[i,j].Role != PieceType.Empty)
                        {
                            this.PiecePositions[Side].Add((j,i));
                        }
                    }
                }
            }
            return this.PiecePositions[Side];
        }

        int[] LocalValue()
        {
            int White = 0;
            int Black = 0;
            for (int i = 0; i < PiecePositions[false].Count; i++)
            {
                (int, int) coords = ((int, int))PiecePositions[false][i];

                White += board[coords.Item2,coords.Item1].LocalValue;
            }
            for (int i = 0; i < PiecePositions[true].Count; i++)
            {
                (int, int) coords = ((int, int))PiecePositions[true][i];

                Black += board[coords.Item2,coords.Item1].LocalValue;
            }

            return new int[] {White, Black};
        }

        bool PawnsLeft()
        {
            for (int i = 0; i < PiecePositions[false].Count; i++)
            {
                (int, int) coords = ((int, int))PiecePositions[false][i];

                if (board[coords.Item2,coords.Item1].Role == PieceType.Pawn)
                return true;
            }
            for (int i = 0; i < PiecePositions[true].Count; i++)
            {
                (int, int) coords = ((int, int))PiecePositions[true][i];

                if (board[coords.Item2,coords.Item1].Role == PieceType.Pawn)
                return true;
            }

            return false;
        }

        public Outcome Status()
        {
            if (this.DeclaredOutcome == Outcome.Ongoing)
            {
                if (MoveFinder.Search(this, Side).Count == 0)
                {
                    if (this.KingInCheck(Side))
                    {
                        DeclaredOutcome = Presets.SideOutcomes[!Side];
                    }
                    else
                    {
                        DeclaredOutcome = Outcome.Draw;
                    }
                }
                else if (this.MoveChain > 49 || this.Repetition.ContainsValue(3))
                {
                    DeclaredOutcome = Outcome.Draw;
                }
                else
                {
                    int[] LocalValues = this.LocalValue();

                    if (LocalValues[0] < 1400 && LocalValues[1] < 1400 && !this.PawnsLeft())
                    {
                        DeclaredOutcome = Outcome.Draw;
                    }
                }
            }
            return DeclaredOutcome;
        }

        public override int GetHashCode()
        {
            int Hash = GetBoardHash(this.board);
            Hash = Hash * 31 + this.Side.GetHashCode();
            Hash = Hash * 31 + GetArrayHash(this.EnpassantSquare);
            Hash = Hash * 31 + GetArrayHash(this.Castling[false]);
            Hash = Hash * 31 + GetArrayHash(this.Castling[true]);

            return Hash;
        }

        void AddSelf()
        {
            if (Repetition.ContainsKey(this))
            {
                Repetition[this]++;
            }
            else
            {
                Repetition[this] = 1;
            }
        }
    }

    public static class Presets {
        public static Piece.Piece[,] StartingPosition = new Piece.Piece[,] 
        {
            {W_Rook, W_Knight, W_Bishop, W_Queen, W_King, W_Bishop, W_Knight, W_Rook},
            {W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn},
            {B_Rook, B_Knight, B_Bishop, B_Queen, B_King, B_Bishop, B_Knight, B_Rook},
        };
        public static Board StartingBoard = Board.Constructor(StartingPosition, false, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8}, 0);

        internal static int[] WKStartPos = {0,4}; // file, rank

        // white short castle
        internal static int[] WKShortCastlePos = {0,6};
        internal static int[] WRShortCastlePos = {0,7};
        internal static int[] WRShortCastleDest = {0,5};

        // white long castle
        internal static int[] WKLongCastlePos = {0,2};
        internal static int[] WRLongCastlePos = {0,0};
        internal static int[] WRLongCastleDest = {0,3};

        internal static int[] BKStartPos = {7,4}; // file, rank

        // black short castle
        internal static int[] BKShortCastlePos = {7,6};
        internal static int[] BRShortCastlePos = {7,7};
        internal static int[] BRShortCastleDest = {7,5};

        // black long castle
        internal static int[] BKLongCastlePos = {7,2};
        internal static int[] BRLongCastlePos = {7,0};
        internal static int[] BRLongCastleDest = {7,3};

        // rook positions
        internal static int[] WhiteRookAPos = {0,0};
        internal static int[] WhiteRookHPos = {0,7};
        internal static int[] BlackRookAPos = {7,0};
        internal static int[] BlackRookHPos = {7,7};

        public static Dictionary<string, int> FileIndex = new Dictionary<string, int>{
            {"a",0},
            {"b",1},
            {"c",2},
            {"d",3},
            {"e",4},
            {"f",5},
            {"g",6},
            {"h",7},
        };

        public static int[] ConvertSquare(string square, bool reverse)
        {
            if (!reverse)
            {
                return new int[] {FileIndex[square[0].ToString()], Int32.Parse(square[1].ToString()) - 1};
            } else {
                return new int[] {Int32.Parse(square[1].ToString()) - 1, FileIndex[square[0].ToString()]};
            }
        }

        public static Dictionary<bool, Outcome> SideOutcomes = new Dictionary<bool, Outcome>{
            {false, Outcome.White},
            {true, Outcome.Black},
        };
    }

    public enum Outcome
    {
        Ongoing,
        White,
        Black,
        Draw
    }
    public static class TestCases
    {

    }
}
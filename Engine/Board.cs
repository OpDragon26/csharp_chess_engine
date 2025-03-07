using static HashCodeHelper.HashCodeHelper;
using Piece;
using static Piece.Presets;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Board
{
    public class Board
    {
        public Piece.Piece[,] board = new Piece.Piece[8,8];
        public Dictionary<bool, bool[]> Castling = new Dictionary<bool, bool[]> {
            {false, new [] {true, true}}, // Short, Long
            {true, new [] {true, true}},
        };

        public int[] EnpassantSquare = {8,8}; // file, rank  8,8 for no en passant
        public bool Side;

        // 50 move rule
        private int MoveChain;
        private Dictionary<int, int> Repetition =  new Dictionary<int, int>();
        private Outcome DeclaredOutcome = Outcome.Ongoing;
        
        private Dictionary<bool, int[]> KingPos = new Dictionary<bool, int[]>{
            {true, new[] {8,8}},
            {false, new[] {8,8}},
        };
        public Dictionary<bool, List<(int,int)>> PiecePositions = new Dictionary<bool, List<(int,int)>>{
            {false, new List<(int,int)>()},
            {true, new List<(int,int)>()},
        };

        public int PieceCounter;

        private ReverseMove LastMove;

        public bool MakeMove(Move.Move move, bool filter, bool generateReverse)
        {
            if (filter)
            {
                List<Move.Move> LegalMoves = MoveFinder.Search(this, this.Side, false);
                if (!move.InMovelist(LegalMoves))
                {
                    return false;
                }
            }
             
            // local variables storing data that is accessed over and over
            (int, int) MoveTo = (move.To[0],move.To[1]);
            (int, int) MoveFrom = (move.From[0], move.From[1]);
            Piece.Piece OriginPiece = this.board[move.From[1],move.From[0]];
            Piece.Piece TargetPiece = this.board[move.To[1],move.To[0]];
            bool OriginColor = OriginPiece.Color;
            bool TargetColor = TargetPiece.Color;
            
            // Local variables to store all the data that will be required to generate the ReverseMove
            ((int, int), (int, int)) extraMove = ((8, 8), (8, 8));
            (int, int) enpassant = (8,8);
            int moveChain = this.MoveChain;
            int[] prevEnpassant = this.EnpassantSquare;
            bool[] whiteCastle = this.Castling[false];
            bool[] blackCastle = this.Castling[true];
            int[] wKingPos =  this.KingPos[false];
            int[] bKingPos = this.KingPos[true];


            if (TargetPiece.Role != PieceType.Empty)
            {
                this.PiecePositions[TargetColor].Remove(MoveTo);
                if (TargetPiece.Role != PieceType.Pawn)
                    this.PieceCounter -= TargetPiece.LocalValue;
            }
                
            
            this.PiecePositions[OriginColor].Remove(MoveFrom);
            this.PiecePositions[OriginColor].Add(MoveTo);

            if (OriginPiece.Role == PieceType.Pawn || TargetPiece.Role != PieceType.Empty)
                this.MoveChain = 0;
            else if (this.Side) 
                this.MoveChain++;
            

            if (move.Promotion != Empty)
                this.board[move.To[1],move.To[0]] = move.Promotion;
            else
                this.board[move.To[1],move.To[0]] = OriginPiece;
            
            this.board[move.From[1],move.From[0]] = Empty;
            
            // castling
            if (Enumerable.SequenceEqual(new[] {move.From[1],move.From[0]}, Presets.WKStartPos) && OriginPiece.Role == PieceType.King)
            {
                if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.WKShortCastlePos) && this.Castling[false][0])
                {
                    this.board[Presets.WRShortCastlePos[0],Presets.WRShortCastlePos[1]] = Empty;
                    this.board[Presets.WRShortCastleDest[0],Presets.WRShortCastleDest[1]] = W_Rook;
                    this.PiecePositions[false].Add((Presets.WRShortCastleDest[1],Presets.WRShortCastleDest[0]));
                    this.PiecePositions[false].Remove((Presets.WRShortCastlePos[1],Presets.WRShortCastlePos[0]));
                    this.Castling[false] = new[] {false, false};

                    extraMove = ((Presets.WRShortCastlePos[1],Presets.WRShortCastlePos[0]), (Presets.WRShortCastleDest[1],Presets.WRShortCastleDest[0]));
                } 
                else if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.WKLongCastlePos) && this.Castling[false][1])
                {
                    this.board[Presets.WRLongCastlePos[0],Presets.WRLongCastlePos[1]] = Empty;
                    this.board[Presets.WRLongCastleDest[0],Presets.WRLongCastleDest[1]] = W_Rook;
                    this.PiecePositions[false].Add((Presets.WRLongCastleDest[1],Presets.WRLongCastleDest[0]));
                    this.PiecePositions[false].Remove((Presets.WRLongCastlePos[1],Presets.WRLongCastlePos[0]));
                    this.Castling[false] = new[] {false, false};
                    
                    extraMove = ((Presets.WRLongCastlePos[1],Presets.WRLongCastlePos[0]), (Presets.WRLongCastleDest[1],Presets.WRLongCastleDest[0]));
                }
            } 
            else if (Enumerable.SequenceEqual(new[] {move.From[1],move.From[0]}, Presets.BKStartPos) && OriginPiece.Role == PieceType.King)
            {
                if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.BKShortCastlePos) && this.Castling[true][0])
                {
                    this.board[Presets.BRShortCastlePos[0],Presets.BRShortCastlePos[1]] = Empty;
                    this.board[Presets.BRShortCastleDest[0],Presets.BRShortCastleDest[1]] = B_Rook;
                    this.PiecePositions[true].Add((Presets.BRShortCastleDest[1],Presets.BRShortCastleDest[0]));
                    this.PiecePositions[true].Remove((Presets.BRShortCastlePos[1],Presets.BRShortCastlePos[0]));
                    this.Castling[true] = new[] {false, false};
                    
                    extraMove = ((Presets.BRShortCastlePos[1],Presets.BRShortCastlePos[0]), (Presets.BRShortCastleDest[1],Presets.BRShortCastleDest[0]));
                } 
                else if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.BKLongCastlePos) && this.Castling[true][1])
                {
                    this.board[Presets.BRLongCastlePos[0],Presets.BRLongCastlePos[1]] = Empty;
                    this.board[Presets.BRLongCastleDest[0],Presets.BRLongCastleDest[1]] = B_Rook;
                    this.PiecePositions[true].Add((Presets.BRLongCastleDest[1],Presets.BRLongCastleDest[0]));
                    this.PiecePositions[true].Remove((Presets.BRLongCastlePos[1],Presets.BRLongCastlePos[0]));
                    this.Castling[true] = new[] {false, false};
                    
                    extraMove = ((Presets.BRLongCastlePos[1],Presets.BRLongCastlePos[0]), (Presets.BRLongCastlePos[1],Presets.BRLongCastlePos[0]));
                }
            }
            // Remove castling rights
            else if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.WhiteRookHPos) || Enumerable.SequenceEqual(new[] {move.From[1],move.From[0]}, Presets.WhiteRookHPos))
                this.Castling[false][0] = false;
            else if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.WhiteRookAPos) || Enumerable.SequenceEqual(new[] {move.From[1],move.From[0]}, Presets.WhiteRookAPos))
                this.Castling[false][1] = false;
            else if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.BlackRookHPos) || Enumerable.SequenceEqual(new[] {move.From[1],move.From[0]}, Presets.BlackRookHPos))
                this.Castling[true][0] = false;
            else if (Enumerable.SequenceEqual(new[] {move.To[1],move.To[0]}, Presets.BlackRookAPos) || Enumerable.SequenceEqual(new[] {move.From[1],move.From[0]}, Presets.BlackRookAPos))
                this.Castling[true][1] = false;
            

            // en passant (Holy Hell!)
            if (OriginPiece.Role == PieceType.Pawn)
            {
                int RankDistance = move.From[1] - move.To[1];

                if (Enumerable.SequenceEqual(move.To, this.EnpassantSquare))
                {
                    if (this.board[move.To[1] + 1,move.To[0]].Role == PieceType.Pawn)
                    {
                        this.PiecePositions[this.board[move.To[1] + 1,move.To[0]].Color].Remove((move.To[0],move.To[1] + 1));
                        this.board[move.To[1] + 1,move.To[0]] = Empty;
                        enpassant = (move.To[1] + 1,move.To[0]);
                    }
                    else if (this.board[move.To[1] - 1,move.To[0]].Role == PieceType.Pawn)
                    {
                        this.PiecePositions[this.board[move.To[1] - 1,move.To[0]].Color].Remove((move.To[0],move.To[1] - 1));
                        this.board[move.To[1] - 1,move.To[0]] = Empty;
                        enpassant = (move.To[1] - 1,move.To[0]);
                    }
                }
                else if (RankDistance == 2 || RankDistance == -2) // if the pawn made 2 moves forward, set EnpassantSquare
                    this.EnpassantSquare = new[] {move.From[0], move.To[1] + (RankDistance / 2)};
                else
                    this.EnpassantSquare = new[] {8,8};
                
            }
            else if (OriginPiece.Role == PieceType.King) // Changing KingPos
            {
                this.KingPos[OriginColor] = new[] {move.To[0],move.To[1]};
                this.Castling[OriginColor] = new[] {false, false};
                this.EnpassantSquare = new[] {8,8};
            }
            else
                this.EnpassantSquare = new[] {8,8};
            
            if (generateReverse)
                LastMove = new ReverseMove((MoveFrom, MoveTo), extraMove, TargetPiece, move.Promotion != Empty, enpassant, prevEnpassant, moveChain, whiteCastle, blackCastle, wKingPos, bKingPos);
            
            this.Side = !this.Side;
            
            this.AddSelf();

            return true;
        }

        private static Dictionary<bool, Piece.Piece> Pawns = new Dictionary<bool, Piece.Piece>
        {
            {false, W_Pawn},
            {true, B_Pawn}
        };
        public void UnmakeMove()
        {
            // Unmaking the original move
            if (LastMove.Promotion)
                this.board[LastMove.OriginMove.Item1.Item2, LastMove.OriginMove.Item1.Item1] = Pawns[!this.Side];
            else
                this.board[LastMove.OriginMove.Item1.Item2, LastMove.OriginMove.Item1.Item1] = this.board[LastMove.OriginMove.Item2.Item2, LastMove.OriginMove.Item2.Item1];
            this.PiecePositions[!this.Side].Add(LastMove.OriginMove.Item1);
            
            this.board[LastMove.OriginMove.Item2.Item2, LastMove.OriginMove.Item2.Item1] = LastMove.CapturedPiece;
            this.PiecePositions[this.Side].Remove(LastMove.OriginMove.Item2);
            
            if (LastMove.CapturedPiece.Role != PieceType.Pawn)
                this.PieceCounter += LastMove.CapturedPiece.LocalValue;
            
            // unmaking the extra move, if there is one
            if (LastMove.ExtraMove.Item1.Item1 != 8)
            {
                this.board[LastMove.ExtraMove.Item1.Item2, LastMove.ExtraMove.Item1.Item1] = this.board[LastMove.ExtraMove.Item2.Item2, LastMove.ExtraMove.Item2.Item1];
                this.board[LastMove.ExtraMove.Item2.Item2, LastMove.ExtraMove.Item2.Item1] = Empty;
                
                this.PiecePositions[!this.Side].Add(LastMove.ExtraMove.Item1);
                this.PiecePositions[!this.Side].Remove(LastMove.ExtraMove.Item2);
            }
            
            // if there was an en passant capture, put the pawn back
            if (LastMove.Enpassant.Item1 != 8)
            {
                this.board[LastMove.Enpassant.Item2, LastMove.Enpassant.Item1] = Pawns[this.Side];
                this.PiecePositions[this.Side].Add(LastMove.Enpassant);
            }
            
            this.MoveChain = LastMove.MoveChain;
            this.EnpassantSquare = new[] {LastMove.PrevEnpassant[0], LastMove.PrevEnpassant[1]};

            this.Castling[false] = new[] { LastMove.WhiteCastle[0], LastMove.WhiteCastle[1] };
            this.Castling[true] = new[] { LastMove.BlackCastle[0], LastMove.BlackCastle[1] };
            
            this.KingPos[false] =  new[] { LastMove.WKingPos[0], LastMove.WKingPos[1] };
            this.KingPos[true] =  new[] { LastMove.BKingPos[0], LastMove.BKingPos[1] };

            Side = !Side;
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
            NewBoard.Endgame();
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
                {false, new[] {this.Castling[false][0], this.Castling[false][1]}},
                {true, new[] {this.Castling[true][0], this.Castling[true][1]}}};

            Clone.EnpassantSquare = new[] {this.EnpassantSquare[0], this.EnpassantSquare[1]};
            Clone.Side = this.Side == true;
            
            Clone.MoveChain = this.MoveChain;
            Clone.Repetition = new Dictionary<int, int>(this.Repetition);
            Clone.PieceCounter = this.PieceCounter;

            Clone.PiecePositions[false] = new List<(int, int)>(this.PiecePositions[false]);
            Clone.PiecePositions[true] = new List<(int, int)>(this.PiecePositions[true]);
            
            Clone.KingPos[false] = new[] {this.KingPos[false][0],this.KingPos[false][1]};
            Clone.KingPos[true] = new[] {this.KingPos[true][0],this.KingPos[true][1]};
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
                        this.KingPos[color] = new[] {j,i};
                        return new[] {j,i};
                    }
                }
            }
            return new[] {8,8};
        }

        public List<(int,int)> GetPiecePositions(bool side)
        {
            if (this.PiecePositions[side].Count == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (this.board[i,j].Color == side && this.board[i,j].Role != PieceType.Empty)
                        {
                            this.PiecePositions[side].Add((j,i));
                        }
                    }
                }
            }
            return this.PiecePositions[side];
        }

        int[] LocalValue()
        {
            int White = 0;
            int Black = 0;
            for (int i = 0; i < PiecePositions[false].Count; i++)
            {
                (int, int) coords = PiecePositions[false][i];

                White += board[coords.Item2,coords.Item1].LocalValue;
            }
            for (int i = 0; i < PiecePositions[true].Count; i++)
            {
                (int, int) coords = PiecePositions[true][i];

                Black += board[coords.Item2,coords.Item1].LocalValue;
            }

            return new[] {White, Black};
        }

        bool PawnsLeft()
        {
            for (int i = 0; i < PiecePositions[false].Count; i++)
            {
                (int, int) coords = PiecePositions[false][i];

                if (board[coords.Item2,coords.Item1].Role == PieceType.Pawn)
                    return true;
            }
            for (int i = 0; i < PiecePositions[true].Count; i++)
            {
                (int, int) coords = PiecePositions[true][i];

                if (board[coords.Item2,coords.Item1].Role == PieceType.Pawn)
                    return true;
            }

            return false;
        }

        public bool Endgame()
        {
            if (PieceCounter == 0)
            {
                int Total = 0;
                
                for (int i = 0; i < PiecePositions[false].Count; i++)
                {
                    (int, int) coords = PiecePositions[false][i];

                    if (board[coords.Item2,coords.Item1].Role != PieceType.Pawn)
                        Total += board[coords.Item2,coords.Item1].LocalValue;
                }
                for (int i = 0; i < PiecePositions[true].Count; i++)
                {
                    (int, int) coords = PiecePositions[true][i];
                    if (board[coords.Item2,coords.Item1].Role != PieceType.Pawn)
                        Total += board[coords.Item2,coords.Item1].LocalValue;
                }
                this.PieceCounter = Total;
                
                Debug.Log("Pieces counted");
            }
            return PieceCounter <= 3200;
        }

        public (Outcome, List<Move.Move>) Status()
        {
            List<Move.Move> moveList = MoveFinder.Search(this, Side, true);
            if (this.DeclaredOutcome == Outcome.Ongoing)
            {
                if (moveList.Count == 0)
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
            return (DeclaredOutcome,  moveList);
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ZobristHash.Hash(this);
        }

        void AddSelf()
        {
            if (Repetition.ContainsKey(this.GetHashCode()))
            {
                Repetition[this.GetHashCode()]++;
            }
            else
            {
                Repetition[this.GetHashCode()] = 1;
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
        public static Board StartingBoard = Board.Constructor(StartingPosition, false, new[] {true,true}, new[] {true,true}, new[] {8,8}, 0);

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
                return new[] {FileIndex[square[0].ToString()], Int32.Parse(square[1].ToString()) - 1};
            } else {
                return new[] {Int32.Parse(square[1].ToString()) - 1, FileIndex[square[0].ToString()]};
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
    
    public class ReverseMove
    {
        public ((int, int),(int,int)) OriginMove;
        public ((int, int),(int,int)) ExtraMove;
        public Piece.Piece CapturedPiece;
        public bool Promotion;
        public (int, int) Enpassant;
        public int[] PrevEnpassant;
        public int MoveChain;
        public bool[] WhiteCastle;
        public bool[] BlackCastle;
        public int[] WKingPos;
        public int[] BKingPos;
        
        public ReverseMove(((int, int),(int,int)) originMove, ((int, int),(int,int)) extraMove, Piece.Piece capturedPiece, bool promotion, (int, int) enpassant, int[] prevEnpassant,  int moveChain, bool[] whiteCastle, bool[] blackCastle, int[] wKingPos, int[] bKingPos)
        {
            OriginMove = originMove;
            ExtraMove = extraMove;
            CapturedPiece = capturedPiece;
            Promotion = promotion;
            Enpassant = enpassant;
            MoveChain = moveChain;
            PrevEnpassant = prevEnpassant;
            WhiteCastle = whiteCastle;
            BlackCastle = blackCastle;
            WKingPos = wKingPos;
            BKingPos = bKingPos;
        }
    }
}
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

        public (int,int) EnpassantSquare = (8,8); // file, rank  8,8 for no en passant
        public bool Side;

        // 50 move rule
        private int MoveChain;
        private Dictionary<int, int> Repetition =  new Dictionary<int, int>();
        private Outcome DeclaredOutcome = Outcome.Ongoing;
        
        private Dictionary<bool, (int,int)> KingPos = new Dictionary<bool, (int,int)>{
            {true, (8,8)},
            {false, (8,8)},
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
            (int, int) MoveTo = (move.To.Item1,move.To.Item2);
            (int, int) MoveFrom = (move.From.Item1, move.From.Item2);
            Piece.Piece OriginPiece = this.board[move.From.Item2,move.From.Item1];
            Piece.Piece TargetPiece = this.board[move.To.Item2,move.To.Item1];
            bool OriginColor = OriginPiece.Color;
            bool TargetColor = TargetPiece.Color;
            
            // Local variables to store all the data that will be required to generate the ReverseMove
            ((int, int), (int, int)) extraMove = ((8, 8), (8, 8));
            (int, int) enpassant = (8,8);
            int moveChain = this.MoveChain;
            (int,int) prevEnpassant = this.EnpassantSquare;
            bool[] whiteCastle = this.Castling[false];
            bool[] blackCastle = this.Castling[true];
            (int,int) wKingPos =  this.KingPos[false];
            (int,int) bKingPos = this.KingPos[true];


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
                this.board[move.To.Item2,move.To.Item1] = move.Promotion;
            else
                this.board[move.To.Item2,move.To.Item1] = OriginPiece;
            
            this.board[move.From.Item2,move.From.Item1] = Empty;
            
            // castling
            if (move.From == Presets.WKStartPos && OriginPiece.Role == PieceType.King)
            {
                if (move.To == Presets.WKShortCastlePos && this.Castling[false][0])
                {
                    this.board[Presets.WRShortCastlePos.Item2,Presets.WRShortCastlePos.Item1] = Empty;
                    this.board[Presets.WRShortCastleDest.Item2,Presets.WRShortCastleDest.Item1] = W_Rook;
                    this.PiecePositions[false].Add(Presets.WRShortCastleDest);
                    this.PiecePositions[false].Remove(Presets.WRShortCastlePos);
                    this.Castling[false] = new[] {false, false};

                    extraMove = (Presets.WRShortCastlePos, Presets.WRShortCastleDest);
                } 
                else if (move.To == Presets.WKLongCastlePos && this.Castling[false][1])
                {
                    this.board[Presets.WRLongCastlePos.Item2,Presets.WRLongCastlePos.Item1] = Empty;
                    this.board[Presets.WRLongCastleDest.Item2,Presets.WRLongCastleDest.Item1] = W_Rook;
                    this.PiecePositions[false].Add(Presets.WRLongCastleDest);
                    this.PiecePositions[false].Remove(Presets.WRLongCastlePos);
                    this.Castling[false] = new[] {false, false};
                    
                    extraMove = (Presets.WRLongCastlePos, Presets.WRLongCastleDest);
                }
            } 
            else if (move.From == Presets.BKStartPos && OriginPiece.Role == PieceType.King)
            {
                if (move.To == Presets.BKShortCastlePos && this.Castling[true][0])
                {
                    this.board[Presets.BRShortCastlePos.Item2,Presets.BRShortCastlePos.Item1] = Empty;
                    this.board[Presets.BRShortCastleDest.Item2,Presets.BRShortCastleDest.Item1] = B_Rook;
                    this.PiecePositions[true].Add(Presets.BRShortCastleDest);
                    this.PiecePositions[true].Remove(Presets.BRShortCastlePos);
                    this.Castling[true] = new[] {false, false};
                    
                    extraMove = (Presets.BRShortCastlePos, Presets.BRShortCastleDest);
                } 
                else if (move.To == Presets.BKLongCastlePos && this.Castling[true][1])
                {
                    this.board[Presets.BRLongCastlePos.Item2,Presets.BRLongCastlePos.Item1] = Empty;
                    this.board[Presets.BRLongCastleDest.Item2,Presets.BRLongCastleDest.Item1] = B_Rook;
                    this.PiecePositions[true].Add(Presets.BRLongCastleDest);
                    this.PiecePositions[true].Remove(Presets.BRLongCastlePos);
                    this.Castling[true] = new[] {false, false};
                    
                    extraMove = (Presets.BRLongCastlePos, Presets.BRLongCastlePos);
                }
            }
            // Remove castling rights
            else if (move.To == Presets.WhiteRookHPos || move.From == Presets.WhiteRookHPos)
                this.Castling[false][0] = false;
            else if (move.To == Presets.WhiteRookAPos || move.From == Presets.WhiteRookAPos)
                this.Castling[false][1] = false;
            else if (move.To == Presets.BlackRookHPos || move.From == Presets.BlackRookHPos)
                this.Castling[true][0] = false;
            else if (move.To == Presets.BlackRookAPos || move.From == Presets.BlackRookAPos)
                this.Castling[true][1] = false;
            

            // en passant (Holy Hell!)
            if (OriginPiece.Role == PieceType.Pawn)
            {
                int RankDistance = move.From.Item2 - move.To.Item2;

                if (move.To == this.EnpassantSquare)
                {
                    if (this.board[move.To.Item2 + 1,move.To.Item1].Role == PieceType.Pawn)
                    {
                        this.PiecePositions[this.board[move.To.Item2 + 1,move.To.Item1].Color].Remove((move.To.Item1,move.To.Item2 + 1));
                        this.board[move.To.Item2 + 1,move.To.Item1] = Empty;
                        enpassant = (move.To.Item2 + 1,move.To.Item1);
                    }
                    else if (this.board[move.To.Item2 - 1,move.To.Item1].Role == PieceType.Pawn)
                    {
                        this.PiecePositions[this.board[move.To.Item2 - 1,move.To.Item1].Color].Remove((move.To.Item1,move.To.Item2 - 1));
                        this.board[move.To.Item2 - 1,move.To.Item1] = Empty;
                        enpassant = (move.To.Item2 - 1,move.To.Item1);
                    }
                }
                else if (RankDistance == 2 || RankDistance == -2) // if the pawn made 2 moves forward, set EnpassantSquare
                    this.EnpassantSquare = (move.From.Item1, move.To.Item2 + (RankDistance / 2));
                else
                    this.EnpassantSquare = (8,8);
                
            }
            else if (OriginPiece.Role == PieceType.King) // Changing KingPos
            {
                this.KingPos[OriginColor] = (move.To.Item1,move.To.Item2);
                this.Castling[OriginColor] = new[] {false, false};
                this.EnpassantSquare = (8,8);
            }
            else
                this.EnpassantSquare = (8,8);
            
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
            this.EnpassantSquare = LastMove.PrevEnpassant;

            this.Castling[false] = new[] { LastMove.WhiteCastle[0], LastMove.WhiteCastle[1] };
            this.Castling[true] = new[] { LastMove.BlackCastle[0], LastMove.BlackCastle[1] };

            this.KingPos[false] = LastMove.WKingPos;
            this.KingPos[true] =  LastMove.BKingPos;

            Side = !Side;
        }

        public static Board Constructor(Piece.Piece[,] board, bool side, bool[] whiteCastle, bool[] blackCastle, (int,int) enpassantSquare, int moveChain)
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
            
            Clone.board = (Piece.Piece[,])this.board.Clone();
            
            Clone.PiecePositions = new Dictionary<bool, List<(int, int)>>
            {
                {false, new List<(int, int)>(this.PiecePositions[false])},
                {true, new List<(int, int)>(this.PiecePositions[true])}
            };
            
            Clone.KingPos = new Dictionary<bool, (int,int)>
            {
                {false, this.KingPos[false]},
                {true, this.KingPos[true]}
            };
            
            Clone.EnpassantSquare = this.EnpassantSquare;
            Clone.Side = this.Side;
            Clone.MoveChain = this.MoveChain;
            Clone.Repetition = new Dictionary<int, int>(this.Repetition);
            Clone.PieceCounter = this.PieceCounter;
            Clone.Castling = new Dictionary<bool, bool[]> {
                {false, new[] {this.Castling[false][0], this.Castling[false][1]}},
                {true, new[] {this.Castling[true][0], this.Castling[true][1]}}};
            
            return Clone;
        }

        public bool KingInCheck(bool color)
        {
            return MoveFinder.Attacked(this, this.GetKingPos(color), !color);
        }

        private (int,int) GetKingPos(bool color)
        {
            if (this.KingPos[color].Item1 != 8)
            {
                return this.KingPos[color];
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (this.board[i,j].Role == PieceType.King && this.board[i,j].Color == color)
                    {
                        this.KingPos[color] = (j,i);
                        return (j,i);
                    }
                }
            }
            return (8,8);
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
        public static Board StartingBoard = Board.Constructor(StartingPosition, false, new[] {true,true}, new[] {true,true}, (8,8), 0);

        internal static (int,int) WKStartPos = (4,0); // file, rank

        // white short castle
        internal static (int,int) WKShortCastlePos = (6,0);
        internal static (int,int) WRShortCastlePos = (7,0);
        internal static (int,int) WRShortCastleDest = (5,0);

        // white long castle
        internal static (int,int) WKLongCastlePos = (2,0);
        internal static (int,int) WRLongCastlePos = (0,0);
        internal static (int,int) WRLongCastleDest = (3,0);

        internal static (int,int) BKStartPos = (4,7); // file, rank

        // black short castle
        internal static (int,int) BKShortCastlePos = (6,7);
        internal static (int,int) BRShortCastlePos = (7,7);
        internal static (int,int) BRShortCastleDest = (5,7);

        // black long castle
        internal static (int,int) BKLongCastlePos = (2,7);
        internal static (int,int) BRLongCastlePos = (0,7);
        internal static (int,int) BRLongCastleDest = (3,7);

        // rook positions
        internal static (int,int) WhiteRookAPos = (0,0);
        internal static (int,int) WhiteRookHPos = (7,0);
        internal static (int,int) BlackRookAPos = (0,7);
        internal static (int,int) BlackRookHPos = (7,7);

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
        public (int, int) PrevEnpassant;
        public int MoveChain;
        public bool[] WhiteCastle;
        public bool[] BlackCastle;
        public (int,int) WKingPos;
        public (int,int) BKingPos;
        
        public ReverseMove(((int, int),(int,int)) originMove, ((int, int),(int,int)) extraMove, Piece.Piece capturedPiece, bool promotion, (int, int) enpassant, (int, int) prevEnpassant,  int moveChain, bool[] whiteCastle, bool[] blackCastle, (int, int) wKingPos, (int, int) bKingPos)
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
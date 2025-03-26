using Piece;
using static Piece.Presets;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Board
{
    public class Board
    {
        public Piece.Piece[,] board = new Piece.Piece[8,8];
        public Dictionary<bool, bool[]> Castling = new() {
            {false, new [] {true, true}}, // Short, Long
            {true, new [] {true, true}},
        };

        public (int,int) EnpassantSquare = (8,8); // file, rank  8,8 for no en passant
        public bool Side;

        // 50 move rule
        private int MoveChain;
        private Dictionary<int, int> Repetition =  new();
        private Outcome DeclaredOutcome = Outcome.Ongoing;
        
        public Dictionary<bool, (int,int)> KingPos = new() {
            {true, (8,8)},
            {false, (8,8)},
        };

        private int PieceCounter;
        
        public readonly Dictionary<bool, ulong> SideBitboards = new()
        {
            {false, 0},
            {true, 0},
        };
        
        public bool MakeMove(Move.Move move)
        {
            // local variables storing data that is accessed over and over
            Piece.Piece OriginPiece = board[move.From.Item2,move.From.Item1];
            Piece.Piece TargetPiece = board[move.To.Item2,move.To.Item1];
            bool OriginColor = OriginPiece.Color;
            bool TargetColor = TargetPiece.Color;

            if (TargetPiece.Role != PieceType.Empty)
            {
                // take the piece off the other side's bitboards
                SideBitboards[TargetColor] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2, move.To.Item1];
                if (TargetPiece.Role != PieceType.Pawn)
                    PieceCounter -= TargetPiece.LocalValue;
            }
            
            // update the piece's bitboard
            SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[move.From.Item2, move.From.Item1];
            SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2, move.To.Item1];
            
            if (Side) 
                MoveChain++;
            else if (OriginPiece.Role == PieceType.Pawn || TargetPiece.Role != PieceType.Empty)
                MoveChain = 0;
            
            // change the pieces in the actual board representation
            if (move.Promotion != Empty)
            {
                board[move.To.Item2,move.To.Item1] = move.Promotion;
            }
            else
            {
                board[move.To.Item2,move.To.Item1] = OriginPiece;
            }
            
            board[move.From.Item2,move.From.Item1] = Empty;
            
            // castling
            if (move.From == Presets.WKStartPos && OriginPiece.Role == PieceType.King)
            {
                if (move.To == Presets.WKShortCastlePos && Castling[false][0])
                {
                    board[Presets.WRShortCastlePos.Item2,Presets.WRShortCastlePos.Item1] = Empty;
                    board[Presets.WRShortCastleDest.Item2,Presets.WRShortCastleDest.Item1] = W_Rook;
                    // update the bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRShortCastlePos.Item2, Presets.WRShortCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRShortCastleDest.Item2, Presets.WRShortCastleDest.Item1];
                    
                    Castling[false] = new[] {false, false};
                } 
                else if (move.To == Presets.WKLongCastlePos && Castling[false][1])
                {
                    board[Presets.WRLongCastlePos.Item2,Presets.WRLongCastlePos.Item1] = Empty;
                    board[Presets.WRLongCastleDest.Item2,Presets.WRLongCastleDest.Item1] = W_Rook;
                    // update bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRLongCastlePos.Item2, Presets.WRLongCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRLongCastleDest.Item2, Presets.WRLongCastleDest.Item1];
                    
                    Castling[false] = new[] {false, false};
                }
            } 
            else if (move.From == Presets.BKStartPos && OriginPiece.Role == PieceType.King)
            {
                if (move.To == Presets.BKShortCastlePos && Castling[true][0])
                {
                    board[Presets.BRShortCastlePos.Item2,Presets.BRShortCastlePos.Item1] = Empty;
                    board[Presets.BRShortCastleDest.Item2,Presets.BRShortCastleDest.Item1] = B_Rook;
                    // update bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRShortCastlePos.Item2, Presets.BRShortCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRShortCastleDest.Item2, Presets.BRShortCastleDest.Item1];
                    
                    Castling[true] = new[] {false, false};
                } 
                else if (move.To == Presets.BKLongCastlePos && Castling[true][1])
                {
                    board[Presets.BRLongCastlePos.Item2,Presets.BRLongCastlePos.Item1] = Empty;
                    board[Presets.BRLongCastleDest.Item2,Presets.BRLongCastleDest.Item1] = B_Rook;
                    // update bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRLongCastlePos.Item2, Presets.BRLongCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRLongCastleDest.Item2, Presets.BRLongCastleDest.Item1];
                    Castling[true] = new[] {false, false};
                }
            }
            // Remove castling rights
            else if (move.To == Presets.WhiteRookHPos || move.From == Presets.WhiteRookHPos)
                Castling[false][0] = false;
            else if (move.To == Presets.WhiteRookAPos || move.From == Presets.WhiteRookAPos)
                Castling[false][1] = false;
            else if (move.To == Presets.BlackRookHPos || move.From == Presets.BlackRookHPos)
                Castling[true][0] = false;
            else if (move.To == Presets.BlackRookAPos || move.From == Presets.BlackRookAPos)
                Castling[true][1] = false;
            

            // en passant (Holy Hell!)
            if (OriginPiece.Role == PieceType.Pawn)
            {
                int RankDistance = move.From.Item2 - move.To.Item2;

                if (move.To == EnpassantSquare)
                {
                    if (board[move.To.Item2 + 1,move.To.Item1].Role == PieceType.Pawn)
                    {
                        // update bitboard
                        SideBitboards[!OriginColor] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2 + 1,move.To.Item1];
                        
                        board[move.To.Item2 + 1,move.To.Item1] = Empty;
                    }
                    else if (board[move.To.Item2 - 1,move.To.Item1].Role == PieceType.Pawn)
                    {
                        // update bitboard
                        SideBitboards[!OriginColor] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2 - 1,move.To.Item1];
                        
                        board[move.To.Item2 - 1,move.To.Item1] = Empty;
                    }
                }
                else if (RankDistance == 2 || RankDistance == -2) // if the pawn made 2 moves forward, set EnpassantSquare
                    EnpassantSquare = (move.From.Item1, move.To.Item2 + (RankDistance / 2));
                else
                    EnpassantSquare = (8,8);
                
            }
            else if (OriginPiece.Role == PieceType.King) // Changing KingPos
            {
                KingPos[OriginColor] = (move.To.Item1,move.To.Item2);
                Castling[OriginColor] = new[] {false, false};
                EnpassantSquare = (8,8);
            }
            else
                EnpassantSquare = (8,8);
            
            Side = !Side;
            
            AddSelf();

            return true;
        }

        private static readonly Dictionary<bool, Piece.Piece> Pawns = new()
        {
            {false, W_Pawn},
            {true, B_Pawn}
        };

        public static Board Constructor(Piece.Piece[,] board, bool side, bool[] whiteCastle, bool[] blackCastle, (int,int) enpassantSquare, int moveChain)
        {
            Bitboards.Bitboards.Init();
            
            Board NewBoard = new Board();
            NewBoard.board = board;
            NewBoard.Castling = new Dictionary<bool, bool[]> {{false, blackCastle},{true, whiteCastle}};
            NewBoard.EnpassantSquare = enpassantSquare;
            NewBoard.Side = side;
            NewBoard.MoveChain = moveChain;
            NewBoard.KingPos[false] = NewBoard.GetKingPos(false);
            NewBoard.KingPos[true] = NewBoard.GetKingPos(true);
            NewBoard.Endgame();
            NewBoard.LocalValue();
            
            // fill out the bitboards
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Role != PieceType.Empty)
                    {
                        NewBoard.SideBitboards[board[i, j].Color] |= Bitboards.Bitboards.SquareBitboards[i, j];
                    }
                }
            }
            
            return NewBoard;
        }

        public Board DeepCopy()
        {
            Board Clone = new Board();
            
            Clone.board = (Piece.Piece[,])board.Clone();
            
            Clone.KingPos = new Dictionary<bool, (int,int)>
            {
                {false, KingPos[false]},
                {true, KingPos[true]}
            };
            
            Clone.EnpassantSquare = EnpassantSquare;
            Clone.Side = Side;
            Clone.MoveChain = MoveChain;
            Clone.Repetition = new Dictionary<int, int>(Repetition);
            Clone.PieceCounter = PieceCounter;
            Clone.Castling = new Dictionary<bool, bool[]>
            {
                {false, (bool[])Castling[false].Clone()},
                {true, (bool[])Castling[true].Clone()},
            };
            
            // copy bitboards over
            Clone.SideBitboards[false] = SideBitboards[false];
            Clone.SideBitboards[true] = SideBitboards[true];
            
            return Clone;
        }

        public bool KingInCheck(bool color)
        {
            return MoveFinder.Attacked(this, GetKingPos(color), !color);
        }

        public (int,int) GetKingPos(bool color)
        {
            // only loops through the boards when you don't already know the position of the king
            if (KingPos[color].Item1 != 8)
            {
                return KingPos[color];
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece.Piece TargetPiece = board[i, j];
                    if (TargetPiece.Role == PieceType.King && TargetPiece.Color == color)
                    {
                        KingPos[color] = (j,i);
                        return (j,i);
                    }
                }
            }
            return (8,8);
        }
        
        int[] LocalValue()
        {
            int White = 0;
            int Black = 0;

            ulong whitePieces = SideBitboards[false];
            ulong blackPieces = SideBitboards[true];
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((whitePieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                        White += board[i,j].LocalValue;
                    else if ((blackPieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                        Black += board[i,j].LocalValue;
                }
            }
            
            return new[] { White, Black };
        }

        bool PawnsLeft()
        {
            ulong whitePieces = SideBitboards[false];
            ulong blackPieces = SideBitboards[true];
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((whitePieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0 || (blackPieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                    {
                        if (board[i, j].Role == PieceType.Pawn)
                            return true;
                    }
                }
            }
            
            return false;
        }

        public bool Endgame()
        {
            if (PieceCounter == 0)
            {
                int Total = 0;
                
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (board[i,j].Role != PieceType.Pawn)
                            Total += board[i,j].LocalValue;
                    }
                }
                
                PieceCounter = Total;
                
                Debug.Log("Pieces counted");
            }
            return PieceCounter <= 3200;
        }

        public Outcome Status(bool complete)
        {
            if (complete) // looks for checkmates
            {
                List<Move.Move> moveList = MoveFinder.FilteredSearch(this, Side, false);
                
                if (DeclaredOutcome == Outcome.Ongoing)
                {
                    if (moveList.Count == 0)
                    {
                        if (KingInCheck(Side))
                        {
                            DeclaredOutcome = Presets.SideOutcomes[!Side];
                        }
                        else
                        {
                            DeclaredOutcome = Outcome.Draw;
                        }
                    }
                    else if (MoveChain > 49 || Repetition.ContainsValue(3))
                    {
                        DeclaredOutcome = Outcome.Draw;
                    }
                    else
                    {
                        int[] LocalValues = LocalValue();

                        if (LocalValues[0] < 1400 && LocalValues[1] < 1400 && !PawnsLeft())
                        {
                            DeclaredOutcome = Outcome.Draw;
                        }
                    }
                }
            }
            else // doesn't look for checkmates (doesn't need to search for moves, used in nodes)
            {
                if (DeclaredOutcome == Outcome.Ongoing)
                {
                    if (MoveChain > 49 || Repetition.ContainsValue(3))
                    {
                        DeclaredOutcome = Outcome.Draw;
                    }
                    else
                    {
                        int[] LocalValues = LocalValue();

                        if (LocalValues[0] < 1400 && LocalValues[1] < 1400 && !PawnsLeft())
                        {
                            DeclaredOutcome = Outcome.Draw;
                        }
                    }
                }
            }
            
            return DeclaredOutcome;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.ZobristHash.Hash(this);
        }

        void AddSelf()
        {
            int HashValue = GetHashCode();
            
            if (Repetition.ContainsKey(HashValue))
            {
                Repetition[HashValue]++;
            }
            else
            {
                Repetition.Add(HashValue, 1);
            }
        }
    }

    public static class Presets {
        public static Piece.Piece[,] StartingPosition =
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

    public static class TestCases
    {
        public static Piece.Piece[,] Castle =
        {
            {W_Rook, W_Knight, W_Bishop, W_Queen, W_King, Empty, Empty, W_Rook},
            {W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn},
            {B_Rook, B_Knight, B_Bishop, B_Queen, B_King, B_Bishop, B_Knight, B_Rook},
        };

        public static Board CastleCheck = Board.Constructor(Castle, false,new[] {true, true}, new[] {true, true}, (8,8), 2);    
    }
}
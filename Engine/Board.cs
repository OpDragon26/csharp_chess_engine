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
        
        private Dictionary<bool, (int,int)> KingPos = new() {
            {true, (8,8)},
            {false, (8,8)},
        };
        public Dictionary<bool, List<(int,int)>> PiecePositions = new() {
            {false, new List<(int,int)>()},
            {true, new List<(int,int)>()},
        };

        private int PieceCounter;

        private ReverseMove LastMove;
        
        public readonly Dictionary<bool, ulong> SideBitboards = new()
        {
            {false, 0},
            {true, 0},
        };

        public readonly Dictionary<bool, ulong[]> PieceBitboards = new()
        {
            {false, new ulong[6]},
            {true, new ulong[6]}
        };
        
        public bool MakeMove(Move.Move move, bool filter, bool generateReverse)
        {
            if (filter) // if the move isn't legal, don't make it
            {
                List<Move.Move> LegalMoves = MoveFinder.Search(this, Side, false);
                if (!move.InMovelist(LegalMoves))
                {
                    return false;
                }
            }
             
            // local variables storing data that is accessed over and over
            Piece.Piece OriginPiece = board[move.From.Item2,move.From.Item1];
            Piece.Piece TargetPiece = board[move.To.Item2,move.To.Item1];
            bool OriginColor = OriginPiece.Color;
            bool TargetColor = TargetPiece.Color;
            
            // Local variables to store all the data that will be required to generate the ReverseMove
            ((int, int), (int, int)) extraMove = ((8, 8), (8, 8));
            (int, int) enpassant = (8,8);
            int moveChain = MoveChain;
            (int,int) prevEnpassant = EnpassantSquare;
            bool[] whiteCastle = (bool[])Castling[false].Clone();
            bool[] blackCastle = (bool[])Castling[true].Clone();
            (int,int) wKingPos =  KingPos[false];
            (int,int) bKingPos = KingPos[true];
            ulong[] whiteBitboard = (ulong[])PieceBitboards[false].Clone();
            ulong[] blackBitboard = (ulong[])PieceBitboards[true].Clone();
            ulong sideBitboardWhite = SideBitboards[false];
            ulong sideBitboardBlack = SideBitboards[true];

            if (TargetPiece.Role != PieceType.Empty)
            {
                PiecePositions[TargetColor].Remove(move.To);
                // take the piece off the other side's bitboards
                SideBitboards[TargetColor] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2, move.To.Item1];
                PieceBitboards[TargetColor][(int)TargetPiece.Role] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2, move.To.Item1];
                if (TargetPiece.Role != PieceType.Pawn)
                    PieceCounter -= TargetPiece.LocalValue;
            }
            
            PiecePositions[OriginColor].Remove(move.From);
            PiecePositions[OriginColor].Add(move.To);
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
                // if the move was a promotion, remove from the pawn bitboard and add to the promotion piece's
                PieceBitboards[OriginColor][5] ^= Bitboards.Bitboards.SquareBitboards[move.From.Item2, move.From.Item1];
                PieceBitboards[OriginColor][(int)move.Promotion.Role] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2, move.To.Item1];
            }
            else
            {
                board[move.To.Item2,move.To.Item1] = OriginPiece;
                PieceBitboards[OriginColor][(int)OriginPiece.Role] ^= Bitboards.Bitboards.SquareBitboards[move.From.Item2, move.From.Item1];
                PieceBitboards[OriginColor][(int)OriginPiece.Role] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item2, move.To.Item1];
            }
            
            board[move.From.Item2,move.From.Item1] = Empty;
            
            // castling
            if (move.From == Presets.WKStartPos && OriginPiece.Role == PieceType.King)
            {
                if (move.To == Presets.WKShortCastlePos && Castling[false][0])
                {
                    board[Presets.WRShortCastlePos.Item2,Presets.WRShortCastlePos.Item1] = Empty;
                    board[Presets.WRShortCastleDest.Item2,Presets.WRShortCastleDest.Item1] = W_Rook;
                    PiecePositions[false].Add(Presets.WRShortCastleDest);
                    PiecePositions[false].Remove(Presets.WRShortCastlePos);
                    // update the bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRShortCastlePos.Item2, Presets.WRShortCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRShortCastleDest.Item2, Presets.WRShortCastleDest.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRShortCastlePos.Item2, Presets.WRShortCastlePos.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRShortCastleDest.Item2, Presets.WRShortCastleDest.Item1];
                    
                    Castling[false] = new[] {false, false};

                    extraMove = (Presets.WRShortCastlePos, Presets.WRShortCastleDest);
                } 
                else if (move.To == Presets.WKLongCastlePos && Castling[false][1])
                {
                    board[Presets.WRLongCastlePos.Item2,Presets.WRLongCastlePos.Item1] = Empty;
                    board[Presets.WRLongCastleDest.Item2,Presets.WRLongCastleDest.Item1] = W_Rook;
                    PiecePositions[false].Add(Presets.WRLongCastleDest);
                    PiecePositions[false].Remove(Presets.WRLongCastlePos);
                    // update bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRLongCastlePos.Item2, Presets.WRLongCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRLongCastleDest.Item2, Presets.WRLongCastleDest.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRLongCastlePos.Item2, Presets.WRLongCastlePos.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.WRLongCastleDest.Item2, Presets.WRLongCastleDest.Item1];
                    
                    Castling[false] = new[] {false, false};
                    
                    extraMove = (Presets.WRLongCastlePos, Presets.WRLongCastleDest);
                }
            } 
            else if (move.From == Presets.BKStartPos && OriginPiece.Role == PieceType.King)
            {
                if (move.To == Presets.BKShortCastlePos && Castling[true][0])
                {
                    board[Presets.BRShortCastlePos.Item2,Presets.BRShortCastlePos.Item1] = Empty;
                    board[Presets.BRShortCastleDest.Item2,Presets.BRShortCastleDest.Item1] = B_Rook;
                    PiecePositions[true].Add(Presets.BRShortCastleDest);
                    PiecePositions[true].Remove(Presets.BRShortCastlePos);
                    // update bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRShortCastlePos.Item2, Presets.BRShortCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRShortCastleDest.Item2, Presets.BRShortCastleDest.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRShortCastlePos.Item2, Presets.BRShortCastlePos.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRShortCastleDest.Item2, Presets.BRShortCastleDest.Item1];
                    Castling[true] = new[] {false, false};
                    
                    extraMove = (Presets.BRShortCastlePos, Presets.BRShortCastleDest);
                } 
                else if (move.To == Presets.BKLongCastlePos && Castling[true][1])
                {
                    board[Presets.BRLongCastlePos.Item2,Presets.BRLongCastlePos.Item1] = Empty;
                    board[Presets.BRLongCastleDest.Item2,Presets.BRLongCastleDest.Item1] = B_Rook;
                    PiecePositions[true].Add(Presets.BRLongCastleDest);
                    PiecePositions[true].Remove(Presets.BRLongCastlePos);
                    // update bitboards
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRLongCastlePos.Item2, Presets.BRLongCastlePos.Item1];
                    SideBitboards[OriginColor] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRLongCastleDest.Item2, Presets.BRLongCastleDest.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRLongCastlePos.Item2, Presets.BRLongCastlePos.Item1];
                    PieceBitboards[OriginColor][0] ^= Bitboards.Bitboards.SquareBitboards[Presets.BRLongCastleDest.Item2, Presets.BRLongCastleDest.Item1];
                    Castling[true] = new[] {false, false};
                    
                    extraMove = (Presets.BRLongCastlePos, Presets.BRLongCastlePos);
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
                        PiecePositions[board[move.To.Item2 + 1,move.To.Item1].Color].Remove((move.To.Item1,move.To.Item2 + 1));
                        // update bitboards
                        SideBitboards[board[move.To.Item2 + 1, move.To.Item1].Color] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item1, move.To.Item2 + 1];
                        PieceBitboards[board[move.To.Item2 + 1, move.To.Item1].Color][5] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item1, move.To.Item2 + 1];
                        
                        board[move.To.Item2 + 1,move.To.Item1] = Empty;
                        enpassant = (move.To.Item2 + 1,move.To.Item1);
                    }
                    else if (board[move.To.Item2 - 1,move.To.Item1].Role == PieceType.Pawn)
                    {
                        PiecePositions[board[move.To.Item2 - 1,move.To.Item1].Color].Remove((move.To.Item1,move.To.Item2 - 1));
                        // update bitboards
                        SideBitboards[board[move.To.Item2 - 1, move.To.Item1].Color] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item1, move.To.Item2 - 1];
                        PieceBitboards[board[move.To.Item2 + 1, move.To.Item1].Color][5] ^= Bitboards.Bitboards.SquareBitboards[move.To.Item1, move.To.Item2 - 1];
                        
                        board[move.To.Item2 - 1,move.To.Item1] = Empty;
                        enpassant = (move.To.Item2 - 1,move.To.Item1);
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
            
            if (generateReverse)
                LastMove = new ReverseMove((move.From, move.To), extraMove, TargetPiece, move.Promotion != Empty, enpassant, prevEnpassant, moveChain, whiteCastle, blackCastle, wKingPos, bKingPos, whiteBitboard, blackBitboard, sideBitboardWhite, sideBitboardBlack);
            
            Side = !Side;
            
            AddSelf();

            return true;
        }

        private static readonly Dictionary<bool, Piece.Piece> Pawns = new()
        {
            {false, W_Pawn},
            {true, B_Pawn}
        };
        public void UnmakeMove()
        {
            // Unmaking the original move
            if (LastMove.Promotion)
                board[LastMove.OriginMove.Item1.Item2, LastMove.OriginMove.Item1.Item1] = Pawns[!Side];
            else
                board[LastMove.OriginMove.Item1.Item2, LastMove.OriginMove.Item1.Item1] = board[LastMove.OriginMove.Item2.Item2, LastMove.OriginMove.Item2.Item1];
            
            // put back the moved pieces
            PiecePositions[!Side].Add(LastMove.OriginMove.Item1);
            
            board[LastMove.OriginMove.Item2.Item2, LastMove.OriginMove.Item2.Item1] = LastMove.CapturedPiece;
            PiecePositions[Side].Remove(LastMove.OriginMove.Item2);
            
            if (LastMove.CapturedPiece.Role != PieceType.Pawn)
                PieceCounter += LastMove.CapturedPiece.LocalValue;
            
            // unmaking the extra move, if there is one
            if (LastMove.ExtraMove.Item1.Item1 != 8)
            {
                board[LastMove.ExtraMove.Item1.Item2, LastMove.ExtraMove.Item1.Item1] = board[LastMove.ExtraMove.Item2.Item2, LastMove.ExtraMove.Item2.Item1];
                board[LastMove.ExtraMove.Item2.Item2, LastMove.ExtraMove.Item2.Item1] = Empty;
                
                PiecePositions[!Side].Add(LastMove.ExtraMove.Item1);
                PiecePositions[!Side].Remove(LastMove.ExtraMove.Item2);
            }
            
            // if there was an en passant capture, put the pawn back
            if (LastMove.Enpassant.Item1 != 8)
            {
                board[LastMove.Enpassant.Item2, LastMove.Enpassant.Item1] = Pawns[Side];
                PiecePositions[Side].Add(LastMove.Enpassant);
            }
            
            MoveChain = LastMove.MoveChain;
            EnpassantSquare = LastMove.PrevEnpassant;

            Castling[false] = new[] { LastMove.WhiteCastle[0], LastMove.WhiteCastle[1] };
            Castling[true] = new[] { LastMove.BlackCastle[0], LastMove.BlackCastle[1] };

            KingPos[false] = LastMove.WKingPos;
            KingPos[true] =  LastMove.BKingPos;

            SideBitboards[false] = LastMove.SideBitboardWhite;
            SideBitboards[true] = LastMove.SideBitboardBlack;

            PieceBitboards[false] = LastMove.BitboardWhite;
            PieceBitboards[true] = LastMove.BitboardBlack;
            
            Side = !Side;
        }

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
            NewBoard.PiecePositions[false] = NewBoard.GetPiecePositions(false);
            NewBoard.PiecePositions[true] = NewBoard.GetPiecePositions(true);
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
                        
                        NewBoard.PieceBitboards[board[i, j].Color][(int)board[i, j].Role] |= Bitboards.Bitboards.SquareBitboards[i, j];
                    }
                    
                }
            }
            
            return NewBoard;
        }

        public Board DeepCopy()
        {
            Board Clone = new Board();
            
            Clone.board = (Piece.Piece[,])board.Clone();
            
            Clone.PiecePositions = new Dictionary<bool, List<(int, int)>>
            {
                {false, new List<(int, int)>(PiecePositions[false])},
                {true, new List<(int, int)>(PiecePositions[true])}
            };
            
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
                {true, (bool[])Castling[true].Clone()}
            };
            
            // copy bitbooards over
            Clone.SideBitboards[false] = SideBitboards[false];
            Clone.SideBitboards[true] = SideBitboards[true];
            Clone.PieceBitboards[false] = (ulong[])PieceBitboards[false].Clone();
            Clone.PieceBitboards[true] = (ulong[])PieceBitboards[true].Clone();
            
            return Clone;
        }

        public bool KingInCheck(bool color)
        {
            return MoveFinder.Attacked(this, GetKingPos(color), !color);
        }

        private (int,int) GetKingPos(bool color)
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

        public List<(int,int)> GetPiecePositions(bool side)
        {
            // initialize PiecePositions if it's empty
            if (PiecePositions[side].Count == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Piece.Piece TargetPiece = board[i, j];
                        if (TargetPiece.Color == side && TargetPiece.Role != PieceType.Empty)
                        {
                            PiecePositions[side].Add((j,i));
                        }
                    }
                }
            }
            return PiecePositions[side];
        }

        int[] LocalValue()
        {
            int White = 0;
            int Black = 0;
            
            List<(int,int)> Positions = PiecePositions[false];
            int l = Positions.Count;
            
            for (int i = 0; i < l; i++)
            {
                (int, int) coords = Positions[i];

                White += board[coords.Item2,coords.Item1].LocalValue;
            }
            
            Positions = PiecePositions[true];
            l  = Positions.Count;
            
            for (int i = 0; i < l; i++)
            {
                (int, int) coords = Positions[i];

                Black += board[coords.Item2,coords.Item1].LocalValue;
            }
            return new[] { White, Black };
        }

        bool PawnsLeft()
        {
            List<(int,int)> Positions = PiecePositions[false];
            int l = Positions.Count;
            
            for (int i = 0; i < l; i++)
            {
                (int, int) coords = Positions[i];

                if (board[coords.Item2,coords.Item1].Role == PieceType.Pawn)
                    return true;
            }
            
            Positions = PiecePositions[true];
            l  = Positions.Count;
            for (int i = 0; i < l; i++)
            {
                (int, int) coords = Positions[i];

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
                PieceCounter = Total;
                
                Debug.Log("Pieces counted");
            }
            return PieceCounter <= 3200;
        }

        public (Outcome, List<Move.Move>) Status()
        {
            List<Move.Move> moveList = MoveFinder.Search(this, Side, true);
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
            return (DeclaredOutcome,  moveList);
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
        public ulong SideBitboardWhite;
        public ulong SideBitboardBlack;
        public ulong[] BitboardWhite;
        public ulong[] BitboardBlack;
        
        public ReverseMove(((int, int),(int,int)) originMove, ((int, int),(int,int)) extraMove, Piece.Piece capturedPiece, bool promotion, (int, int) enpassant, (int, int) prevEnpassant,  int moveChain, bool[] whiteCastle, bool[] blackCastle, (int, int) wKingPos, (int, int) bKingPos, ulong[] bitboardWhite, ulong[] bitboardBlack, ulong sideBitboardWhite, ulong sideBitboardBlack)
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
            BitboardWhite = bitboardWhite;
            BitboardBlack = bitboardBlack;
            SideBitboardWhite = sideBitboardWhite;
            SideBitboardBlack = sideBitboardBlack;
        }
    }

    public static class TestCases
    {
    }
}
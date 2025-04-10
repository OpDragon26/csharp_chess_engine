using System;
using System.Collections.Generic;
using Board;
using Piece;
using UnityEditor;
using UnityEngine;
using static MagicNumbers.MagicNumbers;
using Presets = Piece.Presets;

// Todo:
// Create a bitboard for the legal moves based on a bitboard

namespace Bitboards
{
    public static class Bitboards
    {
        public static readonly ulong[,] SquareBitboards = new ulong[8, 8];
        private static readonly ulong Square = 0x8000000000000000;
        private static bool initialized;
        
        // rooks
        public static readonly ulong[,] RookMask = new ulong[8, 8];
        private static readonly ulong rank = 0xFF00000000000000;
        private static readonly ulong file = 0x8080808080808080;
        public static readonly ulong[,][] RookBlockerCombinations = new ulong[8, 8][];
        public static readonly ulong[,][] RookMoves = new ulong[8, 8][];
        //public static Dictionary<((int,int), ulong), ulong> RookDict = new();
        public static readonly ulong[,][] RookLookup = new ulong[8, 8][];
        
        // bishops
        public static readonly ulong[,] BishopMask = new ulong[8, 8];
        private static readonly ulong UpDiagonal = 0x102040810204080;
        private static readonly ulong DownDiagonal = 0x8040201008040201;
        public static readonly ulong[,][] BishopBlockerCombinations = new ulong[8, 8][];
        public static readonly ulong[,][] BishopMoves = new ulong[8, 8][];
        //public static Dictionary<((int,int), ulong), ulong> BishopDict = new();
        public static readonly ulong[,][] BishopLookup = new ulong[8, 8][];
        
        // kings & knights
        public static readonly ulong[,] KingMask = new ulong[8, 8];
        public static readonly ulong[,] KnightMask = new ulong[8, 8];

        // Pawns
        public static readonly ulong[,] WhitePawnMask = new ulong[8,8];
        public static readonly ulong[,] WhitePawnCaptureMask = new ulong[8,8];
        public static readonly ulong[,] BlackPawnMask = new ulong[8,8];
        public static readonly ulong[,] BlackPawnCaptureMask = new ulong[8,8];
        private static readonly ulong WDoubleMove = 0x808000000000;
        private static readonly ulong BDoubleMove = 0x80800000;
        private static readonly ulong WSingleMove = 0x8000000000;
        private static readonly ulong BSingleMove = 0x80000000;


        public static void Init()
        {
            if (!initialized)
            {
                ulong init = 0x8000000000000000; // The first square

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        SquareBitboards[i, j] = init;
                        init >>= 1; // shift it to the right for the next square
                        
                        // create rook masks
                        ulong currentFile = file >> i;
                        ulong currentRank = rank >> (j * 8);
                        ulong rookMask = currentFile ^ currentRank;

                        if (i != 0) // if the coordinate is not on the edge, then subtract the final ranks and files, because those won't need to be checked for blockers
                            rookMask &= ~file;
                        if (i != 7)
                            rookMask &= ~(file >> 7);
                        if (j != 0)
                            rookMask &= ~(rank);
                        if (j != 7)
                            rookMask &= ~(rank >> 56);
                        
                        // Add the mask to the array
                        RookMask[i, j] = rookMask;
                        // Debug.Log(Convert.ToString((long)RookMoveMask[i, j], 2).PadLeft(64, '0'));
                        
                        // create bishop masks
                        ulong relativeUD = UpDiagonal;
                        ulong relativeDD = DownDiagonal;
                        
                        int udShift = (i + j - 7) * 8;
                        int ddShift = (j - i) * 8;
                        
                        if (udShift < 0)
                            relativeUD <<= -udShift;
                        else
                            relativeUD >>= udShift;
                        
                        if (ddShift < 0)
                            relativeDD <<= -ddShift;
                        else
                            relativeDD >>= ddShift;
                        
                        ulong bishopMask = relativeUD ^ relativeDD;
                        
                        bishopMask &= ~file;
                        bishopMask &= ~(file >> 7);
                        bishopMask &= ~(rank);
                        bishopMask &= ~(rank >> 56);
                        
                        // add the mask to the array
                        BishopMask[i, j] = bishopMask;
                        
                        // create king masks
                        ulong kingMask = ulong.MaxValue;

                        
                        for (int k = 0; k < 8; k++)
                        {
                            if (!(k == j || k == j - 1 || k == j + 1))
                            {
                                kingMask &= ~(file >> k);
                            }
                            
                            if (!(k == i || k == i - 1 || k == i + 1))
                            {
                                kingMask &= ~(rank >> (k * 8));
                            }
                        }
                        
                        kingMask &= ~SquareBitboards[i, j];
                        
                        KingMask[i, j] = kingMask;
                        
                        // knights
                        ulong knightMask = UInt64.MaxValue;

                        for (int k = 0; k < 8; k++)
                        {
                            if (k > j + 2 || k < j - 2 || k == j)
                            {
                                knightMask &= ~(file >> k);
                            }
                            
                            if (k > i + 2 || k < i - 2 || k == i)
                            {
                                knightMask &= ~(rank >> (k * 8));
                            }
                        }

                        knightMask &= ~kingMask;

                        for (int k = -1; k < 2; k += 2)
                        {
                            for (int h = -1; h < 2; h += 2)
                            {
                                try
                                {
                                    knightMask &= ~((file >> (j + h * 2)) & (rank >> ((i + k * 2) * 8)));
                                } catch {}
                            }
                        }
                        
                        KnightMask[i, j] = knightMask;

                        // pawns

                        
                        if (i == 0 || i == 7)
                        {
                            // if pawns cannot appear on the given square, set the bitboard to 0
                            WhitePawnMask[i, j] = 0;
                            WhitePawnCaptureMask[i, j] = 0;
                            BlackPawnMask[i, j] = 0;
                            BlackPawnCaptureMask[i, j] = 0;
                        }
                        else
                        {
                            // add captures
                            ulong wCaptureMask = rank >> (8 * i + 8);
                            ulong bCaptureMask = rank >> (8 * i - 8);
                            for (int k = 0; k < 8; k++)
                            {
                                if (!(k == j - 1 || k == j + 1))
                                {
                                    wCaptureMask &= ~(file >> k);
                                    bCaptureMask &= ~(file >> k);
                                }
                            }

                            WhitePawnCaptureMask[i, j] = wCaptureMask;
                            BlackPawnCaptureMask[i, j] = bCaptureMask;
                            
                            // add singular forward moves
                            
                            // white
                            ulong wPawnMask = 0;
                            if (i == 1)
                                wPawnMask |= WDoubleMove >> j;
                            else
                                wPawnMask |= WSingleMove >> (j + (i - 2) * 8);
                            WhitePawnMask[i, j] = wPawnMask;
                            
                            // black
                            ulong bPawnMask = 0;
                            if (i == 6)
                                bPawnMask |= BDoubleMove >> j;
                            else
                                bPawnMask |= (BSingleMove >> j) << (40 - i * 8);
                            BlackPawnMask[i, j] = bPawnMask;
                        }

                        
                    }
                }

                // Get all the possible blocker combinations for a bitboard, find the legal moves from there
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        // rooks
                        RookBlockerCombinations[i, j] = BlockerCombinations(RookMask[i, j]);
                        RookMoves[i,j] = GetMoves(RookBlockerCombinations[i, j], (i,j), PieceType.Rook);
                        /*
                        int l = RookBlockerCombinations[i, j].Length;
                        for (int k = 0; k < l; k++)
                        {
                            RookDict.Add(((i, j), RookBlockerCombinations[i, j][k]), RookMoves[i,j][k]);
                        }
                        */
                        
                        
                        // bishops
                        BishopBlockerCombinations[i, j] = BlockerCombinations(BishopMask[i, j]);
                        BishopMoves[i, j] = GetMoves(BishopBlockerCombinations[i, j], (i,j), PieceType.Bishop);
                        /*
                        l = BishopBlockerCombinations[i, j].Length;
                        for (int k = 0; k < l; k++)
                        {
                            BishopDict.Add(((i, j), BishopBlockerCombinations[i, j][k]), BishopMoves[i,j][k]);
                        }
                        */
                    }
                }
                
                // generate magic numbers
                StaticInitMagicNumbers();
                //InitMagicNumbers(PieceType.Rook)
                //InitMagicNumbers(PieceType.Bishop);
                
                // init lookup tables
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        RookLookup[i, j] = new ulong[RookNumbers[i,j].highest + 1];
                        BishopLookup[i, j] = new ulong[BishopNumbers[i,j].highest + 1];
                    }
                }
                
                // fill up the lookup tables using the magic numbers
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        // rooks
                        for (int k = 0; k < RookBlockerCombinations[i, j].Length; k++)
                        {
                            RookLookup[i, j][(RookBlockerCombinations[i, j][k] * RookNumbers[i, j].number) >> RookNumbers[i, j].push] = RookMoves[i, j][k];
                        }
                        
                        // bishops
                        for (int k = 0; k < BishopBlockerCombinations[i, j].Length; k++)
                        {
                            BishopLookup[i, j][(BishopBlockerCombinations[i, j][k] * BishopNumbers[i, j].number) >> BishopNumbers[i, j].push] = BishopMoves[i, j][k];
                        }
                    }
                }

                initialized = true;
            }
        }

        public static ulong GetSquareBitboard(int file, int rank)
        {
            return Square >> (file + rank * 8);
        }

        private static ulong[] BlockerCombinations(ulong blockerMask)
        {
            // count how many on bits are there in the blockerMask, that's going to give us the amount of combinations
            List<int> indices = new List<int>();
            int l = 0;
            for (int i = 0; i < 64; i++)
            {
                if (((blockerMask << 63 - i) >> 63) != 0)
                {
                    l++;
                    indices.Add(i);
                }
            }
            ulong[] combinations = new ulong[(int)Math.Pow(2, l)];
            
            // for each combination
            for (ulong i = 0; i < (ulong)combinations.Length; i++)
            {
                ulong combination = 0;
                
                // for each index in the mask, push the bits of the combination to the right indices 
                for (int j = 0; j < l; j++)
                {
                    combination ^= ((i << 63 - j) >> 63) << indices[j];
                }
                
                combinations[i] = combination;
            }

            return combinations;
        }

        // get the squares a ray piece can move to in a certain blocker combination using piece patterns
        private static ulong[] GetMoves(ulong[] blockers, (int,int) pos, PieceType piece)
        {
            ulong[] moves = new ulong[blockers.Length];
            Pattern PiecePattern = Patterns.PiecePatterns[piece];

            if (PiecePattern.Repeat)
            {
                for (int k = 0; k < blockers.Length; k++)
                {
                    ulong blocker = blockers[k];
                    ulong move = 0;
                
                    // calculate the distance to each side
                    int[] iterators = PiecePattern.Iterator.GetIterators(pos);

                    for (int l = 0; l < 4; l++)
                    {
                        int it = iterators[l];
                        (int,int) Pattern = PiecePattern.MovePattern[l];

                        for (int h = 0; h < it; h++)
                        {
                            (int,int) Target = (pos.Item1 + Pattern.Item1 * (h + 1),  pos.Item2 + Pattern.Item2 * (h + 1));
                            move |= SquareBitboards[Target.Item2, Target.Item1]; // add the given square to the bitboard

                            if ((SquareBitboards[Target.Item2, Target.Item1] & blocker) != 0) // if the square isn't empty
                                break;
                        }
                    }
                    moves[k] = move;
                }
            }
            return moves;
        }
    }
}
using UnityEngine;
using System;
using System.Collections.Generic;
using NUnit.Framework;

// Todo:
// count the on bits in a capture mask and generate every int 

namespace Bitboards
{
    public static class Bitboards
    {
        public static ulong[,] SquareBitboards = new ulong[8, 8];
        public static bool initialized;
        
        // rooks
        public static ulong[,] RookMask = new ulong[8, 8];
        public static ulong rank = 0xFF00000000000000;
        public static ulong file = 0x8080808080808080;
        public static ulong[,][] RookBlockerCombinations = new ulong[8, 8][];
        
        // bishops
        public static ulong[,] BishopMask = new ulong[8, 8];
        public static ulong UpDiagonal = 0x102040810204080;
        public static ulong DownDiagonal = 0x8040201008040201;
        public static ulong[,][] BishopBlockerCombinations = new ulong[8, 8][];


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
                    }
                }
                
                // Get all the possible blocker combinations for a bitboard, find the legal moves from there, and add them to the dict
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        RookBlockerCombinations[i, j] = blockerCombinations(RookMask[i, j]);
                        BishopBlockerCombinations[i, j] = blockerCombinations(BishopMask[i, j]);
                    }
                }
            
                initialized = true;
            }
        }

        public static ulong[] blockerCombinations(ulong blockerMask)
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
                
                for (int j = 0; j < l; j++)
                {
                    combination ^= ((i << 63 - j) >> 63) << indices[j];
                }
                
                combinations[i] = combination;
            }

            return combinations;
        }
    }
}
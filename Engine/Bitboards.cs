using UnityEngine;
using System;

// Todo:
// cut off the edge of rook 

namespace Bitboards
{
    public static class Bitboards
    {
        public static ulong[,] SquareBitboards = new ulong[8, 8];
        public static ulong[,] RookMoveMask = new ulong[8, 8];
        public static ulong[,] RookBlockerMask = new ulong[8, 8];
        public static ulong rank = 0xFF00000000000000;
        public static ulong file = 0x8080808080808080;
        public static bool initialized;

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
                        ulong mask = currentFile ^ currentRank;
                        ulong blockerMask = mask;

                        if (i != 0) // if the coordinate is not on the edge, then subtract the final ranks and files, because those won't need to be checked for blockers
                            blockerMask &= ~file;
                        if (i != 7)
                            blockerMask &= ~(file >> 7);
                        if (j != 0)
                            blockerMask &= ~(rank);
                        if (j != 7)
                            blockerMask &= ~(rank >> 56);
                            
                        
                        // Add the mask to the array
                        RookMoveMask[i, j] = mask;
                        RookBlockerMask[i, j] = blockerMask;
                        // Debug.Log(Convert.ToString((long)RookMoveMask[i, j], 2).PadLeft(64, '0'));
                    }
                }
                
                
            
                initialized = true;
            }
        }
    }
}
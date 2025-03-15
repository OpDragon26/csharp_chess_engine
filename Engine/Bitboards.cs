using UnityEngine;
using System;

namespace Bitboards
{
    public static class Bitboards
    {
        public static ulong[,] SquareBitboards = new ulong[8, 8];
        public static ulong[,] RookMoveMask = new ulong[8, 8];
        private static ulong rank = 0xFF00000000000000;
        private static ulong file = 0x8080808080808080;

        public static void Init()
        {
            ulong init = 0x8000000000000000; // The first square

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    SquareBitboards[i, j] = init;
                    init >>= 1; // shift it to the right for the next square
                    
                    // create rook masks
                    RookMoveMask[i, j] = (file >> i) ^ (rank >> j * 8);
                    //Debug.Log(Convert.ToString((long)RookMoveMask[i, j], 2).PadLeft(64, '0'));
                }
            }
            
            
        }
    }
}
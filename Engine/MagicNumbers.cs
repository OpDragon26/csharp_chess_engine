using System.Collections.Generic;
using static Bitboards.Bitboards;
using System;
using System.Linq;
using Piece;

namespace MagicNumbers
{
    public static class MagicNumberGenerator
    {
        private static readonly List<ulong> UsedNumbers = new();
        private static readonly Random RandGen = new();

        public static (ulong number, int push, ulong highest) FindMagicNumber(ulong[] combinations)
        {
            ulong magicNumber = 0;
            int push = 0;
            
            ulong[] results = new ulong[combinations.Length];
            
            while (true) // keep generating magic numbers until one is found
            {
                // generate random ulong
                ulong candidateNumber = RandomUlong();
                
                // multiply every combination with the magic number and push them right by 48, only leaving the leftmost 16 bits
                for (int i = 0; i < combinations.Length; i++)
                {
                    unchecked
                    {
                        results[i] = (combinations[i] * candidateNumber) >> 48;
                    }
                }

                // if the result array contains duplicates, the number isn't magic, so don't bother checking it for further pushes
                if (results.GroupBy(x => x).Any(g => g.Count() > 1)) 
                    continue;
                
                // this part only happens if the number was magic, to some extent
                ulong[] temp = (ulong[])results.Clone();
                
                for (int i = 1; i < 16; i++)
                {
                    // push further right by a certain amount, and check for duplicates again
                    for (int j = 0; j < temp.Length; j++)
                    {
                        temp[j] >>= 1;
                    }
                    
                    // if there are duplicates, break the loop
                    if (results.GroupBy(x => x).Any(g => g.Count() > 1)) 
                        break;
                    
                    // if there are no duplicates, push results too
                    for (int j = 0; j < results.Length; j++)
                    {
                        results[j] >>= 1;
                    }

                    push++;
                }
                break;
            }
            
            return (magicNumber, push, results.Max());
        }
        
        private static ulong RandomUlong()
        {
            while (true)
            {
                var buffer = new byte[sizeof(ulong)];
                RandGen.NextBytes(buffer);
                ulong result = BitConverter.ToUInt64(buffer, 0);
                if (!UsedNumbers.Contains(result))
                {
                    UsedNumbers.Add(result);
                    return result;
                }
            }
        }
    }

    public static class MagicNumbers
    {
        private static readonly (ulong number, int push, ulong highest)[,] RookNumbers = new (ulong number, int push, ulong highest)[8,8];
        private static readonly (ulong number, int push, ulong highest)[,] BishopNumbers = new (ulong number, int push, ulong highest)[8,8];
        private static int found = 0;

        public static string GetNumString(PieceType piece)
        {
            string result = "";
            
            for (int i = 0; i < 8; i++)
            {
                string temp = "{";
                
                for (int j = 0; j < 8; j++)
                {
                    if (piece == PieceType.Rook)
                        temp += $"({RookNumbers[i,j].number}, {RookNumbers[i,j].push}, {RookNumbers[i,j].highest})";
                    else
                        temp += $"({BishopNumbers[i,j].number}, {BishopNumbers[i,j].push}, {BishopNumbers[i,j].highest})";
                }
                
                result += temp + "},\n";
            }
            
            return result;
        }

        public static void InitMagicNumbers(PieceType piece)
        {
            if (piece == PieceType.Rook)
            {
                // loop through the bitboards and find a magic number for each blocker combination
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        RookNumbers[i,j] = MagicNumberGenerator.FindMagicNumber(RookBlockerCombinations[i,j]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        BishopNumbers[i,j] = MagicNumberGenerator.FindMagicNumber(BishopBlockerCombinations[i,j]);
                    }
                }
            }
        }
    }
}
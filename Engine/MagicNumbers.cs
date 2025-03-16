using System.Collections.Generic;
using static Bitboards.Bitboards;
using System;
using System.Linq;
using Piece;
using UnityEngine;
using Random = System.Random;

namespace MagicNumbers
{
    public static class MagicNumberGenerator
    {
        private static readonly List<ulong> UsedNumbers = new();
        public static Random RandGen = new();

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
                magicNumber += candidateNumber;
                break;
            }
            
            return (magicNumber, push + 48, results.Max());
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
        public static (ulong number, int push, ulong highest)[,] RookNumbers = new (ulong number, int push, ulong highest)[8,8];
        public static (ulong number, int push, ulong highest)[,] BishopNumbers = new (ulong number, int push, ulong highest)[8,8];

        public static string GetNumString(PieceType piece)
        {
            string result = "";
            
            for (int i = 0; i < 8; i++)
            {
                string temp = "{";
                
                for (int j = 0; j < 8; j++)
                {
                    if (piece == PieceType.Rook)
                        temp += $"({RookNumbers[i,j].number}, {RookNumbers[i,j].push}, {RookNumbers[i,j].highest}), ";
                    else
                        temp += $"({BishopNumbers[i,j].number}, {BishopNumbers[i,j].push}, {BishopNumbers[i,j].highest}), ";
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

        public static void UpdateMagicNumbers(PieceType piece)
        {
            if (piece == PieceType.Rook)
            {
                // loop through the bitboards and find a magic number for each blocker combination
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        (ulong number, int push, ulong highest) magicNumber = MagicNumberGenerator.FindMagicNumber(RookBlockerCombinations[i,j]);
                        
                        // if the new magic number has a higher push, or has equal push and a lower highest number, replace the old magic number for the square
                        if (magicNumber.push > RookNumbers[i,j].push || (magicNumber.push == RookNumbers[i,j].push && magicNumber.highest < RookNumbers[i,j].highest))
                            RookNumbers[i,j] = magicNumber;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        (ulong number, int push, ulong highest) magicNumber = MagicNumberGenerator.FindMagicNumber(BishopBlockerCombinations[i,j]);
                        
                        // if the new magic number has a higher push, or has equal push and a lower highest number, replace the old magic number for the square
                        if (magicNumber.push > BishopNumbers[i,j].push || (magicNumber.push == BishopNumbers[i,j].push && magicNumber.highest < BishopNumbers[i,j].highest))
                            BishopNumbers[i,j] = magicNumber;
                    }
                }
            }
        }

        public static void StaticInitMagicNumbers()
        {
            RookNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(15829607625327214870, 52, 4094), (4943192134468692017, 52, 4084), (4614407218929778432, 53, 2046), (13905264217074003536, 53, 2047), (16532385665966414190, 52, 4086), (15622625673169615, 52, 4088), (498667501499538895, 52, 4086), (5210770577686438134, 51, 8188), },
{(4404872916745009564, 53, 2047), (7016999167631441365, 53, 2031), (16382456373204105771, 53, 2040), (14497100265737407979, 54, 1022), (6557608497692514259, 53, 2038), (9461021744341485680, 54, 1023), (5332287042964127471, 53, 2018), (2142726850214255809, 52, 4092), },
{(17579710242836413322, 52, 4092), (4914640414277663728, 53, 2045), (11624341299529351268, 53, 2042), (6397556463916437587, 53, 2045), (7072552107317371734, 53, 2046), (12704750331362130087, 53, 2042), (5245054459791313141, 53, 2046), (5352534400942252809, 52, 4095), },
{(6855945381461403090, 53, 2046), (15683736357380947169, 53, 2036), (15236360471669999607, 53, 2046), (8205902358082930231, 53, 2046), (17982517103730093027, 53, 2044), (5761390343781403411, 53, 2045), (10676247660651227581, 53, 2046), (11793826830343745368, 52, 4094), },
{(18143999588309568702, 52, 4087), (4551613396546142117, 53, 2044), (6780319512922615638, 53, 2047), (4570356780527125242, 53, 2047), (9728863186323380786, 53, 2043), (5707048160363038178, 53, 2046), (16687989911157624208, 53, 2046), (8048309692492221699, 52, 4094), },
{(7690337166431114430, 52, 4091), (7260957346753163516, 53, 2038), (8723510095718249088, 53, 2038), (153092547872521936, 53, 2044), (3912417857599742678, 53, 2045), (13964262478792069599, 53, 2044), (1193055295480231183, 53, 2046), (7709366747233837605, 52, 4094), },
{(14467361505110384782, 53, 2046), (4475802879202700209, 53, 2034), (5810498917017440274, 53, 2044), (10943054360392156382, 53, 2022), (14919737912605148824, 53, 2000), (260417633451450314, 53, 2037), (10068216542861203405, 53, 2039), (13028286460316777534, 52, 4090), },
{(13150763306886159670, 52, 4094), (7542415078686602792, 52, 4086), (10362141718680749007, 52, 4087), (1796265078396222995, 52, 4091), (813870412028688823, 52, 4084), (432012726176964750, 52, 4088), (7738098262021658948, 52, 4091), (10459891628325111114, 51, 8183), },
            };

            BishopNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(15055545398617211996, 58, 61), (16008890540794078199, 59, 27), (2550843379006096779, 59, 29), (18064541187957453211, 59, 29), (1259355262064679059, 59, 28), (2640117355223469213, 59, 29), (3316338458865620337, 59, 27), (314562119677057766, 58, 61), },
{(2299377724739934298, 59, 28), (14556677991727540342, 59, 29), (45287947871983690, 59, 29), (3923043181303719822, 59, 28), (6988904356194255137, 59, 28), (11678830763965903159, 60, 15), (13711351539345891325, 59, 28), (18345862439310834116, 59, 27), },
{(9901592624687817246, 59, 28), (1112589773714814787, 59, 26), (7653465464155633820, 57, 126), (8441658130407813163, 56, 247), (7796184880221952323, 57, 127), (3805867415840165165, 56, 247), (13018886736500268387, 59, 18), (861360832335769275, 59, 27), },
{(3813761590207912007, 59, 28), (11840609253364038539, 59, 29), (11870034085033088943, 56, 247), (17160772396943258887, 53, 2033), (4581540828647341290, 53, 2019), (11685206029140899769, 57, 127), (6910128804904879121, 59, 29), (15972165749892854821, 59, 23), },
{(5459891855613270045, 60, 15), (2004451710192250317, 59, 29), (1258881586207139533, 56, 248), (1330008569646771699, 53, 2037), (3430471511059421753, 54, 1023), (8632627628886514258, 56, 250), (8214603965802478071, 59, 27), (5813171141027204826, 59, 28), },
{(7767928591217625477, 59, 27), (4116136269640253977, 59, 21), (9163585168437799550, 56, 247), (15849246386012897242, 57, 127), (12218356009148623395, 56, 245), (13739638821877285624, 57, 126), (11408407160494860516, 59, 29), (1156827939301726386, 59, 28), },
{(13507209110543103064, 59, 28), (14223156230265237292, 59, 27), (14219094959267993, 60, 15), (4436622775446954118, 59, 27), (12935362786096461845, 59, 28), (17534006271525372027, 59, 29), (12626205955617136962, 59, 29), (9608865608485327791, 59, 28), },
{(5884390195443219990, 58, 61), (2950984773874272189, 60, 15), (6163355476242855291, 59, 29), (2382113612455179233, 59, 29), (15610587955330775350, 59, 29), (8988450839089140918, 59, 28), (7761628710034260999, 60, 15), (832783515093581598, 58, 63), },
            };
        }
    }
}
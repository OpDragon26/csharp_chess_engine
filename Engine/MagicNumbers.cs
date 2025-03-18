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
            ulong magicNumber;
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
                if (!results.GroupBy(x => x).Any(g => g.Count() > 1))
                {
                    ulong[] temp = (ulong[])results.Clone();
                
                    for (int i = 0; i < 16; i++)
                    {
                        // push further right by a certain amount, and check for duplicates again
                        for (int j = 0; j < temp.Length; j++)
                        {
                            temp[j] >>= 1;
                        }
                    
                        // if there are no duplicates in temp
                        if (!temp.GroupBy(x => x).Any(g => g.Count() > 1))
                        {
                            if (i != 0)
                            {
                                for (int j = 0; j < results.Length; j++)
                                {
                                    results[j] >>= 1;
                                }

                                push++;
                            }
                        }
                        else break;
                        
                    }
                    magicNumber = candidateNumber;
                    break;
                }
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
{(6677373202327596466, 50, 16379), (4069462089598862724, 51, 8188), (6103041769978737287, 50, 16359), (17326281138926701774, 50, 16372), (4004143080694134800, 50, 16369), (3316717999356395122, 51, 8183), (7962024778974587085, 50, 16335), (12861717718797485617, 49, 32756), },
{(4790002616661785756, 50, 16367), (493522592641642669, 51, 8155), (17963498415181725583, 51, 8173), (17595711892284358305, 52, 4089), (3289154571965131566, 51, 8168), (1063043774957390161, 51, 8182), (2758976086579743901, 51, 8169), (8584332733735538584, 50, 16377), },
{(11565620208933541146, 50, 16370), (8411946808381839738, 51, 8184), (7173965593028559258, 50, 16322), (12419366357788823589, 50, 16232), (17723263263620602825, 51, 8181), (9226692493098440444, 51, 8188), (9061648454111048249, 51, 8182), (11603636536934463861, 49, 32685), },
{(3783251821132906470, 51, 8188), (12352423734199707513, 51, 8182), (1041500588243590569, 51, 8181), (14918141503328041944, 51, 8187), (957829021503813730, 50, 16324), (6172669806062320397, 50, 16328), (10823924366204496853, 51, 8176), (17522048102402684895, 49, 32727), },
{(14339309764655976086, 50, 16369), (13634991920589529581, 51, 8177), (9697795000764414066, 51, 8189), (13463635125906983218, 50, 16286), (16640799292405551568, 50, 16310), (6802637175553290762, 51, 8187), (12034831209836601, 51, 8178), (14968379475734388028, 49, 32708), },
{(5777557294016103782, 51, 8189), (12414834219221783690, 51, 8163), (17101681070049722886, 51, 8185), (5009007208934988930, 51, 8186), (4456922798733272132, 51, 8165), (98269371341630600, 51, 8180), (11585585889899702554, 51, 8180), (2921527179812376290, 49, 32716), },
{(14499780159630479710, 51, 8171), (10865074347018599915, 51, 8166), (2850139464410849474, 51, 8118), (12197316577469035700, 51, 8168), (3151639492192253819, 51, 8177), (13179455625375104844, 51, 8172), (11832685430911734507, 51, 8180), (8388475633036082964, 50, 16365), },
{(7436871899686309010, 50, 16379), (14179893128534240955, 50, 16363), (4928342657281818753, 50, 16272), (6153868758447107769, 50, 16369), (9929660783591794024, 50, 16356), (12861789671841452164, 50, 16350), (5553831155057053536, 50, 16364), (18122460828454590614, 49, 32763), },
            };

            BishopNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(921497391920253657, 56, 252), (14047294880282919897, 57, 117), (3968737243220072254, 57, 116), (15519566644191188094, 57, 113), (17243882388955479790, 57, 119), (1269399093960098234, 57, 109), (2693700303763867278, 57, 120), (3586535738182220244, 55, 492), },
{(2595151062368290850, 57, 114), (10496358464819778374, 57, 120), (6328728486682232305, 57, 109), (3500785617760803997, 57, 114), (1197285212367634982, 57, 119), (11811817401818997130, 57, 110), (17621266311871709162, 57, 118), (6500656361984139775, 57, 117), },
{(12149545992211925258, 58, 62), (10101438392152639774, 57, 117), (18358824450510340679, 54, 1010), (13984623142645481690, 54, 986), (9939574080368674094, 54, 991), (15846401670507413569, 55, 500), (17564378054268011452, 57, 120), (5294551039469968265, 58, 62), },
{(685111239640218424, 57, 119), (7383321947577842788, 57, 120), (3040344504082844672, 54, 1008), (3161635835032027615, 51, 8157), (11200287927766126475, 51, 8144), (8538209821006929907, 54, 1004), (9654487013917007344, 57, 111), (18389608593799021341, 57, 121), },
{(9755490980185072697, 57, 120), (14466781738038051473, 57, 119), (10764583075621399894, 54, 1004), (6255644433231913840, 51, 8123), (586673278078122316, 52, 4079), (6584274536078396810, 54, 997), (17033282879629110286, 57, 116), (1400701170950373448, 57, 113), },
{(10989692448745524860, 57, 121), (2877145630545850998, 57, 119), (12831436489198255254, 54, 1008), (6995029547818310511, 55, 507), (15559828486934900945, 54, 1007), (4172041460091402931, 54, 1010), (12154420632515950450, 57, 119), (18422975260781849672, 57, 119), },
{(11188723269495654872, 57, 110), (1573032213560600171, 57, 114), (7065968987537339318, 57, 117), (11261248484666935533, 57, 122), (6479328684293982892, 57, 117), (7225452212382637605, 57, 117), (492147088771048684, 57, 120), (17151101229187355359, 57, 116), },
{(14346816830743529952, 55, 483), (15031956021077695911, 57, 122), (13004156398362521411, 57, 117), (14294784303664054655, 57, 101), (10701774226364851263, 57, 118), (10155706935576773634, 57, 116), (40514729702879095, 57, 119), (4337620452896292361, 56, 250), },
            };
        }
    }
}
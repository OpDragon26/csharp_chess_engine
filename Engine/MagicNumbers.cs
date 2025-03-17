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
                
                    for (int i = 1; i < 16; i++)
                    {
                        // push further right by a certain amount, and check for duplicates again
                        for (int j = 0; j < temp.Length; j++)
                        {
                            temp[j] >>= 1;
                        }
                    
                        // if there are no duplicates in temp
                        if (!temp.GroupBy(x => x).Any(g => g.Count() > 1))
                        {
                            for (int j = 0; j < results.Length; j++)
                            {
                                results[j] >>= 1;
                            }

                            push++;
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
{(1677159705002471451, 50, 16362), (10612477941589208987, 51, 8176), (4295839219994165475, 52, 4094), (2144953800133292526, 52, 4094), (128634253201483797, 52, 4094), (10565719841729005291, 51, 8182), (5238413309391698191, 51, 8173), (9036001631857971951, 50, 16376), },
{(16916067461742591020, 52, 4092), (12348871363154343369, 52, 4082), (6336634660816159967, 52, 4063), (12861699556792609860, 52, 4085), (254969543623314644, 52, 4075), (1023445544819273418, 52, 4089), (9675359323452954262, 52, 4085), (845142433331488395, 51, 8183), },
{(16792714016594522090, 51, 8189), (13116486093704723915, 52, 4091), (16109940210435061811, 52, 4091), (1575046982188235894, 52, 4094), (16774882613138137655, 52, 4094), (8059769327457055490, 52, 4083), (13950665609350899286, 52, 4093), (5393239524160599348, 51, 8183), },
{(14340150851262841950, 51, 8186), (17436818610270488992, 52, 4087), (12713368738213099111, 52, 4090), (11373394458562407010, 51, 8146), (2681694971721950111, 52, 4092), (17572440821826403855, 51, 8155), (7835543227211660657, 52, 4092), (10560187821547167831, 50, 16367), },
{(17496086425181648706, 51, 8164), (7721082518129805262, 52, 4089), (13919854125102433885, 51, 8167), (9952613139424627184, 52, 4089), (8458127058336286236, 52, 4087), (1150996856737653028, 52, 4084), (5688797820794116999, 52, 4089), (6568412650752224844, 50, 16357), },
{(17655854234003818542, 52, 4093), (6367296239267162800, 52, 4059), (627951972921738114, 52, 4093), (7294288657803949578, 52, 4085), (2176365510961508675, 52, 4094), (10536851603012478979, 52, 4088), (17722294748760321177, 52, 4091), (914267004321506027, 50, 16348), },
{(15617667072442913710, 52, 4094), (12101190479530793394, 52, 4083), (10998910376161820921, 52, 4085), (4382920799841126483, 52, 4087), (11788151620249067369, 52, 4077), (17753014832303161228, 52, 4077), (3211710598628594665, 52, 4069), (6181331247540565847, 51, 8180), },
{(16909588813121627086, 51, 8190), (12522670766495978054, 52, 4094), (6020843890089322486, 51, 8175), (10156434878222301634, 51, 8183), (18112776412586688150, 51, 8182), (11524108288395477478, 51, 8167), (5344129271958563912, 51, 8184), (17270736745577273138, 50, 16378), },
            };

            BishopNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(17302010647290341325, 57, 124), (7283051691430498438, 58, 57), (16136073359576436078, 58, 59), (15992684217289796906, 58, 59), (8272100461450816795, 58, 57), (9842335931674369120, 59, 31), (11204543805508583762, 58, 60), (397146857602930544, 57, 126), },
{(16421941166706513567, 58, 61), (4173209757628712847, 58, 59), (2528676085216097234, 58, 59), (17399121272521885776, 58, 55), (14325485758750871545, 58, 59), (14144065714230690838, 58, 54), (13901731466730609336, 58, 59), (5560012621903290805, 58, 59), },
{(798643898081418729, 58, 58), (16605095374270527725, 58, 58), (11426433016397230097, 55, 462), (12920633981026463803, 55, 497), (7583865756835459340, 55, 500), (18158802008237956205, 55, 500), (364905992730505872, 58, 59), (2943731338105954212, 58, 60), },
{(5260240507022205105, 58, 55), (13652650815765265098, 58, 44), (17698313126199443809, 56, 252), (12127189643093844057, 52, 4077), (14999471415410424037, 53, 2044), (9794203238718837953, 55, 503), (11488017462347775511, 58, 43), (10145577792786537065, 58, 59), },
{(9561028322972934663, 58, 47), (13196301366718661933, 58, 49), (5465293055733022421, 55, 502), (4043604686903243108, 52, 4071), (2827502039959037434, 52, 4062), (16160208545217825290, 55, 498), (11448374240588505984, 58, 59), (12506574118513916516, 58, 58), },
{(14711156288027815967, 58, 59), (2667895254535493647, 58, 46), (7703367800835292658, 55, 500), (8140272411181029490, 55, 502), (5277151638065557512, 55, 499), (985962032112583572, 55, 496), (1735892508275297019, 59, 31), (7521554625698199903, 58, 60), },
{(3957949200989862126, 59, 31), (1500683992117728087, 58, 57), (7925906208068914606, 58, 57), (9630562481169203903, 58, 59), (5252626534526069575, 58, 60), (16704518353091823155, 58, 60), (3508782547637829955, 58, 59), (16402271541189817201, 58, 60), },
{(4220438007324093555, 57, 124), (2018818754988093529, 58, 59), (2538740070808037132, 58, 59), (15486855645322204735, 58, 55), (833183614429634865, 58, 56), (6212862402480412164, 58, 57), (18414761610178027766, 58, 59), (17454074124957654303, 57, 126), },
            };
        }
    }
}
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
{(6416312523746746857, 49, 32764), (3629172270483163201, 50, 16365), (2614157119682188525, 49, 32765), (17883916387979847086, 49, 32742), (4764431467430503353, 49, 32704), (10965589041419955033, 50, 16383), (9151229872997945946, 50, 16366), (15937554827926194741, 49, 32748), },
{(5051011351307344979, 49, 32766), (17458858960378225077, 52, 4092), (18424818953587257576, 51, 8187), (11029012400717160716, 49, 32767), (15973943302058999214, 50, 16380), (5160425498478016754, 49, 32699), (16432074645641597321, 50, 16379), (10828143832637280499, 50, 16364), },
{(7038999947338187387, 49, 32738), (17129147074578965363, 51, 8180), (5666591341238269220, 51, 8185), (7282982124398558654, 49, 32767), (12164369020063763936, 49, 32765), (12723620838545696844, 52, 4093), (1844293007520682296, 50, 16346), (11511517623489416529, 50, 16376), },
{(4686877000275732453, 49, 32684), (9126719546645988454, 50, 16350), (12461680635433253433, 51, 8187), (1649408177383253863, 50, 16382), (2025325446471302565, 49, 32727), (14231401145996326198, 49, 32730), (6448891962221544838, 51, 8182), (12476933240837337228, 49, 32755), },
{(10709167403376171839, 49, 32729), (4320174495520858814, 50, 16378), (10997787856689286100, 50, 16379), (3761046322220939744, 51, 8186), (4748118815168341115, 51, 8185), (9935781616320894268, 50, 16376), (8839908371182984054, 50, 16373), (17357888053897712487, 49, 32767), },
{(7725585134856350435, 50, 16378), (16393513520468017668, 52, 4094), (15159478697516153558, 49, 32753), (11531700092997925751, 50, 16381), (11052476764264115537, 49, 32745), (3407011943604966216, 49, 32765), (9383761537249108336, 49, 32738), (7584495688635981277, 50, 16380), },
{(6796511803040002949, 49, 32756), (10381754256915226623, 51, 8191), (15447508846887064125, 51, 8185), (863987250451850357, 51, 8191), (8624869368536773697, 51, 8184), (17435006602284044553, 49, 32746), (8155527178534263954, 50, 16371), (12836866611994230100, 49, 32742), },
{(8625291513691795230, 49, 32767), (17782282614526336142, 50, 16379), (5635881300280223927, 49, 32764), (12516260368582988586, 50, 16363), (11008989111411194616, 51, 8188), (13390622892647756506, 51, 8189), (8210195289776931490, 51, 8188), (11829756923666472376, 49, 32765), },
            };

            BishopNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(594604481899319183, 56, 238), (14982279496325696706, 56, 254), (3805517969112256309, 58, 62), (13898086277628227783, 57, 127), (6115134454792682443, 55, 511), (17948691761463658268, 51, 8114), (11904698747698383793, 56, 254), (15434588760711709439, 55, 483), },
{(16315352960953244844, 58, 59), (3973368824569461105, 55, 495), (14938705295438957017, 57, 125), (8115818431265375813, 57, 119), (16854088096481095508, 56, 250), (13961245609470834463, 58, 62), (13512462215199211037, 58, 63), (4829347140072434955, 58, 63), },
{(1901769568474538583, 56, 251), (17791146468487919843, 57, 125), (7829146584893121093, 55, 509), (15367926449648904618, 54, 1008), (12351134881321728434, 53, 2045), (3623123625731079386, 52, 4079), (11595755588214593799, 57, 126), (894948353505895931, 57, 125), },
{(4195841629344978354, 55, 497), (13740153372424886521, 51, 7970), (5444856414476334561, 52, 4089), (6620917076889140812, 50, 16369), (2805270473305722591, 52, 4094), (12537131442495114359, 52, 4085), (13779224117111594180, 53, 2040), (15400981097001609590, 57, 122), },
{(17990372791021827838, 57, 125), (8916157410060477583, 55, 485), (4328867569095373309, 54, 1011), (2373813856663599248, 50, 16378), (14174609933092900269, 52, 4094), (6408747812517607522, 54, 985), (9392980743998992306, 56, 245), (14968246127184675712, 57, 124), },
{(4363475104861686964, 56, 247), (9147299364174533057, 57, 122), (1923422289043149827, 55, 511), (9094892858124744563, 50, 16231), (9645221558418890175, 54, 1021), (14138122817479305057, 53, 2034), (6227956459937068689, 57, 121), (3845352159089029083, 56, 250), },
{(5662800704623298905, 58, 63), (13785248715993446266, 58, 58), (11103342748765126402, 58, 63), (17373362260172745199, 57, 124), (4721818375176420746, 56, 245), (15108157222078502751, 56, 251), (6471240429412223008, 59, 31), (16528526781016198450, 56, 252), },
{(13759848969648619740, 52, 4058), (18061587635398724471, 58, 61), (1445566771684177279, 58, 63), (4663604147844543407, 56, 255), (5986546168594945909, 57, 127), (10094607383741563923, 55, 495), (3284387224892999078, 57, 126), (213064541831513746, 52, 4049), },
            };
        }
    }
}
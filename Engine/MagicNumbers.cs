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
                            temp[j] >>= 2;
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
{(16899554226551918382, 49, 32722), (2752093704055496646, 49, 32621), (2633479314493923478, 50, 16377), (14323769953491118950, 49, 32671), (11801113904539376533, 49, 32688), (11565789388793544964, 49, 32623), (11105191275763045579, 49, 32398), (6698564919052400637, 49, 32765), },
{(4946306750650790396, 50, 16369), (10069603912076653065, 50, 16357), (3085703946002166200, 50, 16342), (13263251054825847244, 50, 16328), (17691894292212729769, 50, 16333), (4683374394148127510, 50, 16351), (11091978774440834176, 50, 16325), (2676742410824099082, 49, 32672), },
{(8794901475309891190, 49, 32669), (15712248363484443965, 50, 16361), (7761158760109308928, 50, 16372), (2517312059112382288, 49, 32366), (12406275977977288852, 50, 16377), (5145940683015506211, 50, 16350), (6133809255776742485, 50, 16358), (5236249147406970497, 49, 32680), },
{(16894343652854370694, 49, 32645), (1533265014481376336, 50, 16332), (1461386968639862366, 49, 32537), (1814971825059349503, 49, 32524), (415300036109849077, 50, 16354), (13445342435928969585, 49, 32584), (15837433654435597745, 50, 16362), (12577553829580044993, 49, 32720), },
{(2871647551614077686, 49, 32684), (18141810037585428316, 50, 16341), (2472481881156473539, 50, 16353), (3448838166319881723, 50, 16373), (18088217806150319386, 50, 16365), (12574664808024002854, 50, 16378), (1577553562217503442, 50, 16376), (8273507709306745848, 49, 32730), },
{(2245273972205898154, 49, 32602), (896710163899171087, 50, 16327), (4497043970442538614, 50, 16361), (18367843459504406563, 50, 16377), (10925738367099928855, 50, 16358), (6453896893049502357, 50, 16377), (8427228882362139301, 50, 16377), (13794669097885108291, 49, 32729), },
{(2888484863379853890, 50, 16375), (9203564541510899684, 50, 16350), (6943672984535249983, 50, 16329), (6473831783664882764, 50, 16340), (759260852566736642, 50, 16363), (3094768774635874574, 50, 16351), (6066715781262597385, 50, 16322), (909056567276067519, 49, 32714), },
{(5469828682041222630, 49, 32723), (290207667795613038, 49, 29710), (16251241661001547071, 49, 32586), (18343337584043788775, 49, 32256), (14066793888029752769, 49, 32561), (16695866981333741770, 49, 32693), (4177338996119128014, 49, 32186), (17368604947873823330, 49, 32747), },
            };

            BishopNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(15388610500149917075, 52, 3701), (14696046214483514650, 53, 1886), (1061701099663681264, 53, 1845), (13676102357798805568, 53, 1935), (15233735805687835520, 53, 1887), (9271356575111464213, 53, 1858), (10578629545610053812, 53, 1954), (8633254452758886080, 52, 3901), },
{(9010041079918678338, 53, 1859), (15186327940350440556, 53, 1953), (5247861906411165731, 53, 1892), (9663889749675814349, 53, 1858), (16272731090776808546, 53, 1873), (10065924065484463645, 53, 1886), (9826327618069322364, 53, 1843), (7689519305137824104, 53, 1918), },
{(7677709387895194457, 53, 1868), (16048750074187360850, 53, 1926), (9186201325049717447, 51, 7969), (368080758950232207, 51, 7713), (17659976843243731712, 52, 4071), (561230990172265338, 51, 7885), (13460793712896508319, 53, 1545), (545165931647800314, 53, 1861), },
{(8879061887391485745, 53, 1928), (7136063517661374714, 53, 1887), (15273323864888313883, 51, 7656), (8398283902105492384, 50, 16291), (7400038408631708672, 50, 16236), (16057093172633674858, 51, 7504), (10499702881756119289, 53, 1808), (14799964197790694414, 53, 1945), },
{(15646114840011565663, 53, 1875), (5535042181825406474, 53, 1809), (6767111543126389839, 51, 7606), (11856934942724460983, 50, 16347), (10735250728614340277, 50, 16316), (10375342730120124660, 51, 7933), (4958771270652705175, 53, 1902), (13940831267458058833, 53, 1932), },
{(15187229038852146580, 53, 1952), (5621398573799948973, 53, 1933), (13020208216883414212, 52, 4058), (3470132361250997038, 52, 4071), (17323136040274666220, 51, 7964), (5798201761089876854, 52, 4059), (5686016495950584399, 53, 1948), (15957355317092324879, 53, 1927), },
{(9651427301695152521, 53, 1833), (7222143011213408680, 53, 1910), (4723098115469264344, 53, 1826), (5414629137746377089, 53, 1770), (1356014504520882838, 53, 1958), (16010983183863929679, 53, 1935), (10950506627598666476, 53, 1846), (1063454757788878579, 53, 1925), },
{(14146361073149805200, 52, 3678), (9001120776899947817, 53, 1924), (5872346951858048473, 53, 1925), (10741438425439445062, 53, 1715), (14774626160596041873, 53, 1480), (9819964869280967176, 53, 1880), (14315131859875711072, 53, 1848), (11034533831401843131, 52, 3731), },
            };
        }
    }
}
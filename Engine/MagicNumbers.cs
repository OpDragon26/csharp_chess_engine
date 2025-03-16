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
                magicNumber += candidateNumber;
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
        private static (ulong number, int push, ulong highest)[,] RookNumbers = new (ulong number, int push, ulong highest)[8,8];
        private static (ulong number, int push, ulong highest)[,] BishopNumbers = new (ulong number, int push, ulong highest)[8,8];

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

        public static void StaticInit()
        {
            RookNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(59632646563074354, 4, 4093), (7265320600205459656, 5, 2046), (16942996308915119101, 4, 4090), (6388996823375660299, 4, 4090), (12822992897955842195, 6, 1023), (11066110587780406774, 4, 4085), (6574313827600263979, 4, 4087), (17856279720901744254, 3, 8190), },
{(3997170275247360860, 5, 2046), (4529957951573928984, 5, 2043), (7153615918978939876, 5, 2041), (2328443150139195462, 5, 2041), (11205818918791824116, 5, 2042), (1709349296579376540, 5, 2041), (5366792794500251910, 5, 2042), (18248455629160819514, 4, 4092), },
{(8860318347701032550, 4, 4092), (18400668359752384526, 4, 4047), (4346894678677920310, 5, 2046), (15034373533888584758, 5, 2046), (17491810514197282240, 4, 4075), (5090706400721183539, 5, 2042), (7543017382533186532, 5, 2041), (2841782015048343040, 4, 4094), },
{(4324345802511410530, 4, 4094), (5678329194587482485, 5, 2047), (309822619820226552, 5, 2047), (11672356202075502707, 5, 2047), (14909497563438554455, 5, 2046), (946515080879183084, 5, 2047), (482013073607205733, 5, 2044), (4584658387996621634, 3, 8186), },
{(10437987441485044966, 4, 4092), (9980814374518598644, 5, 2047), (8639245274124841160, 4, 4072), (10768080612795654423, 5, 2045), (8238146313989382449, 5, 2042), (7125133399060421374, 5, 2044), (12102801547588333987, 5, 2045), (14808331454815911920, 3, 8176), },
{(5714741892836549466, 4, 4089), (15352316359981137947, 5, 2012), (5962872709608726850, 5, 2043), (3893397777871901960, 6, 1023), (935763292444216852, 5, 2047), (1868280897762391863, 5, 2043), (13826590039618391854, 5, 2042), (8458903281615415053, 4, 4094), },
{(9025298790715521686, 4, 4088), (3702170759703311588, 5, 2044), (13397403834967875985, 5, 2045), (9030079783436330387, 5, 2042), (10619052764417549054, 5, 2042), (2390310835719838351, 5, 2040), (6098197412560641185, 5, 2043), (15482226826631851269, 4, 4094), },
{(10617659717723447246, 4, 4095), (5900557435013339221, 4, 4088), (11806770232409471140, 4, 4091), (12634656282205242433, 4, 4086), (15510464214494470448, 4, 4085), (9674917205564813233, 4, 4085), (7169049707720589930, 4, 4092), (7922549042135770061, 3, 8189), },
            };

            BishopNumbers = new (ulong number, int push, ulong highest)[,]
            {
{(2299397023613189748, 10, 63), (13104381256064307801, 11, 29), (12152295379802961247, 11, 29), (11996347088531379864, 11, 29), (11537644148199278596, 11, 28), (1766599077465837058, 11, 26), (14134700991743858391, 11, 30), (13935761350911681428, 10, 63), },
{(7878282153785783512, 11, 29), (16685022824364721768, 11, 30), (5590933912324181355, 11, 29), (16573396597427984629, 11, 27), (3204320122073565102, 11, 29), (15342994916754991056, 11, 29), (17637574167105097080, 11, 29), (7997487421424188333, 11, 29), },
{(1580808218509645755, 11, 30), (8731818003948480821, 11, 29), (16011815839997253407, 8, 248), (1578673643829878931, 8, 246), (669777201178847782, 8, 237), (14672532507548008928, 8, 251), (5544964259830317869, 11, 29), (11412287592274991864, 11, 29), },
{(16847716266247602392, 11, 28), (12693401835607935503, 11, 29), (8429187272933637760, 8, 252), (9449321165458819941, 5, 2040), (1819944729591880659, 5, 2041), (5856462645105568626, 8, 253), (3500973813080544545, 11, 29), (2953249068576148447, 12, 15), },
{(11870395706661287112, 11, 28), (5586663078875392206, 11, 27), (16459143177513676784, 9, 127), (9721511084363702053, 5, 2039), (5544829149014227065, 5, 2033), (4880146289794865426, 8, 251), (106296131685496849, 11, 29), (18158956582321776355, 11, 28), },
{(14094854752093471251, 11, 20), (17982800280419012393, 11, 29), (1341670199492566188, 8, 250), (14781575913745043472, 8, 245), (3629930977926782154, 8, 243), (6422346478972961902, 9, 126), (10194548772873504678, 12, 15), (5714700268625878899, 11, 29), },
{(12464074204717300605, 11, 30), (14213655094460988544, 11, 28), (5291566407772829326, 11, 29), (5512483869479775465, 11, 29), (3831405748453622192, 11, 30), (3053429207074399311, 11, 29), (10476339488749012602, 11, 29), (2127336357360126231, 11, 30), },
{(6239166388910516101, 10, 62), (7022688891761784145, 11, 28), (8340740576875223014, 11, 28), (2619604308126286380, 11, 29), (18004788164628719333, 11, 29), (7826146475163264525, 12, 15), (7257713819832817923, 11, 19), (3415977139785645064, 10, 61), },
            };
        }
    }
}
namespace Bitboards
{
    public static class Bitboards
    {
        public static ulong[,] SquareBitboards = new ulong[8, 8];

        public static void Init()
        {
            ulong init = 0x8000000000000000; // The first square

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    SquareBitboards[i, j] = init;
                    init >>= 1; // shift it to the right for the next square
                }
            }
        }
    }
}
namespace Match
{
    public class Match
    {
        public bool PlayerSide = false;
        public int Depth = 2;

        public Match(bool side, int depth)
        {
            PlayerSide = side;
            Depth = depth;
        }
    }
}
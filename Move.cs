using Piece;

namespace Move
{
    public class Move
    {
        public int[] From = new int[] {0,0};
        public int[] To = new int[] {0,0};

        public Piece.Piece Promotion = Piece.Presets.Empty; // empty for the piece to stay the same
    }
}
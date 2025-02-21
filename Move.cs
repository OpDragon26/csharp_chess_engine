using Piece;

namespace Move
{
    public class Move
    {
        public int[] From = new int[] {0,0};
        public int[] To = new int[] {0,0};

        public Piece.Piece Promotion = Piece.Presets.Empty; // Empty for no promotion

        public Move Constructor(int[] from, int[] to, Piece.Piece promotion)
        {
            Move NewMove = new Move();

            NewMove.From = from;
            NewMove.To = to;
            NewMove.Promotion = promotion;

            return NewMove;
        }
    }
}
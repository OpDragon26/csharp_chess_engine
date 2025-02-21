namespace Piece
{
    public enum PieceType
    {
        Empty,
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public class Piece
    {
        public PieceType Role = PieceType.Empty;
        public bool Color = false; // false = white, true = black
    }
}
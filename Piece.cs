using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Threading.Tasks.Dataflow;

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
        public bool Color = false; // false = white (or empty), true = black

        public static Piece Constructor(PieceType role, bool color)
        {
            Piece NewPiece = new Piece();

            NewPiece.Color = color;
            NewPiece.Role = role;

            return NewPiece;
        }
    }

    public static class Presets
    {
        public static Piece Empty = Piece.Constructor(PieceType.Empty, false);
        public static Piece W_Pawn = Piece.Constructor(PieceType.Pawn, false);
        public static Piece W_Rook = Piece.Constructor(PieceType.Rook, false);
        public static Piece W_Knight = Piece.Constructor(PieceType.Knight, false);
        public static Piece W_Bishop = Piece.Constructor(PieceType.Bishop, false);
        public static Piece W_Queen = Piece.Constructor(PieceType.Queen, false);
        public static Piece W_King = Piece.Constructor(PieceType.King, false);
        public static Piece B_Pawn = Piece.Constructor(PieceType.Pawn, true);
        public static Piece B_Rook = Piece.Constructor(PieceType.Rook, true);
        public static Piece B_Knight = Piece.Constructor(PieceType.Knight, true);
        public static Piece B_Bishop = Piece.Constructor(PieceType.Bishop, true);
        public static Piece B_Queen = Piece.Constructor(PieceType.Queen, true);
        public static Piece B_King = Piece.Constructor(PieceType.King, true);

        public static Dictionary<Piece, string> PieceString = new Dictionary<Piece, string>{
            {Empty, " "},
            {W_Pawn, "♟"},
            {W_Rook, "♜"},
            {W_Knight, "♞"},
            {W_Bishop, "♝"},
            {W_Queen, "♛"},
            {W_King, "♚"},
            {B_Pawn, "♙"},
            {B_Rook, "♖"},
            {B_Knight, "♘"},
            {B_Bishop, "♗"},
            {B_Queen, "♕"},
            {B_King, "♔"},
        };
    }
}
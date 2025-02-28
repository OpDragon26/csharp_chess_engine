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
        public int Value = 0;
        public int LocalValue = 0;

        static Dictionary<PieceType, int> Values = new Dictionary<PieceType, int>{
            {PieceType.Pawn, 100},
            {PieceType.Rook, 500},
            {PieceType.Knight, 300},
            {PieceType.Bishop, 300},
            {PieceType.Queen, 900},
            {PieceType.King, 1000},
            {PieceType.Empty, 0},
        };

        static Dictionary<bool, int> Multipliers = new Dictionary<bool, int>{
            {false, 1},
            {true, -1},
        };

        public Piece(PieceType role, bool color)
        {
            Color = color;
            Role = role;
            LocalValue = Values[role];
            Value = Values[role] * 100;
            Value *= Multipliers[color];
        }

    }

    public static class Presets
    {
        public static Piece Empty = new Piece(PieceType.Empty, false);
        public static Piece W_Pawn = new Piece(PieceType.Pawn, false);
        public static Piece W_Rook = new Piece(PieceType.Rook, false);
        public static Piece W_Knight = new Piece(PieceType.Knight, false);
        public static Piece W_Bishop = new Piece(PieceType.Bishop, false);
        public static Piece W_Queen = new Piece(PieceType.Queen, false);
        public static Piece W_King = new Piece(PieceType.King, false);
        public static Piece B_Pawn = new Piece(PieceType.Pawn, true);
        public static Piece B_Rook = new Piece(PieceType.Rook, true);
        public static Piece B_Knight = new Piece(PieceType.Knight, true);
        public static Piece B_Bishop = new Piece(PieceType.Bishop, true);
        public static Piece B_Queen = new Piece(PieceType.Queen, true);
        public static Piece B_King = new Piece(PieceType.King, true);

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

        public static Dictionary<Piece, Piece> Clone = new Dictionary<Piece, Piece>{
            {Empty, Empty},
            {W_Pawn, W_Pawn},
            {W_Rook, W_Rook},
            {W_Knight, W_Knight},
            {W_Bishop, W_Bishop},
            {W_Queen, W_Queen},
            {W_King, W_King},
            {B_Pawn, B_Pawn},
            {B_Rook, B_Rook},
            {B_Knight, B_Knight},
            {B_Bishop, B_Bishop},
            {B_Queen, B_Queen},
            {B_King, B_King},
        };
    }
}
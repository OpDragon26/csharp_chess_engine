using System.Collections.Generic;

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
        public PieceType Role;
        public bool Color; // false = white (or empty), true = black
        public int Value;
        public int LocalValue;
        public int HashValue;

        public static readonly Dictionary<PieceType, int> Values = new Dictionary<PieceType, int>{
            {PieceType.Pawn, 100},
            {PieceType.Rook, 500},
            {PieceType.Knight, 300},
            {PieceType.Bishop, 300},
            {PieceType.Queen, 900},
            {PieceType.King, 1000},
            {PieceType.Empty, 0},
        };

        static readonly Dictionary<bool, int> Multiplier = new Dictionary<bool, int>{
            {false, 1},
            {true, -1},
        };

        public Piece(PieceType role, bool color, bool hash = true)
        {
            Color = color;
            Role = role;
            LocalValue = Values[role];
            Value = Values[role] * Multiplier[color];
            if (hash)
                HashValue = HashCodeHelper.HashCodeHelper.GetPieceHashValue(new  Piece(role, color, false));
        }

        public override int GetHashCode()
        {
            return this.HashValue;
        }

    }

    public static class Presets
    {
        public static readonly Piece Empty = new(PieceType.Empty, false);
        public static readonly Piece W_Pawn = new(PieceType.Pawn, false);
        public static readonly Piece W_Rook = new(PieceType.Rook, false);
        public static readonly Piece W_Knight = new(PieceType.Knight, false);
        public static readonly Piece W_Bishop = new(PieceType.Bishop, false);
        public static readonly Piece W_Queen = new(PieceType.Queen, false);
        public static readonly Piece W_King = new(PieceType.King, false);
        public static readonly Piece B_Pawn = new(PieceType.Pawn, true);
        public static readonly Piece B_Rook = new(PieceType.Rook, true);
        public static readonly Piece B_Knight = new(PieceType.Knight, true);
        public static readonly Piece B_Bishop = new(PieceType.Bishop, true);
        public static readonly Piece B_Queen = new(PieceType.Queen, true);
        public static readonly Piece B_King = new(PieceType.King, true);

        public static readonly Dictionary<Piece, string> PieceString = new Dictionary<Piece, string>{
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

        public static readonly Dictionary<Piece, Piece> Clone = new Dictionary<Piece, Piece>{
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
using System.Collections.Generic;

namespace Piece
{

    public enum PieceType
    {
        Empty = 6,
        Pawn = 5,
        Knight = 3,
        Bishop = 1,
        Rook = 0,
        Queen = 2,
        King = 4
    }

    public class Piece
    {
        public PieceType Role;
        public bool Color; // false = white (or empty), true = black
        public int Value;
        public int LocalValue;
        public int HashValue;
        public uint MPValue;

        public static readonly Dictionary<PieceType, int> Values = new() {
            {PieceType.Pawn, 130},
            {PieceType.Rook, 500},
            {PieceType.Knight, 300},
            {PieceType.Bishop, 300},
            {PieceType.Queen, 900},
            {PieceType.King, 1000},
            {PieceType.Empty, 0},
        };
        
        public static readonly Dictionary<PieceType, int> SortValues = new() {
            {PieceType.Pawn, 1},
            {PieceType.Rook, 4},
            {PieceType.Knight, 2},
            {PieceType.Bishop, 3},
            {PieceType.Queen, 5},
            {PieceType.King, 6},
        };

        static readonly Dictionary<bool, int> Multiplier = new() {
            {false, 1},
            {true, -1},
        };

        public Piece(PieceType role, bool color,  uint mPValue, bool hash = true)
        {
            Color = color;
            Role = role;
            LocalValue = Values[role];
            Value = Values[role] * Multiplier[color];
            MPValue = mPValue;
            if (hash)
                HashValue = HashCodeHelper.HashCodeHelper.GetPieceHashValue(new  Piece(role, color,0, false));
        }

        public override int GetHashCode()
        {
            return HashValue;
        }

    }

    public static class Presets
    {
        public static readonly Piece Empty = new(PieceType.Empty, false,6);
        public static readonly Piece W_Pawn = new(PieceType.Pawn, false,0);
        public static readonly Piece W_Rook = new(PieceType.Rook, false,1);
        public static readonly Piece W_Knight = new(PieceType.Knight, false,2);
        public static readonly Piece W_Bishop = new(PieceType.Bishop, false,3);
        public static readonly Piece W_Queen = new(PieceType.Queen, false,4);
        public static readonly Piece W_King = new(PieceType.King, false,5);
        public static readonly Piece B_Pawn = new(PieceType.Pawn, true,8);
        public static readonly Piece B_Rook = new(PieceType.Rook, true,9);
        public static readonly Piece B_Knight = new(PieceType.Knight, true,10);
        public static readonly Piece B_Bishop = new(PieceType.Bishop, true,11);
        public static readonly Piece B_Queen = new(PieceType.Queen, true,12);
        public static readonly Piece B_King = new(PieceType.King, true,13);

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
using Piece;
using static Piece.Presets;

namespace Move
{
    public class Move
    {
        public int[] From = new int[] {0,0};
        public int[] To = new int[] {0,0};

        public Piece.PieceType Promotion = Piece.PieceType.Empty; // Empty for no promotion


        public static Dictionary<string, int> FileIndex = new Dictionary<string, int>{
            {"a",0},
            {"b",1},
            {"c",2},
            {"d",3},
            {"e",4},
            {"f",5},
            {"g",6},
            {"h",7},
        };

        public static Dictionary<string, Piece.PieceType> Promotions = new Dictionary<string, Piece.PieceType>{
            {"Q",Piece.PieceType.Queen},
            {"R",Piece.PieceType.Rook},
            {"N",Piece.PieceType.Knight},
            {"B",Piece.PieceType.Bishop},
        };

        public static Move Constructor(int[] from, int[] to, Piece.PieceType promotion)
        {
            Move NewMove = new Move();

            NewMove.From = from;
            NewMove.To = to;
            NewMove.Promotion = promotion;

            return NewMove;
        }

        public static Move FromString(string move) // Format: from-to(-promotion) a2-b1(-Q)
        {
            Move NewMove = new Move();

            string[] MoveString = move.Split("-");

            NewMove.From[0] = FileIndex[MoveString[0][0].ToString()];
            NewMove.From[1] = Int32.Parse(MoveString[0][1].ToString()) - 1;
            NewMove.To[0] = FileIndex[MoveString[1][0].ToString()];
            NewMove.To[1] = Int32.Parse(MoveString[1][1].ToString()) - 1;

            if (MoveString.Length == 2)
            {
                NewMove.Promotion = PieceType.Empty;
            }
            else if (MoveString.Length == 3)
            {
                NewMove.Promotion = Promotions[MoveString[2]];
            }

            return NewMove;
        }
    }
}
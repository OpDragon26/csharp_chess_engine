using Piece;
using static Piece.Presets;

namespace Move
{
    public class Move
    {
        public int[] From = new int[] {0,0};
        public int[] To = new int[] {0,0};

        public Piece.Piece Promotion = Piece.Presets.Empty; // Empty for no promotion
        public bool Color = false;


        public static Dictionary<string, int> FileIndex = new Dictionary<string, int>{
            {"a",1},
            {"b",2},
            {"c",3},
            {"d",4},
            {"e",5},
            {"f",6},
            {"g",7},
            {"h",8},
        };

        public static Dictionary<string, Piece.Piece> Promotions = new Dictionary<string, Piece.Piece>{
            {"Q",W_Queen},
            {"R",W_Rook},
            {"N",W_Knight},
            {"B",W_Bishop},
            {"q",B_Queen},
            {"r",B_Rook},
            {"n",B_Knight},
            {"b",B_Bishop},
        };

        public Move Constructor(int[] from, int[] to, bool color, Piece.Piece promotion)
        {
            Move NewMove = new Move();

            NewMove.From = from;
            NewMove.To = to;
            NewMove.Color = color;
            NewMove.Promotion = promotion;

            return NewMove;
        }

        public static Move FromString(string move) // Format: from-to(-promotion) a2-b1(-Q)
        {
            Move NewMove = new Move();

            string[] MoveString = move.Split("-");

            NewMove.From[0] = FileIndex[MoveString[0][0].ToString()];
            NewMove.From[1] = 7 - Convert.ToInt32(MoveString[0][1]);
            NewMove.To[0] = FileIndex[MoveString[1][0].ToString()];
            NewMove.To[1] = 7 - Convert.ToInt32(MoveString[1][1]);


            if (MoveString.Length == 2)
            {
                NewMove.Promotion = Piece.Presets.Empty;
            }
            else if (MoveString.Length == 3)
            {
                NewMove.Promotion = Promotions[MoveString[2]];
            }

            return NewMove;
        }
    }
}
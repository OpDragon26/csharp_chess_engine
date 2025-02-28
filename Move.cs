using static Piece.Presets;
using static Board.Presets;

namespace Move
{
    public class Move
    {
        public int[] From = new int[] {0,0};
        public int[] To = new int[] {0,0};

        public Piece.Piece Promotion = Empty; // Empty for no promotion

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

        public static Move Constructor(int[] from, int[] to, Piece.Piece promotion)
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

            NewMove.From = ConvertSquare(MoveString[0], false);
            NewMove.To = ConvertSquare(MoveString[1], false);

            //Console.WriteLine(NewMove.From[0]);
            //Console.WriteLine(NewMove.From[1]);
            //Console.WriteLine(NewMove.To[0]);
            //Console.WriteLine(NewMove.To[1]);

            if (MoveString.Length == 2)
            {
                NewMove.Promotion = Empty;
            }
            else if (MoveString.Length == 3)
            {
                NewMove.Promotion = Promotions[MoveString[2]];
            }

            return NewMove;
        }

        public int Value(Board.Board board)
        {
            return board.board[To[1],To[0]].LocalValue - board.board[From[1],From[0]].LocalValue;
        }

        public bool InMovelist(List<Move> MoveList)
        {
            for (int i = 0; i < MoveList.Count; i++)
            {
                if (MoveList[i].Equals(this))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object? obj)
        {
            var item = obj as Move;

            if (item == null)
            {
                return false;
            }

            return Enumerable.SequenceEqual(this.From, item.From) && Enumerable.SequenceEqual(this.To, item.To) && this.Promotion == item.Promotion;
        }

        public override int GetHashCode() => HashCode.Combine(this.From, this.To, this.Promotion);

        public static string[] Files = {"a","b","c","d","e","f","g","h"};
        public string Notate()
        {
            return Files[From[0]] + (From[1] + 1).ToString() + "-" + Files[To[0]] + (To[1] + 1).ToString();
        }
    }
}
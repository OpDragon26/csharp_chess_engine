using static Piece.Presets;
using System.Collections.Generic;
using System;

namespace Move
{
    public class Move
    {
        public (int,int) From;
        public (int,int) To;
        public readonly int Importance;
        public bool EnPassant;

        public Piece.Piece Promotion; // Empty for no promotion

        private static readonly Dictionary<string, int> FileIndex = new Dictionary<string, int>{
            {"a",0},
            {"b",1},
            {"c",2},
            {"d",3},
            {"e",4},
            {"f",5},
            {"g",6},
            {"h",7},
        };

        private static readonly Dictionary<string, Piece.Piece> Promotions = new Dictionary<string, Piece.Piece>{
            {"Q",W_Queen},
            {"R",W_Rook},
            {"N",W_Knight},
            {"B",W_Bishop},
            {"q",B_Queen},
            {"r",B_Rook},
            {"n",B_Knight},
            {"b",B_Bishop},
        };

        public Move((int,int) from, (int,int) to, Piece.Piece promotion, int importance=0, bool enPassant=false)
        {
            From = from;
            To = to;
            Promotion = promotion;
            Importance = importance;
            EnPassant = enPassant;
        }

        public static Move FromString(string move) // Format: from-to(-promotion) a2-b1(-Q)
        {
            Move NewMove = new Move((8,8), (8,8), Empty, 0);

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
        
        private static (int,int) ConvertSquare(string square, bool reverse)
        {
            if (!reverse)
            {
                return (FileIndex[square[0].ToString()], Int32.Parse(square[1].ToString()) - 1);
            } else {
                return (Int32.Parse(square[1].ToString()) - 1, FileIndex[square[0].ToString()]);
            }
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

        public override bool Equals(object obj)
        {
            var item = obj as Move;

            if (item == null)
            {
                return false;
            }

            return this.From == item.From && this.To == item.To && this.Promotion == item.Promotion;
        }

        public override int GetHashCode() => HashCode.Combine(this.From, this.To, this.Promotion);

        public static string[] Files = {"a","b","c","d","e","f","g","h"};
        public string Notate()
        {
            return (Files[From.Item1] + (From.Item2 + 1)).ToString() + "-" + (Files[To.Item1] + (To.Item2 + 1)).ToString();
        }
    }
}
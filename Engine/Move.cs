using static Piece.Presets;
using System.Collections.Generic;
using System;

namespace Move
{
    public struct Move
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

        public bool InMovelist(List<Move> MoveList)
        {
            for (int i = 0; i < MoveList.Count; i++)
            {
                if (MoveList[i].From == From && MoveList[i].To == To && MoveList[i].Promotion.Role == Promotion.Role)
                {
                    return true;
                }
            }

            return false;
        }

        public static string[] Files = {"a","b","c","d","e","f","g","h"};
    }

    public static class MoveOp
    {
        // every move can be represented as a 32 bit uint:
        // the coords to the from and to squares need 4 numbers each between 0-7, meaning 3 bits is enough to represent one
        // -> first 12 bits
        // there are 13 kind of pieces (6 for each side + empty)
        // -> 4 bits for promotion
        // all that remains is importance which can have the remaining 16 bits
        

        //public static (int,int) From();
    }
}
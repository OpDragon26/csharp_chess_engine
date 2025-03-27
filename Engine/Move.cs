using static Piece.Presets;
using System.Collections.Generic;
using System;
using UnityEngine;

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
        
        /*
        0000 w_pawn: 0
        0001 w_rook: 1
        0010 w_knight: 2
        0011 w_bishop: 3
        0100 w_queen: 4
        0101 w_king: 5
        
        0110 empty: 6
        
        1000 b_pawn: 8
        1001 b_rook: 9
        1010 b_knight: 10
        1011 b_bishop: 11
        1100 b_queen: 12
        1101 b_king: 13
        */

        private static readonly Piece.Piece[] PromotionPieces = new[]
        {
            W_Pawn,
            W_Rook,
            W_Knight,
            W_Bishop,
            W_Queen,
            W_King,
            Empty,
            Empty,
            B_Pawn,
            B_Rook,
            B_Knight,
            B_Bishop,
            B_Queen,
            B_Knight
        };

        private static readonly uint ImportanceMask = 0xFFFF; // the 16 bits on the left

        public static (uint, uint) From(uint move)
        {
            return (move >> 29, (move << 3) >> 29);
        }
        
        public static (uint, uint) To(uint move)
        {
            return ((move << 6) >> 29, (move << 9) >> 29);
        }

        public static Piece.Piece Promotion(uint move)
        {
            return PromotionPieces[(move << 12) >> 28];
        }

        public static int Importance(uint move)
        {
            return (Int16)(move & ImportanceMask);
        }

        public static uint Construct((uint,uint) From, (uint,uint) To, Piece.Piece promotion, int importance=0)
        {
            uint move = 0;

            move |= ((uint)importance & ImportanceMask); // add the importance to the move
            
            // push the max 3 bits of the coord to the right position
            move |= From.Item1 << 29;
            move |= From.Item2 << 26;
            move |= To.Item1 << 23;
            move |= To.Item2 << 20;
            
            // add the promotion
            move |= promotion.MPValue << 16;
            
            return move;
        }
    }
}
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
        /*
        every move can be represented as a 32 bit uint:
        the coords to the from and to squares need 4 numbers each between 0-7, meaning 3 bits is enough to represent one
        -> first 12 bits
        there are 13 kind of pieces (6 for each side + empty)
        -> 4 bits for promotion
        special moves:
        3 bits for special moves to not need to check for pieces in the MakeMove function
        all that remains is importance which can have the remaining 13 bits

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
        
        Special moves:
        000 0000000000000 No special move: 0
        001 0000000000000 White short castle: 8192 or 0x2000
        010 0000000000000 White long castle: 16384 or 0x4000
        011 0000000000000 White double move: 24576 or 0x6000
        100 0000000000000 Black double move: 32768 or 0x8000
        101 0000000000000 Black short castle: 40960 or 0xA000
        110 0000000000000 Black long castle: 49152 or 0xC000
        111 0000000000000 En passant: 57344 or 0xE000
        */

        private static readonly Piece.Piece[] PromotionPieces =
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

        private static readonly uint ImportanceMask = 0x1FFF; // the 13 bits on the left
        private static readonly uint SpecialMask = 0xE000;

        public static readonly Dictionary<bool, uint[]> CastleMasks = new()
        {
            { false, new uint[] {0x2000, 0x4000} },
            { true, new uint[] {0xA000, 0xC000} }
        };

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

        public static uint Special(uint move)
        {
            return move & SpecialMask;
        }

        public static uint Construct((uint,uint) From, (uint,uint) To, Piece.Piece promotion, uint specialCode, uint importance=0)
        {
            uint move = 0;

            move |= (importance & ImportanceMask); // add the importance to the move
            
            // push the max 3 bits of the coord to the right position
            move |= From.Item1 << 29;
            move |= From.Item2 << 26;
            move |= To.Item1 << 23;
            move |= To.Item2 << 20;
            
            // add the promotion
            move |= promotion.MPValue << 16;
            
            // add the special move code
            move |= specialCode;
            
            return move;
        }
    }
}
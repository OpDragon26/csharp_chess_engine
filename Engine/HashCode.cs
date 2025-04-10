using System.Collections.Generic;
using System;
using Piece;

namespace HashCodeHelper
{
    public static class HashCodeHelper
    {
        static readonly Dictionary<PieceType, int> LocalHashValues = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 1 },
            { PieceType.Rook, 2 },
            { PieceType.Knight, 3 },
            { PieceType.Bishop, 4 },
            { PieceType.Queen, 5 },
            { PieceType.King, 6 },
        };
        
        public static int GetPieceHashValue(Piece.Piece piece)
        {
            if (piece.Role == PieceType.Empty)
            {
                return 0;
            }

            if (piece.Color)
            {
                return LocalHashValues[piece.Role] + 6;
            }

            return LocalHashValues[piece.Role];
        }
    }

    public static class ZobristHash
    {
        private static readonly int[][,] BitTables = new int[13][,]
        {
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
            new int[8,8],
        };
        private static int BlackToMove;
        public static readonly Random RandGen = new();

        public static void Init()
        {
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        BitTables[i + 1][j, k] = RandInt();
                    }
                }
            }

            BlackToMove = RandInt();
        }

        public static int Hash(Board.Board board)
        {
            int hash = 0;
            if (board.Side)
                hash = hash ^ BlackToMove;

            ulong whitePieces = board.SideBitboards[false];
            ulong blackPieces = board.SideBitboards[true];
            
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((whitePieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0 || (blackPieces & Bitboards.Bitboards.SquareBitboards[i, j]) != 0)
                    {
                        hash = hash ^ BitTables[board.board[i, j].HashValue][i, j];
                    }
                }
            }
            
            return hash;
        }

        private static int RandInt()
        {
            var buffer = new byte[sizeof(int)];
            RandGen.NextBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}
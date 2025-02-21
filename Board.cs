using System.Security.Cryptography.X509Certificates;
using Piece;
using static Piece.Presets;

namespace Board
{
    public class Board
    {
        public Piece.Piece[,] board = new Piece.Piece[8,8];

        public void PrintBoard(bool color)
        {
            if (!color) {
                string StringBoard = string.Empty;

                for (int i = 0; i < 8; i++)
                {
                    StringBoard += (8 - i).ToString() + " ";

                    for (int j = 0; j < 8; j++)
                    {
                        StringBoard += Piece.Presets.PieceString[this.board[i,j]] + " ";
                    }
                    StringBoard += "\n";
                }

                Console.WriteLine("# A B C D E F G H");
                Console.WriteLine(StringBoard);
            }
        }

        public static Board Constructor(Piece.Piece[,] board)
        {
            Board NewBoard = new Board();

            NewBoard.board = board;

            return NewBoard;

        }
    }

    public static class Presets {
        public static Piece.Piece[,] StartingPosition = new Piece.Piece[,] 
        {
            {B_Rook, B_Knight, B_Bishop, B_Queen, B_King, B_Bishop, B_Knight, B_Rook},
            {B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn},
            {W_Rook, W_Knight, W_Bishop, W_Queen, W_King, W_Bishop, W_Knight, W_Rook},
        };
        public static Board StartingBoard = Board.Constructor(StartingPosition);
    }
}
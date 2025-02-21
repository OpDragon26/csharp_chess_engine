using System.Security.Cryptography.X509Certificates;
using Piece;
using static Piece.Presets;
using Move;

namespace Board
{
    public class Board
    {
        public Piece.Piece[,] board = new Piece.Piece[8,8];

        public void PrintBoard(bool color)
        {
            string StringBoard = string.Empty;

            if (color) {

                for (int i = 0; i < 8; i++)
                {
                    StringBoard += (i + 1).ToString() + " ";

                    for (int j = 0; j < 8; j++)
                    {
                        StringBoard += Piece.Presets.PieceString[this.board[i,j]] + " ";
                    }
                    StringBoard += "\n";
                }
                Console.WriteLine("# A B C D E F G H");

            } else {
                for (int i = 0; i < 8; i++)
                {
                    StringBoard += (8 - i).ToString() + " ";

                    for (int j = 0; j < 8; j++)
                    {
                        StringBoard += Piece.Presets.PieceString[this.board[7 - i,7 - j]] + " ";
                    }
                    StringBoard += "\n";
                }
                Console.WriteLine("# H G F E D C B A");
            }

            Console.WriteLine(StringBoard);
        }

        public void MakeMove(Move.Move move)
        {
            this.board[move.To[1],move.To[0]] = this.board[move.From[1],move.From[0]];
            
            if (move.Promotion != PieceType.Empty)
            {
                this.board[move.To[1],move.To[0]].Role = move.Promotion;
            }

            this.board[move.From[1],move.From[0]] = Empty;
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
            {W_Rook, W_Knight, W_Bishop, W_Queen, W_King, W_Bishop, W_Knight, W_Rook},
            {W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn, W_Pawn},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {Empty, Empty, Empty, Empty, Empty, Empty, Empty, Empty},
            {B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn, B_Pawn},
            {B_Rook, B_Knight, B_Bishop, B_Queen, B_King, B_Bishop, B_Knight, B_Rook},
        };
        public static Board StartingBoard = Board.Constructor(StartingPosition);
    }
}
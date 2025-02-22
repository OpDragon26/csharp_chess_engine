using System.Security.Cryptography.X509Certificates;
using Piece;
using static Piece.Presets;
using Move;

namespace Board
{
    public class Board
    {
        public Piece.Piece[,] board = new Piece.Piece[8,8];
        public Dictionary<bool, bool[]> Castling = new Dictionary<bool, bool[]> {
            {false, new bool[] {true, true}}, // Short, Long
            {true, new bool[] {true, true}},
        };

        public int[] EnpassantSquare = {8,8}; // file, rank  8,8 for no en passant


        public void PrintBoard(bool color)
        {
            string StringBoard = string.Empty;

            if (color) {

                for (int i = 0; i < 8; i++)
                {
                    StringBoard += (i + 1).ToString() + " ";
                    for (int j = 0; j < 8; j++)
                    {
                        StringBoard += Piece.Presets.PieceString[this.board[i,7 - j]] + " ";
                    }
                    StringBoard += "\n";
                }
                Console.WriteLine("# H G F E D C B A");
            } else {
                for (int i = 0; i < 8; i++)
                {
                    StringBoard += (8 - i).ToString() + " ";

                    for (int j = 0; j < 8; j++)
                    {
                        StringBoard += Piece.Presets.PieceString[this.board[7 - i,j]] + " ";
                    }
                    StringBoard += "\n";
                }
                
                Console.WriteLine("# A B C D E F G H");
            }
            Console.WriteLine(StringBoard);
        }

        public void MakeMove(Move.Move move)
        {
            this.board[move.To[1],move.To[0]] = this.board[move.From[1],move.From[0]];
            if (move.Promotion != Empty)
            {
                this.board[move.To[1],move.To[0]] = move.Promotion;
            }
            this.board[move.From[1],move.From[0]] = Empty;
            
            // castling
            if (Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.WKStartPos))
            {
                if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WKShortCastlePos))
                {
                    this.board[Presets.WRShortCastlePos[0],Presets.WRShortCastlePos[1]] = Empty;
                    this.board[Presets.WRShortCastleDest[0],Presets.WRShortCastleDest[1]] = W_Rook;
                } 
                else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WKLongCastlePos))
                {
                    this.board[Presets.WRLongCastlePos[0],Presets.WRLongCastlePos[1]] = Empty;
                    this.board[Presets.WRLongCastleDest[0],Presets.WRLongCastleDest[1]] = W_Rook;
                }
            } 
            else if (Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.BKStartPos))
            {
                if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BKShortCastlePos))
                {
                    this.board[Presets.BRShortCastlePos[0],Presets.BRShortCastlePos[1]] = Empty;
                    this.board[Presets.BRShortCastleDest[0],Presets.BRShortCastleDest[1]] = B_Rook;
                } 
                else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BKLongCastlePos))
                {
                    this.board[Presets.BRLongCastlePos[0],Presets.BRLongCastlePos[1]] = Empty;
                    this.board[Presets.BRLongCastleDest[0],Presets.BRLongCastleDest[1]] = B_Rook;
                }
            }

            // en passant (Holy Hell!)
            if (this.board[move.To[1],move.To[0]].Role == PieceType.Pawn)
            {
                int RankDistance = move.From[1] - move.To[1];

                if (Enumerable.SequenceEqual(move.To, this.EnpassantSquare))
                {
                    if (this.board[move.To[1] + 1,move.To[0]].Role == PieceType.Pawn)
                    {
                        this.board[move.To[1] + 1,move.To[0]] = Empty;
                    }
                    else if (this.board[move.To[1] - 1,move.To[0]].Role == PieceType.Pawn)
                    {
                        this.board[move.To[1] - 1,move.To[0]] = Empty;
                    }
                }
                else if (RankDistance == 2 || RankDistance == -2)
                {
                    this.EnpassantSquare = new int[] {move.From[0], move.To[1] + (RankDistance / 2)};
                }
                else
                {
                    this.EnpassantSquare = new int[] {8,8};
                }

            }
            else
            {
                this.EnpassantSquare = new int[] {8,8};
            } 
        }

        public static Board Constructor(Piece.Piece[,] board, bool[] whiteCastle, bool[] blackCastle, int[] enpassantSquare)
        {
            Board NewBoard = new Board();
            NewBoard.board = board;
            NewBoard.Castling = new Dictionary<bool, bool[]> {{false, blackCastle},{true, whiteCastle}};
            NewBoard.EnpassantSquare = enpassantSquare;
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
        public static Board StartingBoard = Board.Constructor(StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});

        internal static int[] WKStartPos = new int[] {0,4}; // file, rank

        // white short castle
        internal static int[] WKShortCastlePos = new int[] {0,6};
        internal static int[] WRShortCastlePos = new int[] {0,7};
        internal static int[] WRShortCastleDest = new int[] {0,5};

        // white long castle
        internal static int[] WKLongCastlePos = new int[] {0,2};
        internal static int[] WRLongCastlePos = new int[] {0,0};
        internal static int[] WRLongCastleDest = new int[] {0,3};

        internal static int[] BKStartPos = new int[] {7,4}; // file, rank

        // black short castle
        internal static int[] BKShortCastlePos = new int[] {7,6};
        internal static int[] BRShortCastlePos = new int[] {7,7};
        internal static int[] BRShortCastleDest = new int[] {7,5};

        // black long castle
        internal static int[] BKLongCastlePos = new int[] {7,2};
        internal static int[] BRLongCastlePos = new int[] {7,0};
        internal static int[] BRLongCastleDest = new int[] {7,3};

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

        public static int[] ConvertSquare(string square, bool reverse)
        {
            if (!reverse)
            {
                return new int[] {FileIndex[square[0].ToString()], Int32.Parse(square[1].ToString()) - 1};
            } else {
                return new int[] {Int32.Parse(square[1].ToString()) - 1, FileIndex[square[0].ToString()]};
            }
        }
    }
}
using Piece;
using static Piece.Presets;

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
        public bool Side = false;


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

        public bool MakeMove(Move.Move move, bool filter)
        {
            if (filter)
            {
                Move.Move[] LegalMoves = MoveFinder.Search(this, this.Side);
                if (!move.InMovelist(LegalMoves))
                {
                    return false;
                }
            }

            this.board[move.To[1],move.To[0]] = this.board[move.From[1],move.From[0]];
            if (move.Promotion != Empty)
            {
                this.board[move.To[1],move.To[0]] = move.Promotion;
            }
            this.board[move.From[1],move.From[0]] = Empty;
            
            // castling
            if (Enumerable.SequenceEqual(new int[] {move.From[1],move.From[0]}, Presets.WKStartPos))
            {
                this.Castling[false] = new bool[] {false, false};
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
                this.Castling[true] = new bool[] {false, false};
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
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WhiteRookHPos))
            {
                this.Castling[false][0] = false;
            }
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.WhiteRookAPos))
            {
                this.Castling[false][1] = false;
            }
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BlackRookHPos))
            {
                this.Castling[true][0] = false;
            }
            else if (Enumerable.SequenceEqual(new int[] {move.To[1],move.To[0]}, Presets.BlackRookAPos))
            {
                this.Castling[true][1] = false;
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

            this.Side = !this.Side;
            return true;
        }

        public static Board Constructor(Piece.Piece[,] board, bool[] whiteCastle, bool[] blackCastle, int[] enpassantSquare)
        {
            Board NewBoard = new Board();
            NewBoard.board = board;
            NewBoard.Castling = new Dictionary<bool, bool[]> {{false, blackCastle},{true, whiteCastle}};
            NewBoard.EnpassantSquare = enpassantSquare;
            return NewBoard;
        }

        public Board DeepCopy()
        {
            Board Clone = new Board();

            Piece.Piece[,] CloneBoard = new Piece.Piece[8,8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    CloneBoard[i,j] = Piece.Presets.Clone[this.board[i,j]];
                }
            }
            Clone.board = CloneBoard;
            Clone.Castling = new Dictionary<bool, bool[]> {
                {false, new bool[] {this.Castling[false][0], this.Castling[false][1]}},
                {true, new bool[] {this.Castling[true][0], this.Castling[true][1]}}};
            Clone.EnpassantSquare = new int[] {this.EnpassantSquare[0], this.EnpassantSquare[1]};
            Clone.Side = this.Side == true;
            
            return Clone;
        }

        public bool KingInCheck(bool color)
        {
            return MoveFinder.Attacked(this, this.KingPos(color), !color);
        }

        public int[] KingPos(bool color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (this.board[i,j].Role == PieceType.King && this.board[i,j].Color == color)
                    return new int[] {j,i};
                }
            }
            return new int[] {8,8};
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

        internal static int[] WKStartPos = {0,4}; // file, rank

        // white short castle
        internal static int[] WKShortCastlePos = {0,6};
        internal static int[] WRShortCastlePos = {0,7};
        internal static int[] WRShortCastleDest = {0,5};

        // white long castle
        internal static int[] WKLongCastlePos = {0,2};
        internal static int[] WRLongCastlePos = {0,0};
        internal static int[] WRLongCastleDest = {0,3};

        internal static int[] BKStartPos = {7,4}; // file, rank

        // black short castle
        internal static int[] BKShortCastlePos = {7,6};
        internal static int[] BRShortCastlePos = {7,7};
        internal static int[] BRShortCastleDest = {7,5};

        // black long castle
        internal static int[] BKLongCastlePos = {7,2};
        internal static int[] BRLongCastlePos = {7,0};
        internal static int[] BRLongCastleDest = {7,3};

        // rook positions
        internal static int[] WhiteRookAPos = {0,0};
        internal static int[] WhiteRookHPos = {0,7};
        internal static int[] BlackRookAPos = {7,0};
        internal static int[] BlackRookHPos = {7,7};

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
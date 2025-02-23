using Board;
using Piece;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});

// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
PlayingBoard.MakeMove(Move.Move.FromString("a8-f2"));
PlayingBoard.MakeMove(Move.Move.FromString("f1-f3"));
PlayingBoard.MakeMove(Move.Move.FromString("g1-g2"));
Move.Move[] Moves = MoveFinder.Search(PlayingBoard,false);
PlayingBoard.MakeMove(Moves[5]);
// Console.WriteLine(PlayingBoard.KingInCheck(false));
// Console.WriteLine(MoveFinder.Attacked(PlayingBoard, Board.Presets.ConvertSquare("e5", false), true));

PlayingBoard.PrintBoard(false);

// create an algorithm to find all legal moves ✓
// - create an algorithm to find all legal moves with all pieces of a certain color on a certain board ✓
// -- list all possile moves disregarding checks ✓
// -- create an algorithm to detect checks ✓
// -- filter castling through checks ✓
// prevent illegal moves
// - parameter to make illegal moves
// 50 move rule and threefold repetition
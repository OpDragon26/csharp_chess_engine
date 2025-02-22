using Board;
using Piece;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});
// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
//PlayingBoard.MakeMove(Move.Move.FromString("e2-e4"));
Move.Move[] Moves = MoveFinder.Search(PlayingBoard,true);
PlayingBoard.MakeMove(Moves[0]);
PlayingBoard.PrintBoard(false);

// create an algorithm to find all legal moves 
// - create an algorithm to find all legal moves with all pieces of a certain color on a certain board
// -- list all possile moves disregarding checks ✓
// -- create an algorithm to detect checks
// -- filter castling through pieces
// prevent illegal moves
// - parameter to make illegal moves
// 50 move rule and threefold repetition
using Board;
using Piece;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});
// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
PlayingBoard.MakeMove(Move.Move.FromString("e7-e3"));
Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Pawn, false, Board.Presets.ConvertSquare("e2", false));
PlayingBoard.PrintBoard(false);
PlayingBoard.MakeMove(Moves[0]);
PlayingBoard.PrintBoard(false);

// - create an algorythm to find all legal moves with all pieces of a certain color on a certain board
// -- don't add moves where the king would be in check
// -- filter castling through pieces
// prevent illegal moves
// - parameter to make illegal moves
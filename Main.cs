using Board;
using Move;
using Piece;
using static Piece.Presets;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});
// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
PlayingBoard.MakeMove(Move.Move.FromString("e2-e7"));
PlayingBoard.MakeMove(Move.Move.FromString("e8-g8"));
Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Pawn, false, Board.Presets.ConvertSquare("e7", false));
PlayingBoard.PrintBoard(false);
PlayingBoard.MakeMove(Moves[0]);
PlayingBoard.PrintBoard(false);

// create an algorythm to find all legal moves
// - create an algorythm to find all moves with a piece of a certain color from a certain square on a certain board
// -- find a way to do that with pawns, including en passant and promotion
// -- find a way to do that with kings and castling
// - create an algorythm to find all legal moves with all pieces of a certain color on a certain board
// -- don't add moves where the king would be in check
// -- filter castling through pieces
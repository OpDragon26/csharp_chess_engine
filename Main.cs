using Board;
using Move;
using Piece;
using static Piece.Presets;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});
// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
PlayingBoard.MakeMove(Move.Move.FromString("h8-h7"));
Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Rook, true, Board.Presets.ConvertSquare("h7", false));
PlayingBoard.PrintBoard(true);
PlayingBoard.MakeMove(Moves[0]);
PlayingBoard.PrintBoard(true);

// create an algorythm to find all legal moves
// - create an algorythm to find all moves with a piece of a certain color from a certain square on a certain board
// -- find a way to do that with kings and castling
// -- find a way to do that with pawns, including en passant and promotion
// - create an algorythm to find all legal moves with all pieces of a certain color on a certain board
// -- don't add moves where the king would be in check
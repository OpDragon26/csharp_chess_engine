using Board;
using Move;
using static Piece.Presets;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition);
PlayingBoard.MakeMove(Move.Move.FromString("e1-c1")); // Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
PlayingBoard.MakeMove(Move.Move.FromString("e8-c8"));

PlayingBoard.PrintBoard(true);

// implement en passant and castling
// create and algorythm to find all legal moves
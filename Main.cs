using Board;
using Move;
using static Piece.Presets;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition);
PlayingBoard.MakeMove(Move.Move.FromString("e2-e4")); // Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4

PlayingBoard.PrintBoard(false);
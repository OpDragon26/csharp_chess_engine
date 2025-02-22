using Board;
using Move;
using static Piece.Presets;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});
// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
PlayingBoard.MakeMove(Move.Move.FromString("e2-e5"));
PlayingBoard.MakeMove(Move.Move.FromString("d7-d5")); 
PlayingBoard.MakeMove(Move.Move.FromString("e5-d6")); 

PlayingBoard.PrintBoard(true);

// implement en passant and castling
// create an algorythm to find all legal moves
using Board;
using Move;
using static Piece.Presets;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition);
PlayingBoard.MakeMove(Move.Move.Constructor(
    new int[] {4,1},
    new int[] {4,3},
    Empty
    ));

PlayingBoard.PrintBoard(false);
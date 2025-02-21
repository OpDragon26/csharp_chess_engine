using Board;
using Move;
using static Piece.Presets;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition);
PlayingBoard.MakeMove(Move.Move.Constructor(
    new int[] {3,1},
    new int[] {3,3},
    Empty
    ));

PlayingBoard.PrintBoard(true);
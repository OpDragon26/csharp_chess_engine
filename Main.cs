using Board;
using Piece;

// Board.Board PlayingBoard = Board.Presets.StartingBoard.DeepCopy();
Board.Board PlayingBoard = TestCases.InsufficientMaterialBoard.DeepCopy();

// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
//PlayingBoard.MakeMove(Move.Move.FromString("e2-e4"), true);
// Move.Move[] Moves = MoveFinder.Search(PlayingBoard,false);
// Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Pawn, false, Board.Presets.ConvertSquare("e4", false));
// PlayingBoard.MakeMove(Moves[1], false);
Console.WriteLine(PlayingBoard.Status());

PlayingBoard.PrintBoard(false);

// outcome function: ongoing, white won, black won, draw ✓
// stalemate ✓
// draw by insufficient material ✓
// 50 move rule
// draw by threefold repetition
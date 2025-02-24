using Board;
using Piece;
using static HashCodeHelper.HashCodeHelper;

Board.Board PlayingBoard = Board.Presets.StartingBoard.DeepCopy();

PlayingBoard.MakeMove(Move.Move.FromString("g1-f3"), true);
PlayingBoard.MakeMove(Move.Move.FromString("g8-f6"), true);
PlayingBoard.MakeMove(Move.Move.FromString("f3-g1"), true);
PlayingBoard.MakeMove(Move.Move.FromString("f6-g8"), true);
Console.WriteLine(PlayingBoard.Status());

PlayingBoard.MakeMove(Move.Move.FromString("g1-f3"), true);
PlayingBoard.MakeMove(Move.Move.FromString("g8-f6"), true);
PlayingBoard.MakeMove(Move.Move.FromString("f3-g1"), true);
PlayingBoard.MakeMove(Move.Move.FromString("f6-g8"), true);
Console.WriteLine(PlayingBoard.Status());

PlayingBoard.MakeMove(Move.Move.FromString("g1-f3"), true);
PlayingBoard.MakeMove(Move.Move.FromString("g8-f6"), true);
PlayingBoard.MakeMove(Move.Move.FromString("f3-g1"), true);
PlayingBoard.MakeMove(Move.Move.FromString("f6-g8"), true);

// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
// PlayingBoard.MakeMove(Move.Move.FromString("e2-e4"), true);
// Move.Move[] Moves = MoveFinder.Search(PlayingBoard,false);
// Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Pawn, false, Board.Presets.ConvertSquare("e4", false));
// PlayingBoard.MakeMove(Moves[1], false);
Console.WriteLine(PlayingBoard.Status());

PlayingBoard.PrintBoard(false);

// outcome function: ongoing, white won, black won, draw ✓
// stalemate ✓
// draw by insufficient material ✓
// 50 move rule ✓
// draw by threefold repetition ✓
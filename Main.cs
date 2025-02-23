using Board;
using Piece;

Board.Board PlayingBoard = Board.Board.Constructor(Board.Presets.StartingPosition, new bool[] {true,true}, new bool[] {true,true}, new int[] {8,8});

// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
PlayingBoard.MakeMove(Move.Move.FromString("e2-e4"), true);
PlayingBoard.MakeMove(Move.Move.FromString("d7-d5"), true);
PlayingBoard.MakeMove(Move.Move.FromString("e4-d5"), true);
// Move.Move[] Moves = MoveFinder.Search(PlayingBoard,false);
// Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Pawn, false, Board.Presets.ConvertSquare("e4", false));
// PlayingBoard.MakeMove(Moves[1], false);

PlayingBoard.PrintBoard(false);

// prevent illegal moves ✓
// parameter to make illegal moves ✓
// outcome function: ongoing, white won, black won, draw
// 50 move rule and threefold repetition
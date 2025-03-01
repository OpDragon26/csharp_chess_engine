Match.Match match = new Match.Match(side: true, depth: 3, debug: false, notateMoves: false);
match.Play();

//TestNode.SearchBranches(2, true);
//Console.WriteLine(TestNode.GetEval());
//PlayingBoard = TestNode.ChildNodes[19].ChildNodes[15].board.DeepCopy();

// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
// PlayingBoard.MakeMove(Move.Move.FromString("e2-e4"), true);
// Move.Move[] Moves = MoveFinder.Search(PlayingBoard,false);
// Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Pawn, false, Board.Presets.ConvertSquare("e4", false));
// PlayingBoard.MakeMove(Moves[1], false);

// TODO:

// Optimize the method for finding checks, moves, and evaluating positions by storing the positions of the pieces separately from the boards ✓
// Fix false castling ✓?
// Fix castling ✓?
// Improve evaluation function
// - Make weights additive instead of multiplicative ✓
// - And endgame weight tables
// - Make king safety a priority
// - Make the engine like pawn chains
// - Make the engine want to keep its queen near the enemy king
// - Make the engine like open files
// Add alpha-beta pruning ✓
// Add move ordering ✓
// Keep calculating if there are captures availible in the current position ✓
// Add a transposition table
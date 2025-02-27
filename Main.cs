using Board;
using Piece;
using static HashCodeHelper.HashCodeHelper;

Match.Match match = new Match.Match(true, 2);
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

// Create a node class that stores a board, and it's child nodes, and an evaluation ✓
// Make the node be able to find all nodes reachable from itself and pass on the depth ✓
// Make the node be able to find the best move availible ✓
// - Make nodes be able to evaluate their position ✓
// - Make nodes be able to find their evaluation using that of their children and the active side of their boards ✓
// -- Make nodes be able to request their children's evaluations, until the final node which has no children and evaluates its position ✓
// - Pick the best move subjectively based on side ✓
// Optimize the method for finding checks, moves, and evaluating positions by storing the positions of the pieces separately from the boards *
// Fix false castling ✓?
// Create a game class that can ask for moves and ask for a reply from another player or reply with a bot move ✓
// Improve evaluation function
// - Make king safety a priority
// - Make the engine like pawn chains
// - Make the engine want to keep its queen near the enemy king
// Add alpha-beta pruning
// Keep calculating if there are captures availible in the current position

// weight adjustments
// Make rooks not like being on the square they land on when castling, prompting the engine to move them to the center first instead of locking them in with the other rook
// Decrease negative weights for pieces to avoid sacrifices instead of moving to less advantageous, safer squares
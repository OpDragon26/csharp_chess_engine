using Board;
using Piece;
using static HashCodeHelper.HashCodeHelper;

Board.Board PlayingBoard = Board.Presets.StartingBoard.DeepCopy();

Node.Node TestNode = new Node.Node(PlayingBoard);
TestNode.GenerateChildren();
PlayingBoard = TestNode.ChildNodes[19].board.DeepCopy();


// Move.Move.Constructor(new int[] {4,1},new int[] {4,3},Empty) -> e2-e4
// PlayingBoard.MakeMove(Move.Move.FromString("e2-e4"), true);
// Move.Move[] Moves = MoveFinder.Search(PlayingBoard,false);
// Move.Move[] Moves = MoveFinder.SearchPiece(PlayingBoard, PieceType.Pawn, false, Board.Presets.ConvertSquare("e4", false));
// PlayingBoard.MakeMove(Moves[1], false);

PlayingBoard.PrintBoard(false);

// Create a node class that stores a board, and it's child nodes, and an evaluation ✓
// Make the node be able to find all nodes reachable from itself and pass on the depth
// Make the node be able to find the best move availible
// - Make nodes be able to evaluate their position
// - Make nodes be able to find their evaluation using that of their children and the active side of their boards
// -- Make nodes be able to request their children's evaluations, until the final node which has no children and evaluates its position
// Create a game class that can ask for moves and ask for a reply from another player or reply with a bot move

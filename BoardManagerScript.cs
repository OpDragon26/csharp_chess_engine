using BoardManagerInfo;
using Piece;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// Todo:
// Add Zobrist hashing
// Add quiescence search
// - Add QuiescenceSearch function in MoveFinder
// - Add a MinimaxQuiescence function
// Add endgame tables
// Add opening book
// 2229

public class BoardManagerScript : MonoBehaviour
{
    public GameObject[,] Squares = new GameObject[8, 8];
    public GameObject[,] Pieces = new GameObject[8, 8];
    public GameObject[,] Overlays = new GameObject[8, 8];
    
    SquareScript[,] SquareScripts = new SquareScript[8, 8];
    PieceScript[,] PieceScripts = new PieceScript[8, 8];
    OverlayScript[,] OverlayScripts = new OverlayScript[8, 8];
    
    GameObject PromotionSelection;

    public bool Side;
    public int Depth = 2;
    public bool DebugMode;

    public Match.Match match = new Match.Match(false, 2, false, false);

    public BmStatus Status = BmStatus.Idle;
    
    (int,int) Selected = (0,0);
    (int, int) Moved = (0,0);

    private Piece.Piece PromotionPiece = Presets.Empty;
    private bool Frozen;

    public Text DepthLabel;
    public Text StatusLabel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PromotionSelection = GameObject.FindGameObjectWithTag("Promotion");
        PromotionSelection.SetActive(false);
        StatusLabel.gameObject.SetActive(false);

        match.PlayerSide = Side;
        match.Depth = Depth;

        if (Side)
        {
            Status = BmStatus.BotTurn;
        }
        
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                SquareScripts[i,j] = Squares[i,j].GetComponent<SquareScript>();
                SquareScripts[i,j].UpdateTexture((i + j) % 2);
                SquareScripts[i,j].Coords = (j, i);
                
                PieceScripts[i,j] = Pieces[i,j].GetComponent<PieceScript>();
                
                OverlayScripts[i,j] = Overlays[i,j].GetComponent<OverlayScript>();
            }
        }
        
        UpdatePieceTextures();
        
        DepthLabel.text = "Depth: " + (match.Depth + 1).ToString();
        
        if (DebugMode)
        {
            match.board.MakeMove(Move.Move.FromString("g1-f3"), false, true);
            match.board.UnmakeMove();
            match.board.MakeMove(Move.Move.FromString("g1-f3"), false, true);
            UpdatePieceTextures();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!DebugMode)
        { 
            switch (Status)
            {
                case  BmStatus.Idle:
                    Frozen = false;
                break;
                
                case  BmStatus.PlayerTurn:
                    Frozen = false;

                    if (match.board.board[Selected.Item2, Selected.Item1].Role == PieceType.Pawn && BoardManagerInfo.BoardManagerInfo.PromotionSquare(Moved, match.PlayerSide) && PromotionPiece == Presets.Empty)
                    {
                        PromotionSelection.SetActive(true);

                        Status = BmStatus.WaitingForPromotion;
                        break;
                    }
                    
                    // true if the move was legal and it was made on the board
                    bool moveMade = match.MakeMove(new Move.Move(new [] {Selected.Item1, Selected.Item2}, new [] {Moved.Item1, Moved.Item2}, PromotionPiece, 0));
                    PromotionPiece = Presets.Empty;
                    
                    Selected = (8, 8);
                    Moved = (8, 8);

                    if (moveMade)
                    {
                        UpdatePieceTextures();
                        StatusLabel.gameObject.SetActive(true);

                        Status = BmStatus.BotTurn; // switch to the bot's turn
                        Debug.Log("Move successful");
                    }
                    else
                    {
                        Status = BmStatus.Idle; // Keep waiting
                        Debug.Log("Move failed");
                    }

                break;

                case BmStatus.BotTurn:
                    Frozen = false;

                    Debug.Log("Bot move");
                    // Make the bot's move
                    match.MakeBotMove();
                    
                    UpdatePieceTextures();
                    StatusLabel.gameObject.SetActive(false);
                    
                    Status = BmStatus.Idle; // Wait for a player move

                break;
                
                case BmStatus.WaitingForPromotion:
                    Frozen = true;
                break;
                
                case BmStatus.PromotionQueen:
                    Frozen = false;

                    if (match.PlayerSide)
                        PromotionPiece = Presets.B_Queen;
                    else
                        PromotionPiece = Presets.W_Queen;
                    Status = BmStatus.PlayerTurn;
                break;
                
                case BmStatus.PromotionRook:
                    Frozen = false;

                    if (match.PlayerSide)
                        PromotionPiece = Presets.B_Rook;
                    else
                        PromotionPiece = Presets.W_Rook;
                    Status = BmStatus.PlayerTurn;
                break;
                
                case BmStatus.PromotionBishop:
                    Frozen = false;

                    if (match.PlayerSide)
                        PromotionPiece = Presets.B_Bishop;
                    else
                        PromotionPiece = Presets.W_Bishop;
                    Status = BmStatus.PlayerTurn;
                break;
                
                case BmStatus.PromotionKnight:
                    Frozen = false;

                    if (match.PlayerSide)
                        PromotionPiece = Presets.B_Knight;
                    else
                        PromotionPiece = Presets.W_Knight;
                    Status = BmStatus.PlayerTurn;
                break;
                
                case BmStatus.PromotionEmpty:
                    Frozen = false;

                    PromotionPiece = Presets.B_King;
                    Status = BmStatus.PlayerTurn;
                break;
            }
        }
    }

    void UpdatePieceTextures()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                (int,int) coords = BoardManagerInfo.BoardManagerInfo.Switch((i, j), !match.PlayerSide, false); // The *magic function* requires the opposite side for some reason
                
                PieceScripts[i,j].UpdateTexture(match.board.board[coords.Item1, coords.Item2]);
            }
        }
    }

    public void Click((int,int) coords)
    {
        if (match.PlayerSide == match.board.Side && !Frozen)
        {
            (int,int) ocoords = BoardManagerInfo.BoardManagerInfo.Switch(coords, match.PlayerSide, false); 
            // Get the piece using objective coords - match.match.board[ocoords.Item2, coords.Item1]
        
            if (match.board.board[ocoords.Item2, ocoords.Item1].Role != PieceType.Empty &&
                match.board.board[ocoords.Item2, ocoords.Item1].Color == match.PlayerSide)
            {
                // Update the texture of the overlays
                int set = 1 - OverlayScripts[coords.Item2, coords.Item1].TextureIndex;

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        OverlayScripts[i, j].UpdateTexture(0);
                    }
                }
        
                OverlayScripts[coords.Item2, coords.Item1].UpdateTexture(set);
        
                Debug.Log(match.board.board[ocoords.Item2, ocoords.Item1].Role);
                Debug.Log(match.board.board[ocoords.Item2, ocoords.Item1].Color);

                if (Selected == ocoords)
                {
                    Selected = (8,8);
                }
                
                Selected = ocoords;
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        OverlayScripts[i, j].UpdateTexture(0);
                    }
                }

                Moved = ocoords;
            
                Status = BmStatus.PlayerTurn;
            }
        }
    }

    public void Reset(bool color)
    {
        Selected = (0, 0);
        Moved = (0, 0);
        
        PromotionPiece = Presets.Empty;

        if (color)
            Status = BmStatus.BotTurn;
        else
            Status = BmStatus.Idle;
        
        match = new Match.Match(color, Depth, false, false);
        
        UpdatePieceTextures();
    }

    public void DepthIncrease()
    {
        if (match.Depth < 9)
        {
            match.Depth++;
            Depth++;
            DepthLabel.text = "Depth: " + (match.Depth + 1).ToString();
        }
    }

    public void DepthDecrease()
    {
        if (match.Depth > 0)
        {
            match.Depth--;
            Depth--;
            DepthLabel.text = "Depth: " + (match.Depth + 1).ToString();
        }
    }
}

namespace BoardManagerInfo
{
    public static class BoardManagerInfo
    {
        public static (int, int) Switch((int,int) coords, bool perspective, bool debug)
        {
            if (debug)
            {
                return coords;
            }
            
            if (perspective)
            {
                return (7 - coords.Item1, coords.Item2);
            }
            else
            {
                return (coords.Item1, 7 - coords.Item2);
            }
        }

        public static bool PromotionSquare((int,int) coords, bool perspective)
        {
            Debug.Log(coords);

            if (perspective)
            {
                return coords.Item2 == 0;
            }
            else
            {
                return coords.Item2 == 7;
            }
        }
    }

    public enum BmStatus
    {
        Idle,
        PlayerTurn,
        BotTurn,
        WaitingForPromotion,
        PromotionQueen,
        PromotionRook,
        PromotionBishop,
        PromotionKnight,
        PromotionEmpty
    }
}
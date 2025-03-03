using BoardManagerInfo;
using Piece;
using UnityEngine;

// Todo:
// Add castling

public class BoardManagerScript : MonoBehaviour
{
    public GameObject[,] Squares = new GameObject[8, 8];
    public GameObject[,] Pieces = new GameObject[8, 8];
    public GameObject[,] Overlays = new GameObject[8, 8];
    
    SquareScript[,] SquareScripts = new SquareScript[8, 8];
    PieceScript[,] PieceScripts = new PieceScript[8, 8];
    OverlayScript[,] OverlayScripts = new OverlayScript[8, 8];

    public Match.Match match = new Match.Match(false, 2, false, false);

    public BmStatus Status = BmStatus.Idle;
    
    (int,int) Selected = (0,0);
    (int, int) Moved = (0,0);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        switch (Status)
        {
            case  BmStatus.Idle:
            break;
            
            case  BmStatus.PlayerTurn:
                bool MoveMade = match.MakeMove(new Move.Move(new [] {Selected.Item1, Selected.Item2}, new [] {Moved.Item1, Moved.Item2}, Piece.Presets.Empty));

                Selected = (8, 8);
                Moved = (8, 8);

                if (MoveMade)
                {
                    UpdatePieceTextures();

                    Status = BmStatus.BotTurn;
                }
                else
                {
                    Status = BmStatus.Idle;
                }

            break;

            case BmStatus.BotTurn:
                match.MakeBotMove();
                
                UpdatePieceTextures();
                
                Status = BmStatus.Idle;

            break;
        }
    }

    void UpdatePieceTextures()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                (int,int) coords = BoardManagerInfo.BoardManagerInfo.Switch((i, j), !match.PlayerSide, false);
                
                PieceScripts[i,j].UpdateTexture(match.board.board[coords.Item1, coords.Item2]);
            }
        }
    }

    public void Click((int,int) coords)
    {
        if (match.PlayerSide == match.board.Side)
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
    }
}
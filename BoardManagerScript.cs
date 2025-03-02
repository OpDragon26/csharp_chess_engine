using System.Threading;
using Board;
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

    public Match.Match match = new Match.Match(false, 3, false, false);
    
    (int,int) Selected = (0,0);
    
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
        (int,int) Ocoords = BoardManagerInfo.BoardManagerInfo.Switch(coords, match.PlayerSide, false);
        
        if (match.board.board[Ocoords.Item2, Ocoords.Item1].Role != PieceType.Empty &&
            match.board.board[Ocoords.Item2, Ocoords.Item1].Color == match.PlayerSide)
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
        
            // Get the piece using objective coords - match.match.board[Ocoords.Item2, coords.Item1]
        
            Debug.Log(match.board.board[Ocoords.Item2, Ocoords.Item1].Role);
            Debug.Log(match.board.board[Ocoords.Item2, Ocoords.Item1].Color);

            if (Selected == Ocoords)
            {
                Selected = (8,8);
            }
        
            Selected = Ocoords;
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
            
            // If the selected square isn't occupied by a friendly piece, attempt to move to that square
            // If the move was successful, attempt a bot move

            bool MoveMade = match.MakeMove(new Move.Move(new[] {Selected.Item1, Selected.Item2}, new[] {Ocoords.Item1, Ocoords.Item2}, Piece.Presets.Empty));
            UpdatePieceTextures();

            if (match.board.Status() != Outcome.Ongoing)
            {
                Application.Quit();
            }

            if (MoveMade)
            {
                match.MakeBotMove();
                UpdatePieceTextures();
                
                if (match.board.Status() != Outcome.Ongoing)
                {
                    Application.Quit();
                }
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
}
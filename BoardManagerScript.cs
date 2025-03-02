using UnityEngine;

public class BoardManagerScript : MonoBehaviour
{
    public GameObject[,] Squares = new GameObject[8, 8];
    public GameObject[,] Pieces = new GameObject[8, 8];
    public GameObject[,] Overlays = new GameObject[8, 8];
    
    SquareScript[,] SquareScripts = new SquareScript[8, 8];
    PieceScript[,] PieceScripts = new PieceScript[8, 8];
    OverlayScript[,] OverlayScripts = new OverlayScript[8, 8];

    public Match.Match match = new Match.Match(true, 3, false, false);
    
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
        int set = 1 - OverlayScripts[coords.Item2, coords.Item1].TextureIndex;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                OverlayScripts[i, j].UpdateTexture(0);
            }
        }
        
        OverlayScripts[coords.Item2, coords.Item1].UpdateTexture(set);
        
        coords = BoardManagerInfo.BoardManagerInfo.Switch(coords, match.PlayerSide, false);
        
        Debug.Log(match.board.board[coords.Item2, coords.Item1].Role);
        Debug.Log(match.board.board[coords.Item2, coords.Item1].Color);
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
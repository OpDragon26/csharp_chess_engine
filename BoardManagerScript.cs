using UnityEngine;
using Match;

public class BoardManagerScript : MonoBehaviour
{
    public GameObject[,] Squares = new GameObject[8, 8];
    public GameObject[,] Pieces = new GameObject[8, 8];
    public GameObject[,] Overlays = new GameObject[8, 8];
    
    SquareScript[,] SquareScripts = new SquareScript[8, 8];
    PieceScript[,] PieceScripts = new PieceScript[8, 8];

    public Match.Match match = new Match.Match(false, 3, false, false);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                SquareScripts[i,j] = Squares[i,j].GetComponent<SquareScript>();
                SquareScripts[i,j].UpdateTexture((i + j) % 2);
                
                PieceScripts[i,j] = Pieces[i,j].GetComponent<PieceScript>();
                PieceScripts[i,j].UpdateTexture(match.board.board[i, j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdatePieceTextures()
    {
        
    }
}

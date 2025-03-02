using UnityEngine;

public class BoardManagerScript : MonoBehaviour
{
    public GameObject[,] Squares = new GameObject[8, 8];
    public GameObject[,] Pieces = new GameObject[8, 8];
    public GameObject[,] Overlays = new GameObject[8, 8];
    
    SquareScript[,] SquareScripts = new SquareScript[8, 8];
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                SquareScripts[i,j] = Squares[i,j].GetComponent<SquareScript>();
                SquareScripts[i,j].UpdateTexture((i + j) % 2);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

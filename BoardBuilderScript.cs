using UnityEngine;

public class BoardBuilderScript : MonoBehaviour
{
    public GameObject SquarePrefab;
    public GameObject PiecePrefab;
    public GameObject OverlayPrefab;
    public GameObject BitboardVisualiserPrefab;
    public GameObject MaterialVisualiserPrefab;
    
    BoardManagerScript BoardManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(0,0,0);
        
        BoardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManagerScript>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                BoardManager.Squares[i,j] = Instantiate(SquarePrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                BoardManager.Overlays[i,j] = Instantiate(OverlayPrefab, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
                BoardManager.Pieces[i,j] = Instantiate(PiecePrefab, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.identity);
                BoardManager.BitboardVisualisers[i,j] = Instantiate(BitboardVisualiserPrefab, new Vector3(transform.position.x, transform.position.y, -3), Quaternion.identity);

                transform.position = new Vector3(transform.position.x + 5, transform.position.y, 0);
            }
            
            transform.position = new Vector3(0, transform.position.y - 5, 0);
        }
        
        transform.position = new Vector3(-19.8f,-2,0);

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                BoardManager.WMaterialVisualisers[i * 8 + j] = Instantiate(MaterialVisualiserPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                
                transform.position = new Vector3(transform.position.x + 2.2f, transform.position.y, 0);
            }
            transform.position = new Vector3(-19.8f, transform.position.y - 2.2f, 0);
        }
        
        transform.position = new Vector3(-19.8f,-31,0);
        
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                BoardManager.BMaterialVisualisers[i * 8 + j] = Instantiate(MaterialVisualiserPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                
                transform.position = new Vector3(transform.position.x + 2.2f, transform.position.y, 0);
            }
            transform.position = new Vector3(-19.8f, transform.position.y - 2.2f, 0);
        }
    }
}
using UnityEngine;

public class BoardBuilderScript : MonoBehaviour
{
    public GameObject SquarePrefab;
    public GameObject PiecePrefab;
    public GameObject OverlayPrefab;
    
    BoardManagerScript BoardManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManagerScript>();

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                BoardManager.Squares[i,j] = (Instantiate(SquarePrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity));
                BoardManager.Overlays[i,j] = (Instantiate(OverlayPrefab, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity));
                BoardManager.Pieces[i,j] = (Instantiate(PiecePrefab, new Vector3(transform.position.x, transform.position.y, -2), Quaternion.identity));

                transform.position = new Vector3(transform.position.x + 5, transform.position.y, 0);
            }
            
            transform.position = new Vector3(0, transform.position.y - 5, 0);
        }
    }
}
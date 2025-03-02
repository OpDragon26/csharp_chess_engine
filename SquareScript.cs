using UnityEngine;

public class SquareScript : MonoBehaviour
{
    BoardManagerScript BoardManager;
    
    private int TextureIndex = 0;
    public Sprite[] Textures = new Sprite[2];
    public SpriteRenderer SpriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManagerScript>();
    }

    public void UpdateTexture(int texture)
    {
        TextureIndex = texture;
        SpriteRenderer.sprite = Textures[TextureIndex];
    }
}
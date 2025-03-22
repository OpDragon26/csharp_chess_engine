using UnityEngine;

public class SquareScript : MonoBehaviour
{
    public (int, int) Coords = (0, 0);
    private int TextureIndex;
    public Sprite[] Textures = new Sprite[2];
    public SpriteRenderer SpriteRenderer;

    BoardManagerScript BoardManager;

    void Start()
    {
        BoardManager = GameObject.Find("BoardManager").GetComponent<BoardManagerScript>();
    }

    public void UpdateTexture(int texture)
    {
        TextureIndex = texture;
        SpriteRenderer.sprite = Textures[TextureIndex];
    }

    void OnMouseDown()
    {
        BoardManager.Click(Coords);
    }

    void OnMouseEnter()
    {
        BoardManager.HoverEnter(Coords);
    }

    void OnMouseExit()
    {
        BoardManager.HoverExit(Coords);
    }

}
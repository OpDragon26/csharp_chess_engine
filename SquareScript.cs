using UnityEngine;

public class SquareScript : MonoBehaviour
{
    private int TextureIndex = 0;
    public Sprite[] Textures = new Sprite[2];
    public SpriteRenderer SpriteRenderer;

    public void UpdateTexture(int texture)
    {
        TextureIndex = texture;
        SpriteRenderer.sprite = Textures[TextureIndex];
    }
}
using UnityEngine;

public class BitboardVisualiserScript : MonoBehaviour
{
    public int TextureIndex;
    public Sprite[] Textures = new Sprite[3];
    public SpriteRenderer SpriteRenderer;

    public void UpdateTexture(int texture)
    {
        TextureIndex = texture;
        SpriteRenderer.sprite = Textures[TextureIndex];
    }
}

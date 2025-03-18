using UnityEngine;

public class OverlayScript : MonoBehaviour
{
    public int TextureIndex;
    public Sprite[] Textures = new Sprite[5];
    public SpriteRenderer SpriteRenderer;

    public void UpdateTexture(int texture)
    {
        TextureIndex = texture;
        SpriteRenderer.sprite = Textures[TextureIndex];
    }
}

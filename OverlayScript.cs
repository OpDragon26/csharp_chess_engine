using UnityEngine;

public class OverlayScript : MonoBehaviour
{
    public int TextureIndex;
    public Sprite[] Textures = new Sprite[8];
    public SpriteRenderer SpriteRenderer;

    public void UpdateTexture(int texture)
    {
        TextureIndex = texture;
        SpriteRenderer.sprite = Textures[TextureIndex];
    }

    public void HoverEnter()
    {
        if (TextureIndex > 2)
            UpdateTexture(TextureIndex + 2);
    }

    public void HoverExit()
    {
        if (TextureIndex > 3)
            UpdateTexture(TextureIndex - 2);
    }
}

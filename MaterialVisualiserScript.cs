using UnityEngine;

public class MaterialVisualiserScript : MonoBehaviour
{
    public int TextureIndex;
    public Sprite[] Textures = new Sprite[6];
    public SpriteRenderer SpriteRenderer;

    public void UpdateTexture(int texture)
    {
        TextureIndex = texture;
        SpriteRenderer.sprite = Textures[TextureIndex];
    }
}

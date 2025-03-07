using System.Collections.Generic;
using UnityEngine;

public class PromotionDisplayPieceScript : MonoBehaviour
{
    public Sprite[] Textures = new Sprite[2];
    readonly Dictionary<bool, int>  Colors = new Dictionary<bool, int>
    {
        { false, 0 },
        { true, 1 },
    };
    public SpriteRenderer SpriteRenderer;
    BoardManagerScript BoardManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManagerScript>();
        
        SpriteRenderer.sprite = Textures[Colors[BoardManager.match.PlayerSide]];
    }
}

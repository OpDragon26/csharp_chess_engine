using System.Collections.Generic;
using UnityEngine;
using Piece;

public class PieceScript : MonoBehaviour
{
    private int TextureIndex = 0;
    public Sprite[] Textures = new Sprite[13];
    public SpriteRenderer SpriteRenderer;
    
    public void UpdateTexture(Piece.Piece texture)
    {
        TextureIndex = PieceInfo.PieceInfo.PieceTextures[texture];
        SpriteRenderer.sprite = Textures[TextureIndex];
    }
}

namespace PieceInfo
{
    static class PieceInfo
    {
        public static Dictionary<Piece.Piece, int> PieceTextures = new Dictionary<Piece.Piece, int>
        {
            { Presets.Empty, 0 },
            { Presets.B_Pawn, 1 },
            { Presets.W_Pawn, 2 },
            { Presets.B_Rook, 3 },
            { Presets.W_Rook, 4 },
            { Presets.B_Knight, 5 },
            { Presets.W_Knight, 6 },
            { Presets.B_Bishop, 7 },
            { Presets.W_Bishop, 8 },
            { Presets.B_Queen, 9 },
            { Presets.W_Queen, 10 },
            { Presets.B_King, 11 },
            { Presets.W_King, 12 },
        };
    }
}
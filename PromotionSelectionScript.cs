using UnityEngine;
using BoardManagerInfo;

public class PromotionSelectionScript : MonoBehaviour
{
    BoardManagerScript BoardManager;
    GameObject PromotionSelection;

    private BmStatus[] PieceStatus = new[] { BmStatus.PromotionQueen, BmStatus.PromotionRook, BmStatus.PromotionBishop, BmStatus.PromotionKnight, BmStatus.PromotionEmpty };
    public int Index;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoardManager = GameObject.Find("BoardManager").GetComponent<BoardManagerScript>();
        PromotionSelection = GameObject.FindGameObjectWithTag("Promotion");
    }
    void OnMouseDown()
    {
        PromotionSelection.SetActive(false);
        BoardManager.Status = PieceStatus[Index];
    }
}

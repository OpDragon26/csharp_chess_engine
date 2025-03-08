using UnityEngine;

public class QuitButtonScript : MonoBehaviour
{
    public void OnMouseDown()
    {
        Application.Quit();
    }
}
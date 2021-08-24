using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public bool isFull;

    public Sprite emptyHeart;
    public Sprite fullHeart;

    private Image image;

    private void Awake() 
    {
        image = GetComponent<Image>();
    }

    public void SetFull() 
    {
        image.sprite = fullHeart;
        isFull = true;
    }

    public void SetEmpty() 
    {
        image.sprite = emptyHeart;
        isFull = false;
    }
}

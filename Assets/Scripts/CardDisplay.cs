using UnityEngine;
using System.Collections;

public class CardDisplay : MonoBehaviour
{
    public SpriteRenderer render;
    public Sprite frontSprite; 
    public Sprite backSprite;  

    public void Setup(Sprite face, Sprite back)
    {
        render = GetComponent<SpriteRenderer>();
        frontSprite = face;
        backSprite = back;
        render.sprite = backSprite;
    }

    public void FlipCard()
    {
        render.sprite = frontSprite;
    }
}
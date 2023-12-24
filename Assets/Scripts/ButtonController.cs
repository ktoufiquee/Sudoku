using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public int Value;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
    
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }


}

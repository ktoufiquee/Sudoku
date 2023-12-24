using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TileController : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject emptyTile;
    
    public int value;
    
    public void InitColor(bool isOffset)
    {
        spriteRenderer.color = isOffset ? offsetColor : baseColor;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;
    }

    public void SetHighlight(bool isHighlighted)
    {
        highlight.SetActive(isHighlighted);
    }

    public void SetEmpty()
    {
        emptyTile.SetActive(true);
        spriteRenderer.enabled = false; 
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMarker : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void Show(Sprite sprite, Color color)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
}

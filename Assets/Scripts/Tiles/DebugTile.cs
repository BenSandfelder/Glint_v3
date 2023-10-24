using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugTile : MonoBehaviour
{
    private GridObject gridObject;
    [SerializeField]
    private TextMeshPro text;
    [SerializeField]
    private SpriteRenderer frame;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
        List<Actor> gridActor = gridObject.GetActors();
        text.text = Mathf.Round(gridObject.GetGridVector().x) + "," + Mathf.Round(gridObject.GetGridVector().y);
    }

    public GridObject GetGridObject()
    {
        return gridObject;
    }

    public void ShowObject()
    {
        text.enabled = true;
        frame.enabled = true;
    }

    public void HideObject()
    {
        text.enabled = false;
        frame.enabled = false;
    }
}

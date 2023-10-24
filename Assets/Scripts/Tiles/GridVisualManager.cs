using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualManager : MonoBehaviour
{
    public static GridVisualManager Instance;

    [SerializeField] private Transform gridVisualPrefab;
    [SerializeField] private List<GridVisualType> gridVisualTypes;
    private Vector2Int gridDimensions;
    private TileMarker[,] tileMarkerGrid;

    public enum GridVisualEffect { Point, Blue_Outline, Red_Outline, Blue_Solid, Red_Solid }

    [Serializable]
    //This struct holds the type, sprite, and color of each grid visual effect. Each enum needs a corresponding struct, which is set in the editor.
    public struct GridVisualType
    {
        public GridVisualEffect effect;
        public Sprite sprite;
        public Color color;
    }

    private void Awake()
    {
        //Singleton Pattern
        if (Instance != null)
        {
            Debug.LogError("Multiple Grid Visual Managers present in scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        gridDimensions = LevelGrid.instance.GetGridDimensions();
        tileMarkerGrid = new TileMarker[gridDimensions.x, gridDimensions.y];

        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                Vector2Int gridVector = new Vector2Int(x, y);
                if (LevelGrid.instance.CheckTile(gridVector))
                {
                    Transform tileMarker = Instantiate(gridVisualPrefab, LevelGrid.instance.GetWorldPosition(gridVector), Quaternion.identity);
                    tileMarkerGrid[x, y] = tileMarker.GetComponent<TileMarker>();
                }
            }
        }

        //Register as a listener for the ActorActionSystem and the LevelGrid.
        ActorActionSystem.Instance.OnActionSelected += ActorActionSystem_OnActionChanged;
        LevelGrid.instance.OnAnyActorMove += LevelGrid_OnAnyActorMove;
    }

    private void LevelGrid_OnAnyActorMove(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void ActorActionSystem_OnActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridVectors()
    {
        foreach(TileMarker tile in tileMarkerGrid)
        {
            if (tile != null) tile.Hide();
        }
    }

    public void ShowGridVectors(List<Vector2Int> vectorsToShow, GridVisualEffect gridVisualEffect)
    {
        foreach (Vector2Int vector in vectorsToShow)
        {
            tileMarkerGrid[vector.x, vector.y].Show(GetGridSprite(gridVisualEffect), GetGridColor(gridVisualEffect));
        }
    }

    public void UpdateGridVisual()
    {
        HideAllGridVectors();

        BaseAction selectedAction = ActorActionSystem.Instance.GetSelectedAction();
        switch (selectedAction)
        {
            case AttackAction attackAction:
                ShowGridVectors(attackAction.GetAllVectorsInRange(selectedAction.GetActor().GetGridPosition()), attackAction.GetRangeFinderEffect());
                ShowGridVectors(attackAction.GetValidGridVectors(), attackAction.GetVisualEffect());
                break;
            default:
                ShowGridVectors(selectedAction.GetValidGridVectors(), selectedAction.GetVisualEffect());
                break;

        }
        
    }

    private Sprite GetGridSprite(GridVisualEffect effectToShow)
    {
        foreach (GridVisualType type in gridVisualTypes)
        {
            if (type.effect == effectToShow)
            {
                return type.sprite;
            }
        }
        Debug.LogError("GridVisualType " + effectToShow + " not found.");
        return null;
    }

    private Color GetGridColor(GridVisualEffect effectToShow)
    {
        foreach (GridVisualType type in gridVisualTypes)
        {
            if (type.effect == effectToShow)
            {
                return type.color;
            }
        }
        Debug.LogError("GridVisualType " + effectToShow + " not found.");
        return Color.black;
    }
}

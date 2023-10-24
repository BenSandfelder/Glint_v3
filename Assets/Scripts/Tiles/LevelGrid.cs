using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid instance { get; private set; }
    private GridSystem<GridObject> gridSystem;
    
    [SerializeField] //NOTE: remove this one after fixing everything.
    private GameObject gridHolder;
    [SerializeField]
    private GameObject debugTile;
    [SerializeField]
    private Tilemap ground;
    [SerializeField]
    private Tilemap walls;
    [SerializeField] LayerMask actorsLayer;

    public event EventHandler OnAnyActorMove;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple level grids in scene!");
            Destroy(gameObject);
            return;
        } else
        {
            instance = this;
        }

        //Compress the ground tilemap to its minimum size.
        ground.CompressBounds();

        //Create a vector2 based on the ground tilemap's size.
        Vector2Int gridBounds = new Vector2Int(ground.size.x, ground.size.y);

        //Create the level grid.
        gridSystem = new GridSystem<GridObject>(gridBounds.x, gridBounds.y, 1, 1.5f, gridHolder, ground, walls, (GridSystem<GridObject> g, Vector2Int gridVector) => new GridObject(g, gridVector));
    }

    private void Start()
    {
        //During Start, create the debug tiles.
        //gridSystem.CreateDebugObject(debugTile);
    }

    #region Grid Functions
    // => syntax is a "Lambda expression;" the same as brackets and a return function.
    public Vector2Int GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(Vector2Int gridVector) => gridSystem.GetWorldPosition(gridVector);
    public Vector3 GetCenteredWorldPosition(Vector2Int gridVector) => gridSystem.GetCenteredWorldPosition(gridVector);
    public bool CheckTile(Vector2Int gridVector) => gridSystem.CheckTile(gridVector);

    public bool HasActorOrObject(Vector2Int gridVector)
    {
        //Finds the gridObject at the input vector, returns true if it's already occupied.
        GridObject gridObject = gridSystem.GetGridObject(gridVector);
        return gridObject.HasAnyUnit();
    }

    public Actor GetActor (Vector2Int gridVector)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridVector);
        return gridObject.GetActor();
    }

    public Vector2Int GetGridDimensions()
    {
        return new Vector2Int(ground.size.x, ground.size.y);
    }
    #endregion Grid Functions
    #region Actor Controls

    public void AddActorAtPosition(Vector2Int gridVector, Actor actor)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridVector);
        gridObject.AddActor(actor);
    }

    public List<Actor> GetActorsAtPosition(Vector2Int gridVector)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridVector);
        return gridObject.GetActors();
    }

    public void ClearActorFromPosition(Actor actor, Vector2Int gridVector)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridVector);
        gridObject.RemoveActor(actor);
    }

    public void UpdateActorPosition(Actor actor, Vector2Int fromTile, Vector2Int toTile)
    {
        //Remove an actor from a tile's record of Actors currently occupying it.
        ClearActorFromPosition(actor, fromTile);
        //Add the actor to the destination tile's record.
        AddActorAtPosition(toTile, actor);

        //Announce that a unit has moved.
        OnAnyActorMove?.Invoke(this, EventArgs.Empty);
    }
    #endregion Actor Controls
    
}

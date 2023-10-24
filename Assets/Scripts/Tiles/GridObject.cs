using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private Vector2 gridVector;
    private List<Actor> actors;

    public GridObject(GridSystem<GridObject> gridSystem, Vector2 gridVector)
    {
        this.gridSystem = gridSystem;
        this.gridVector = gridVector;
        actors = new List<Actor>();
    }

    public Vector2 GetGridVector()
    {
        return gridVector;
    }

    public void AddActor(Actor actor)
    {
        actors.Add(actor);
    }

    public void RemoveActor(Actor actor)
    {
        actors.Remove(actor);
    }

    public List<Actor> GetActors()
    {
        return actors;
    }

    public bool HasAnyUnit()
    {
        //If there are more than 0 actors in the tile, return true.
        return actors.Count > 0;
    }

    public Actor GetActor()
    {
        if (HasAnyUnit()) return actors[0];
        else return null;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvObject : MonoBehaviour, IInteract
{
    public string objectName;
    public SpriteRenderer image;
    public enum interactions { Attack, Select, Examine, Disarm, Drop, Pickup, Push, Search, Talk, Unlock, Use }
    public interactions defaultInteraction;
    private Vector2Int gridVector;

    public interactions[] allowedInteractions;
    
    //Bool determining whether the full description is shown or not. True by default.
    public bool identified = true;
    //A general description shown when object is examined while unidentified.
    [TextArea(2, 4)]
    public string description;

    //Extra info appended to the description if the object is identified.
    [TextArea(2, 4)]
    public string fullDescription;

    public abstract void Attack();
    public abstract void Select();
    public abstract void Examine();
    public abstract void Disarm();
    public abstract void Drop();
    public abstract void Pickup();
    public abstract void Push();
    public abstract void Search();
    public abstract void Talk();
    public abstract void Unlock();
    public abstract void Use();

    public Vector2Int GetGridVector()
    {
        return gridVector;
    }
}

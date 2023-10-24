using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Item", order = 1)]
public class Item : ScriptableObject
{
    //unknwnName used while the item is unidentified.
    public string itemName, unknwnName;
    public enum rarity { common, uncommon, rare, very_rare, legendary, artifact, plot }
    public rarity itemRarity;
    public Sprite icon;
    public Color tint;

    //Is the item a commodity? (Commodities such as coins retain their full value when bartered.)
    public bool commodity = false;
    //Is the item droppable? (True by default).
    public bool droppable = true;
    //Has the item been identified? (True by default.)
    public bool identified = true;

    //Ints for trade value, encumbrance, and stacking. Set maxStackSize to 1 if it isn't stackable.
    public float value, coinWeight, maxStackSize;

    //A general description shown when item is examined while unidentified.
    [TextArea(2, 4)]
    public string description;

    //Extra info appended to the description if the item is identified.
    [TextArea(2, 4)]
    public string fullDescription;

    public void Drop()
    {

    }
}

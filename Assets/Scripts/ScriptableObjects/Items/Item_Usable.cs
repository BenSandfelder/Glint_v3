using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Usable", order = 3)]
public class Item_Usable : Item
{
    //How many times can the item be used?
    int numUses;

    //Destroy the item when it's out of uses?
    bool consumable;

    //How quickly does the item recharge?
    enum interval { never, turn, day, week}
    interval rechargeInterval;

    public void Use()
    {

    }

    public void Recharge()
    {

    }
}

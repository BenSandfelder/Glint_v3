using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Equipment", order = 2)]
public class Item_Equip : Item
{
    public enum equipType {head, body, mainHand, offHand, back, hands, belt, feet }
    public equipType equipmentType;
    [SerializeField]

    public void Equip(Actor a)
    {

    }

    public void Unequip(Actor a)
    {

    }
}

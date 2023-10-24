using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSlot : ItemSlot
{
    public enum equipType { head, body, mainHand, offHand, back, hands, belt, feet };
    public equipType slotType;

    public EquipSlot(Item_Equip _equip, int _quantity, equipType _type)
    {
        item = _equip;
        itemCount = _quantity;
        slotType = _type;
    }

    public EquipSlot(Item_Equip _equip, int _quantity, int _type)
    {
        item = _equip;
        itemCount = _quantity;
        switch (_type)
        {
            case 0: slotType = equipType.head; break;
            case 1: slotType = equipType.body; break;
            case 2: slotType = equipType.mainHand; break;
            case 3: slotType = equipType.offHand; break;
            case 4: slotType = equipType.back; break;
            case 5: slotType = equipType.hands; break;
            case 6: slotType = equipType.belt; break;
            case 7: slotType = equipType.feet; break;
            default: slotType = equipType.mainHand; break;
        }
    }
  
}

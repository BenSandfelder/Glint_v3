using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot
{
    [SerializeField]
    public Item item;
    [SerializeField]
    public int itemCount;

    //Empty constructor
    public ItemSlot()
    {
        item = null;
        itemCount = 0;
    }
    //Item-based constructor
    public ItemSlot(Item newItem, int count)
    {
        item = newItem;
        itemCount = count;
    }

    //Item Slot based constructor.
    public ItemSlot(ItemSlot slot)
    {
        item = slot.item;
        itemCount = slot.itemCount;
    }

    public Item GetItem()
    {
        return item;
    }

    public int GetQuantity()
    {
        return itemCount;
    }

    public void AddItem(Item _item, int _quantity)
    {
        item = _item;
        itemCount = _quantity;
    }

    public void AddQuantity(int _quantity)
    {
        itemCount += _quantity;
    }

    public void SubQuantity(int _quantity)
    {
        itemCount -= _quantity;

        if (itemCount <= 0) EmptySlot();
    }

    public void EmptySlot()
    {
        item = null;
        itemCount = 0;
    }

}

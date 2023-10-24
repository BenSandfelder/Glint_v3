using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrade
{
    bool AddItem(Item item, int quantity);
    bool RemoveItem(Item item, int quantity);
    ItemSlot FindFirstEmpty();
    ItemSlot GetItemSlot(Item item);
    ItemSlot[] SendInventory(ItemSlot[] inventoryToSend);
    void ReceiveInventory(ItemSlot[] inventoryToReplace, ItemSlot[] contents);
}

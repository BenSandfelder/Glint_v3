using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorInventory : MonoBehaviour, ITrade
{
    public event EventHandler<ItemSlot[]> OnItemsChanged;
    public ItemSlot[] inventoryItems = new ItemSlot[24];
    public EquipSlot[] equippedItems = new EquipSlot[8];

    [SerializeField] private Item[] startingItems;
    [SerializeField] private Item_Equip[] startingEquipment;

    private void Awake()
    {
        //Initialize inventory slots.
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            inventoryItems[i] = new ItemSlot();
        }

        //Add starting items to the Actor's inventory.
        for (int i = 0; i < startingItems.Length; i++)
        {
            if (startingItems[i] != null)
                AddItem(startingItems[i], 1);
        }

        //Initialize equipment slots.
        for (int i = 0; i < equippedItems.Length; i++)
        {
            equippedItems[i] = new EquipSlot(null, 0, i);
        }

        //Add starting equipment to the actor's currently equipped items.
        for (int i = 0; i < startingEquipment.Length; i++)
        {
            if (startingEquipment[i] != null)
                equippedItems[i].AddItem(startingEquipment[i], 1);
        }
    }

    //Add items to the inventory.
    public bool AddItem(Item item, int quantity)
    {
        //Search the inventory for the item.
        ItemSlot foundSlot = GetItemSlot(item);

        //If we already have the item, and there is room for more...
        if (foundSlot != null)
        {
            //If we already have the item, and there is not room for more...
            if (foundSlot.GetQuantity() + quantity > item.maxStackSize)
            {
                Debug.Log("Not enough room in this slot for " + quantity + item.itemName + "s");
                return false;
            } else
            {
                //Debug.Log("Added " + quantity + item.itemName + "s to inventory.");
                foundSlot.AddQuantity(quantity);
            }
        }
        else
        {
            //If we didn't find a slot containing the item, add it to the first available slot.
            Debug.Log("Added " + quantity + item.itemName + "s to an empty inventory slot.");
            foundSlot = FindFirstEmpty();
            foundSlot.AddItem(item, quantity);
        }
        return true;
    }

    //Remove items from the inventory.
    public bool RemoveItem(Item item, int quantity)
    {
        //Check for the item to be removed.
        ItemSlot temp = GetItemSlot(item);

        if (temp != null)
        {
            //If the item's quantity is >1, then reduce the quantity by the desired amount.
            if (temp.GetQuantity() > quantity)
                temp.SubQuantity(quantity);
            else
            {
                //If there aren't enough items to remove in that slot, reduce Quantity by the slot's item count...
                quantity -= temp.GetQuantity();
                //...then empty the slot.
                temp.EmptySlot();

                //If quantity is still greater than 0, run this function again (search for another slot containing that item).
                if (quantity > 0)
                {
                    //The function will keep removing items until quantity = 0 or it returns false.
                    RemoveItem(item, quantity);
                }
                else return true;

            }
            //NOTE: if you tell an inventory to remove more than it has, it will remove as many as possible before returning false.
        } else { return false; }
        return true;
    }

    //Find the first available slot in the inventory.
    public ItemSlot FindFirstEmpty()
    {
        //Search the inventory for the first empty slot.
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i].GetItem() == null)
                return inventoryItems[i];
        }
        //If the loop completes, there are no empty slots remaining. Return null.
        return null;
    }

    //Search the entire inventory for an item.
    public ItemSlot GetItemSlot(Item item)
    { 
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i].GetItem() == item)
            {
                //... and return the first slot that has it.
                return inventoryItems[i];
            }
        }

        //If the for loop completes, the inventory does not contain the item. Return null.
        return null;
    }

    //Send the inventory to another script.
    public ItemSlot[] SendInventory(ItemSlot[] inventoryToSend)
    {
        return inventoryToSend;
    }

    //Completely replace all contents of this inventory with a new inventory from another source.
    public void ReceiveInventory(ItemSlot[] inventoryToReplace, ItemSlot[] contents)
    {
        for (int i = 0; i < inventoryToReplace.Length; i++)
        {
            if (contents[i].GetItem() != null)
                inventoryToReplace[i].AddItem(contents[i].GetItem(), contents[i].GetQuantity());
            else
            {
                inventoryToReplace[i].EmptySlot();
            }
        }
    }
}

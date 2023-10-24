using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryMenu : MonoBehaviour, ITrade
{
    public static InventoryMenu instance;
    public Actor activeCharacter;
    [SerializeField] private ActorInventory activeInventory;
    public event EventHandler OnItemsChanged;

    public ItemSlot[] itemSlots;
    public ItemSlot[] equipSlots;

    [SerializeField]
    private Color[] rarityColors = new Color[8];

    [SerializeField] private GameObject inventoryHolder;
    [SerializeField] private GameObject[] inventorySlots;
    [SerializeField] private GameObject equipmentHolder;
    [SerializeField] private GameObject[] equipmentSlots;

    [SerializeField]
    private Sprite[] defaultEquipIcons = new Sprite[8];

    private ItemSlot originalSlot;
    private ItemSlot movingSlot;
    private ItemSlot tempSlot;
    private EquipSlot originalEquip;
    private Item_Equip movingEquip;
    private bool isMovingItem;

    void Awake()
    {
        //Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        //Initialize the inventory slot GameObjects.
        inventorySlots = new GameObject[inventoryHolder.transform.childCount];
        //Connect each GameObject in inventorySlots to its corresponding inventorySlot.
        for (int i = 0; i < inventoryHolder.transform.childCount; i++)
        {
            inventorySlots[i] = inventoryHolder.transform.GetChild(i).gameObject;
        }

        //Initialize the equipment slot GameObjects.
        equipmentSlots = new GameObject[equipmentHolder.transform.childCount];
        for (int i = 0; i < equipmentHolder.transform.childCount; i++)
        {
            equipmentSlots[i] = equipmentHolder.transform.GetChild(i).gameObject;
        }

        //Initialize item slots.
        itemSlots = new ItemSlot[inventorySlots.Length];
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i] = new ItemSlot();
        }

        //Initialize equipment slots.
        equipSlots = new ItemSlot[equipmentSlots.Length];
        for (int i = 0; i < equipSlots.Length; i++)
        {
            equipSlots[i] = new EquipSlot(null, 0, i);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Register as a listener for the OnActorSelected event.
        ActorActionSystem.Instance.OnActorSelected += ActorActionSystem_OnActorSelected;
        
        activeCharacter = ActorActionSystem.Instance.GetSelectedActor();
        //Get a reference to the active character's inventory.
        activeInventory = activeCharacter.gameObject.GetComponent<ActorInventory>();

        //Fill item slots by grabbing the active character's inventory and equipment.
        ReceiveInventory(itemSlots, activeInventory.SendInventory(activeInventory.inventoryItems));
        ReceiveInventory(equipSlots, activeInventory.SendInventory(activeInventory.equippedItems));
    }

    //When OnLeftClick is called, picks up or places a held item.
    public void OnLeftClick()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot != null)
        {
            if (isMovingItem) EndItemMove();
            else BeginItemMove();
        }

    }

    //When OnRightClick is called, takes half a stack or places one held item.
    public void OnRightClick()
    {
        originalSlot = GetClosestSlot();
        if (originalSlot != null)
        {
            if (isMovingItem) PlaceOnlyOne();
            else BeginSplitStack();
        }
    }

    #region Item Utilities
    //Adds items to the inventory.
    public bool AddItem(Item item, int quantity)
    {
        //Search the inventory for the item.
        ItemSlot foundSlot = GetItemSlot(item);

        //If we already have the item, and there is room for more...
        if (foundSlot.GetItem() != null && item.maxStackSize > 1)
        {
            Debug.Log("Added " + quantity + item.itemName + "s to inventory.");
            foundSlot.AddQuantity(quantity);
        }
        //If we already have the item, and there is not room for more...
        else if (foundSlot.GetItem() != null && foundSlot.GetQuantity() + quantity > item.maxStackSize)
        {
            Debug.Log("Not enough room in this slot for " + quantity + item.itemName + "s");
            return false;
        }
        else
        {
            Debug.Log("Added " + quantity + item.itemName + "s to an empty inventory slot.");
            foundSlot = FindFirstEmpty();
            foundSlot.AddItem(item, quantity);
        }

        UpdateInventory();
        return true;
    }

    //Removes items from the inventory.
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
        }
        else { return false; }
        return true;
    }

    //Finds an item slot near a mouse click.
    private ItemSlot GetClosestSlot()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (Vector2.Distance(inventorySlots[i].transform.position, Input.mousePosition) <= 64)
            {
                Debug.Log("Closest slot: " + itemSlots[i]);
                return itemSlots[i];
            }
        }

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (Vector2.Distance(equipmentSlots[i].transform.position, Input.mousePosition) <= 64)
            {
                Debug.Log("Closest slot: " + equipSlots[i]);
                return equipSlots[i];
            }
        }

        Debug.Log("No slot clicked.");
        return null;
    }

    //Searches the inventory for an item.
    public ItemSlot GetItemSlot(Item item)
    {
        //Search the entire inventory for the item...
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].GetItem() == item)
            {
                //... and return the first slot that has it.
                return itemSlots[i];
            }
        }

        //If the for loop completes, the inventory does not contain the item.
        return null;
    }

    //Finds the first empty slot in the inventory.
    public ItemSlot FindFirstEmpty()
    {
        //Search the inventory for the first empty slot.
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].GetItem() == null)
                return itemSlots[i];
        }
        //If the loop completes, there are no empty slots remaining.
        return null;
    }

    //Sends the inventory to another script.
    public ItemSlot[] SendInventory(ItemSlot[] inventoryToSend)
    {
        UpdateInventory();
        return inventoryToSend;
    }

    //Receives an updated inventory from another script.
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

        UpdateInventory();
    }

    public void ChangeActiveCharacter(Actor actor, ActorInventory actorInventory)
    {
        //Send the latest version of the actor's current inventory back to that actor.
        if (activeCharacter != null)
        {
            activeInventory.ReceiveInventory(activeInventory.inventoryItems, itemSlots);
            activeInventory.ReceiveInventory(activeInventory.equippedItems, equipSlots);
        }

        //Change the active character.
        activeCharacter = actor;
        activeInventory = actorInventory;

        //Update the InventoryMenu UI with the new inventory.
        ReceiveInventory(itemSlots, actorInventory.inventoryItems);
        ReceiveInventory(equipSlots, actorInventory.equippedItems);
    }

    //Updates all itemSlots held by this inventory.
    public void UpdateInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (itemSlots[i].GetItem() != null)
            {
                //Enable the frame and the item sprite, set them, and change their color.
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = itemSlots[i].GetItem().icon;
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().color = itemSlots[i].GetItem().tint;
                inventorySlots[i].GetComponent<HoverTip>().enabled = true;
                inventorySlots[i].GetComponent<HoverTip>().tipToShow = itemSlots[i].GetItem().itemName;

                //Change the frame color based on the item's rarity. +1 because 0 = "unequipped gray."
                inventorySlots[i].GetComponent<Image>().color = rarityColors[(int)itemSlots[i].GetItem().itemRarity + 1];

                //Ternary: if the max slot size is greater than 1, enable the item count text and set it.
                if (itemSlots[i].GetItem().maxStackSize > 1 && itemSlots[i].GetQuantity() > 1)
                {
                    inventorySlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                    inventorySlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = itemSlots[i].GetQuantity().ToString();
                }
                else
                {
                    //If the max slot size is 1, hide the item count text.
                    inventorySlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
            else
            {
                //If the item in the slot is null, reset it to its default traits.
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                inventorySlots[i].GetComponent<HoverTip>().enabled = false;
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                inventorySlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                inventorySlots[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                inventorySlots[i].GetComponent<Image>().color = rarityColors[0];
            }
        }

        UpdateEquipment();
        //Announce that items have been changed.
        OnItemsChanged?.Invoke(this, null);
    }

    //Updates all equipSlots held by this inventory.
    public void UpdateEquipment()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            try
            {
                //Set the image for each icon in the Equip menu.
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = equipSlots[i].GetItem().icon;
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().color = equipSlots[i].GetItem().tint;
                equipmentSlots[i].GetComponent<Image>().color = rarityColors[(int)equipSlots[i].GetItem().itemRarity +1];

                //Disable count text for non-stackable items.
                equipmentSlots[i].transform.GetChild(1).GetComponent<TMP_Text>().enabled = equipSlots[i].GetItem().maxStackSize > 1 ? true : false;

                if (equipSlots[i].GetItem().maxStackSize > 1 && equipSlots[i].GetQuantity() > 1)
                {
                    equipmentSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                    equipmentSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = equipSlots[i].GetQuantity().ToString();
                }
                else
                {
                    equipmentSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
            catch
            {
                //If it fails, reset Equip slot to default icon and color, disable text.
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = defaultEquipIcons[i];
                equipmentSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                equipmentSlots[i].transform.GetChild(0).GetComponent<Image>().color = rarityColors[0];
                equipmentSlots[i].GetComponent<Image>().color = rarityColors[0];
            }
        }
    }

    public void ActorActionSystem_OnActorSelected(object sender, EventArgs args)
    {
        //Get a reference to the currently selected Actor.
        Actor selectedActor = ActorActionSystem.Instance.GetSelectedActor();
        //Get a reference to that actor's inventory.
        ActorInventory selectedInventory = selectedActor.gameObject.GetComponent<ActorInventory>();

        //Send that info to the ChangeActiveCharacter script.
        ChangeActiveCharacter(selectedActor, selectedInventory);
    }
    #endregion Item Utilities

    #region Moving Items
    //Used to pick up all items in an item slot.
    private bool BeginItemMove()
    {
        //If the clicked slot does not contain an item, return false.
        if (originalSlot.GetItem() == null) return false;

        //Begin moving the item.
        movingSlot = new ItemSlot(originalSlot.GetItem(), originalSlot.GetQuantity());

        //Empty the clicked slot.
        originalSlot.EmptySlot();

        isMovingItem = true;
        UpdateInventory();
        FollowMouse.instance.UpdatePointer(movingSlot.GetItem().icon, movingSlot.GetItem().tint);
        return true;
    }

    private bool BeginSplitStack()
    {
        if (originalSlot.GetItem() == null) return false;

        //Pick up half the items in a slot (rounding down).
        movingSlot = new ItemSlot(originalSlot.GetItem(), Mathf.FloorToInt(originalSlot.GetQuantity() / 2f));
        //Remove half the items from the original slot (rounding down).
        originalSlot.SubQuantity(Mathf.FloorToInt(originalSlot.GetQuantity() / 2f));

        //If there's only 1 item in the slot (thus, taking half reduces it to 0), then empty the slot.
        if (originalSlot.GetQuantity() < 1)
        {
            originalSlot.EmptySlot();
            FollowMouse.instance.ResetPointer();
        }

        isMovingItem = true;
        UpdateInventory();
        //Update the cursor.
        FollowMouse.instance.UpdatePointer(movingSlot.GetItem().icon, movingSlot.GetItem().tint);
        return true;
    }

    private bool PlaceOnlyOne()
    {
        //Return false if the slot clicked is an equipment slot, and the item held isn't compatible with that slot.
        if (!CheckIfEquipment()) return false;

        //Does not allow dropping items into slots containing different items or unstackable items.
        if (originalSlot.GetItem() != null && movingSlot.GetItem() != originalSlot.GetItem())
        {
            Debug.Log("Can't place one item in a slot containing other items.");
            return false;
        }

        //Does not allow placing items in full slots.
        else if (movingSlot.GetItem() == originalSlot.GetItem() && originalSlot.GetQuantity() >= movingSlot.GetItem().maxStackSize)
        {
            Debug.Log("Can't place one item in a full slot.");
            return false;

            //If the slot contains the same item but isn't full, increase the quantity.
        } else if (movingSlot.GetItem() == originalSlot.GetItem() && originalSlot.GetQuantity() + 1 <= movingSlot.GetItem().maxStackSize)
        {
            originalSlot.AddQuantity(1);
            //In any other case (I.E, the slot clicked is empty), place the item in that slot.
        } else
        {
            originalSlot.AddItem(movingSlot.GetItem(), +1);
        }

        //Place one item.
        movingSlot.SubQuantity(1);

        Debug.Log("Placed 1 item in slot.");

        //If there are no items remaining in hand, then we are no longer moving items.
        if (movingSlot.GetQuantity() < 1)
        {
            isMovingItem = false;
            movingSlot.EmptySlot();
            FollowMouse.instance.ResetPointer();
        }
        else
        {
            isMovingItem = true;
        }

        UpdateInventory();
        return true;
    }

    private bool EndItemMove()
    {
        if (!CheckIfEquipment()) return false;
        //Does original slot contain an item already?
        if (originalSlot.GetItem() != null)
        {
            //Check if the item in hand is the same as the item in the clicked slot.
            if (originalSlot.GetItem() == movingSlot.GetItem())
            {
                if (originalSlot.GetQuantity() + movingSlot.GetQuantity() <= originalSlot.GetItem().maxStackSize)
                {
                    //If items are the same and stackable, increase the quantity.
                    Debug.Log("Increasing quantity.");
                    originalSlot.AddQuantity(movingSlot.GetQuantity());
                }
                else
                {
                    //Items are not stackable, attempt fails.
                    Debug.Log("Items are not stackable.");
                    return false;
                }
            }
            else //If the items are not the same...
            {
                //...swap item in hand with item in slot.
                Debug.Log("Swapping items.");
                tempSlot = new ItemSlot (originalSlot.GetItem(), originalSlot.GetQuantity());
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());
                UpdateInventory();
                FollowMouse.instance.UpdatePointer(movingSlot.GetItem().icon, movingSlot.GetItem().tint);
                return true;
            }
        }
        else //if the slot is empty...
        {
            //Place the item in an available empty slot instead.
            Debug.Log("Placing item.");
            originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
        }

        isMovingItem = false;
        UpdateInventory();
        FollowMouse.instance.ResetPointer();
        return true;
    }

    //Returns false if the input slot is an equipment slot, but not the same kind of equipment as the item being compared.
    bool CheckIfEquipment()
    {
        if (originalSlot is EquipSlot)
        {
            //Attempt to cast originalSlot and movingSlot's item as the more specific EquipSlot and AdvEquipment classes.
            try
            {
                originalEquip = originalSlot as EquipSlot;
                movingEquip = movingSlot.GetItem() as Item_Equip;

                //Convert type enum to ints and compare them. (Head = 0, Body = 1, etc.)
                if ((int)movingEquip.equipmentType != (int)originalEquip.slotType)
                {
                    //If the slots are for different types of equipment, reject the swap.
                    Debug.Log("Cannot place" + movingSlot.GetItem().itemName + " in " + originalEquip.slotType + " slot.");
                    return false;
                }
            }
            catch //If the casting fails... (typically because the item is not an equippable type) ...return false.
            {
                Debug.Log(movingSlot.GetItem().itemName + " is not equippable.");
                return false;
            }
        }

        return true;
    }

    #endregion Moving Items
}

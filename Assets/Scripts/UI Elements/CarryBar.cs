using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarryBar : MonoBehaviour
{
    [SerializeField]
    private Slider encumbranceBar;
    [SerializeField]
    private TextMeshProUGUI encumbranceText;
    [SerializeField]
    private TextMeshProUGUI encumbranceNumber;

    public int encumbranceLimit = 1600;
    private float lightLoad;
    private float mediumLoad;
    private float heavyLoad;

    public enum encumbranceLevels {none, light, moderate, heavy, over}
    public encumbranceLevels encumbranceLevel;

    // Start is called before the first frame update
    void Awake()
    {
        lightLoad = encumbranceLimit / 4;
        mediumLoad = lightLoad * 2;
        heavyLoad = lightLoad * 3;

        encumbranceBar.maxValue = encumbranceLimit;
    }

    private void Start()
    {
        InventoryMenu.instance.OnItemsChanged += InventoryMenu_OnItemsChanged;
    }

    private void InventoryMenu_OnItemsChanged(object sender, EventArgs e)
    {
        CalculateEncumbrance();
    }

    // Calculates the coin weight of all items a character has equipped and in their inventory.
    public void CalculateEncumbrance()
    {
        float currentLoad = 0;


        for (int i = 0; i < InventoryMenu.instance.itemSlots.Length; i++)
        {
            try { currentLoad += (InventoryMenu.instance.itemSlots[i].GetItem().coinWeight * InventoryMenu.instance.itemSlots[i].GetQuantity()); } catch { }
            
        }

        for (int i = 0; i < InventoryMenu.instance.equipSlots.Length; i++)
        {
            try { currentLoad += InventoryMenu.instance.equipSlots[i].GetItem().coinWeight; } catch { }
        }

        /*Conveniently, since lightLoad is 1/4 of max load, currentLoad/lightLoad can be used to set the enum.
         * Using floor to int, we can get a value between 0 and 4: 0 if the weight is less than a light load,
         * 4 or greater if it is above the encumbrance limit, and anything in between for light, moderate, or heavy loads.
         * By clamping the value, we ensure that carrying greater than 125% of your limit won't break the game.
         * */
        encumbranceLevel = (encumbranceLevels)Mathf.Clamp(Mathf.FloorToInt(currentLoad/lightLoad), 0, 4);
        UpdateSlider(currentLoad);
    }

    public void UpdateSlider(float val)
    {
        //Update the numbers, descriptive text and position of the slider.
        encumbranceBar.value = val;
        encumbranceNumber.text = Mathf.Round(val) + " / " + encumbranceLimit;
        switch (encumbranceLevel)
        {
            case encumbranceLevels.over :
                encumbranceText.text = "Overencumbered (Cannot move)"; break; 
            case encumbranceLevels.heavy:
                encumbranceText.text = "Heavily encumbered (1/4th Movement)"; break;
            case encumbranceLevels.moderate:
                encumbranceText.text = "Moderately encumbered (1/2 Movement)"; break;
            case encumbranceLevels.light:
                encumbranceText.text = "Lightly encumbered (3/4ths Movement)"; break;
            case encumbranceLevels.none:
                encumbranceText.text = "Unencumbered (Full Movement)"; break;

                /*NOTE: in the future this should show the user's actual Move speed.
                //ALSO: Speed reductions should be fractions, not flat reductions.
                Otherwise, you could use Boots of Speed or similar items to circumvent the penalty from heavy burdens.
                
                Carrying Capacities will be 100 * STR + 600, for values between 900 and 2400, with 10 STR = the standard B/X 1600.
                Items like bags and backpacks can increase this by 600, to a soft cap of 3000.
                Certain magic items, like the Bag of Holding and Handy Haversack, dramatically increase this value.
                The hypothetical hard cap (18 STR, and both capacity-increasing items) is 9500, but even though you'll likely
                never have to worry about the weight of items, you still only get 24 inventory slots... >:)
                */
        }
    }
}

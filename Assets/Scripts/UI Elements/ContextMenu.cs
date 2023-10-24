using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContextMenu : MonoBehaviour
{
    public static ContextMenu instance;

    [SerializeField]
    public Button[] buttonList;
    public TextMeshProUGUI Header;

    void Start()
    {
        instance = this;
    }

    public void ShowContextMenu(EnvObject myObject)
    {
        //Set the context menu's location to be based on the cursor's.
        Vector2 mousePos = Input.mousePosition;
        transform.position = new Vector2(mousePos.x + 26, mousePos.y - 12);
        gameObject.SetActive(true);
        UI_Controller.instance.anyMenuOpen = true;

        //Set the header to the name of the right-clicked object.
        Header.text = myObject.objectName;

        //For each button in the list...
        foreach(Button button in buttonList)
        {
            //All buttons are disabled on default.
            button.enabled = false;

            //Compare the name of the button to the name of each allowed action.
            for (int i = 0; i < myObject.allowedInteractions.Length; i++)
            {
                //If the name of the button matches an action, enable it and break. (Continuing the loop could set it false again.)
                if (myObject.allowedInteractions[i].ToString() == button.name)
                {
                    button.enabled = true;
                    break;
                }
            }

            //Hide all disabled buttons.
            button.gameObject.SetActive(button.enabled);
        }
    }


    public void HideContextMenu()
    {
        gameObject.SetActive(false);
        UI_Controller.instance.anyMenuOpen = false;
    }


}

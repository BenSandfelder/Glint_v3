using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;

    public static UI_Controller instance;

    [SerializeField]
    private GameObject[] UI_Windows;

    public AdvancedCustomEvent OnMenuButtonPressed;
    public AdvancedCustomEvent OnCharButtonPressed;
    public AdvancedCustomEvent OnInvButtonPressed;
    public AdvancedCustomEvent OnSpellButtonPressed;
    public AdvancedCustomEvent OnChronicleButtonPressed;
    public AdvancedCustomEvent OnMapsButtonPressed;
    public AdvancedCustomEvent TogglePlayerMovement;

    public bool anyMenuOpen;
    
    private void Start()
    {
        instance = this;
        CloseMainMenus();

        //Initialize bools.
        anyMenuOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Pressed Tab.");
            OnMenuButtonPressed.Invoke(this, 0);
        }
            
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Pressed C.");
            OnCharButtonPressed.Invoke(this, 1);
        }
            
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Pressed I.");
            OnInvButtonPressed.Invoke(this, 2);

        }
            
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Pressed O.");
            OnSpellButtonPressed.Invoke(this, 3);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Pressed J.");
            OnChronicleButtonPressed.Invoke(this, 4);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Pressed M.");
            OnMapsButtonPressed.Invoke(this, 5);
        }
    }
    
    public void ToggleMainMenu(int menu)
    {
        //If the menu is already active, cycle to the newly input menu.
        if (mainMenu.activeInHierarchy && !UI_Windows[menu].activeInHierarchy)
        {
            CycleMenu(menu);
        } else if (mainMenu.activeInHierarchy && UI_Windows[menu].activeInHierarchy)
        {
            //If the button pressed = the menu currently open, close the menu.
            CloseMainMenus();
            TogglePlayerMovement.Invoke(this, anyMenuOpen);
            Debug.Log(anyMenuOpen ? "Opened menu." : "Closed menu.");
        } else
        {
            //If the menu isn't active, toggle it on.
            ToggleMenu(mainMenu);
            CycleMenu(menu);
            TogglePlayerMovement.Invoke(this, anyMenuOpen);
            Debug.Log(anyMenuOpen ? "Opened menu." : "Closed menu.");
        }

    }

    public void CycleMenu(int menu)
    {
        for (int i = 0; i < UI_Windows.Length; i++)
        {
            //Ternary Operator. Set UI_Windows[menu] to active, disable all others.
            UI_Windows[i].SetActive(i == menu ? true : false);
        }
    }

    private void CloseMainMenus()
    {
        foreach (GameObject window in UI_Windows)
        {
            //Disable all UI windows by default.
            window.SetActive(false);
            anyMenuOpen = false;
        }

        mainMenu.SetActive(false);
        anyMenuOpen = false;
    }

    private void ToggleMenu(GameObject menu)
    {
        //If the menu is off, turn it on. If it's on, turn it off.
        menu.SetActive(!menu.activeInHierarchy);
        anyMenuOpen = (!anyMenuOpen);
    }
}

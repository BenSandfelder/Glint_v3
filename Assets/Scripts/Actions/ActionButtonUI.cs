using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionName;
    [SerializeField] private Image actionFrame;
    [SerializeField] private Image actionIcon;
    [SerializeField] private Button actionButton;
    [SerializeField] private Color iconColor;
    [SerializeField] private Color defaultColor;
    private BaseAction action;

    public void SetBaseAction(BaseAction action)
    {
        this.action = action;
        actionName.text = action.GetActionName();
        actionIcon.sprite = action.GetActionIcon();

        // () => {} is an anonymous function, aka a Lambda expression.
        //The same as a "private void [name] () { } but more compact.
        actionButton.onClick.AddListener(() =>  { ActorActionSystem.Instance.SelectAction(action); });
    }

    public BaseAction GetBaseAction() { return action; }
    
    public void UpdateSelectedVisual()
    {
        //Get a reference to the currently selected action.
        BaseAction selectedAction = ActorActionSystem.Instance.GetSelectedAction();
        //If it's this action, select this Action Button...
        if (selectedAction == action) SelectAction();
        //Otherwise, deselect it.
        else DeselectAction();
    }

    private void SelectAction()
    {
        actionFrame.color = Color.white;
        actionIcon.color = iconColor;
    }
    private void DeselectAction()
    {
        actionFrame.color = defaultColor;
        actionIcon.color = defaultColor;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionSystemUI : MonoBehaviour
{
    [SerializeField]
    private Transform actionButtonPrefab;
    [SerializeField]
    private Transform actionBarTransform;
    [SerializeField]
    private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionsList;

    private void Awake()
    {
        //Create an empty list of actions.
        actionsList = new List<ActionButtonUI>();
    }
    private void Start()
    {
        //Create buttons for the selected actor's actions.
        CreateActorActionButtons();
        //Register as a listener for whenever the selected actor changes.
        ActorActionSystem.Instance.OnActorSelected += ActorActionSystem_OnActorSelected;
        ActorActionSystem.Instance.OnActionSelected += ActorActionSystem_OnActionSelected;
        ActorActionSystem.Instance.OnActionStarted += ActorActionSystem_OnActionStarted;
        TurnSystem.instance.OnRoundUpdate += TurnSystem_OnRoundUpdate;
        Actor.OnAnyActionPointsChanged += Actor_OnAnyActionPointsChanged;
        //Update our action bar.
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void CreateActorActionButtons()
    {
        //Destroy the previous actor's action bar.
        foreach (Transform buttonTransform in actionBarTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        //Empty the list of actions.
        actionsList.Clear();

        //Get the most recently selected actor.
        Actor selectedActor = ActorActionSystem.Instance.GetSelectedActor();

        //Create a new action bar.
        foreach (BaseAction action in selectedActor.GetActionArray())
        {
            //Instantiate a button as a child of the actionBarTransform.
            Transform actionButton = Instantiate(actionButtonPrefab, actionBarTransform);
            ActionButtonUI buttonUI = actionButton.GetComponent<ActionButtonUI>();
            buttonUI.SetBaseAction(action);

            //Add it to the list of actions.
            actionsList.Add(buttonUI);
        }
        //Update our action bar.
        UpdateSelectedVisual();
    }

    private void ActorActionSystem_OnActorSelected(object sender, EventArgs e)
    {
        CreateActorActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void ActorActionSystem_OnActionSelected(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI action in actionsList)
        {
            action.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        Actor selectedActor = ActorActionSystem.Instance.GetSelectedActor();
        actionPointsText.text = "Actions: " + selectedActor.GetActionPoints();
    }

    private void ActorActionSystem_OnActionStarted(object sender, EventArgs args)
    {
        UpdateActionPoints();
    }

    private void TurnSystem_OnRoundUpdate(object sender, EventArgs args)
    {
        UpdateActionPoints();
    }

    private void Actor_OnAnyActionPointsChanged(object sender, EventArgs args)
    {
        UpdateActionPoints();
    }

}

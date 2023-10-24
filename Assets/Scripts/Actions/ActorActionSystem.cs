using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ActorActionSystem : MonoBehaviour
{
    // The {get; private set} means any class can access this, but only this can set it.
    public static ActorActionSystem Instance { get; private set; }

    [SerializeField]
    private Actor selectedActor;
    private BaseAction selectedAction;
    [SerializeField]
    private LayerMask actorLayerMask = 8;
    private bool isBusy;

    public event EventHandler OnActorSelected;
    public event EventHandler OnActionSelected;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;
    private void Awake()
    {
        //Singleton Pattern
        if (Instance != null)
        {
            Debug.LogError("Multiple Action Systems present in scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    private void Start()
    {
        //By default, auto-select the actor assigned in the Inspector.
        //NOTE: later, replace this with the first actor in the party, or the first actor in Initiative.
        SelectActor(selectedActor);
        BaseAction[] actions = selectedActor.GetActionArray();
        SelectAction(actions[0]);
    }

    // Update is called once per frame
    private void Update()
    {
        //Are we in the middle of an action already?
        if (isBusy) return;

        //If it's not the player's turn, do nothing.
        if (!TurnSystem.instance.IsPlayerTurn()) return;

        //If the cursor is over a UI element, do not run any Actor Actions.
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //Did we left-click?
        if (Input.GetMouseButtonDown(0))
        {
            //If we did not select an actor on the click, check if we clicked an Action button.
            HandleSelectedAction();
        }
    }

    private void HandleSelectedAction()
    {
        //Find out where we clicked...
        Vector2Int clickedVector = LevelGrid.instance.GetGridPosition(MouseToScreen.GetPosition());

        //Cancel the action if it's an invalid action.
        if (!selectedAction.ValidateAction(clickedVector)) return;

        //Take the action.
        selectedAction.TakeAction(clickedVector, ClearBusy);

        //Trigger any events that trigger when an action starts.
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    public void SelectActor(Actor actor)
    {
        if (actor != null) selectedActor = actor;
        SelectAction(actor.GetAction<MoveAction>());
        //Compact syntax. Checks for null (?) before invoking the event.
        OnActorSelected?.Invoke(this, EventArgs.Empty);
    }

    public void SelectAction(BaseAction action)
    {
        selectedAction = action;
        Debug.Log(selectedAction.GetActionName() + " selected");
        OnActionSelected?.Invoke(this, EventArgs.Empty);
    }

    public Actor GetSelectedActor()
    {
        return selectedActor;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }
}

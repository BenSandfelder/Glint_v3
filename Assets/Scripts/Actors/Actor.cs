using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using System;

public class Actor : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyActorSpawn;
    public static event EventHandler OnAnyActorDestroyed;

    [SerializeField]
    private bool isEnemy;
    public bool canMove = true;
    private Vector2Int gridVector;
    private BaseAction[] ActionArray;
    private HealthSystem healthSystem;

    //NOTE: Later, add a separate pool of Action points and Move points.
    private int actionPoints = 2;
    private const int actionPointsMAX = 2;

    public event EventHandler OnHit;

    private void Awake()
    {
        //Get the Health system.
        healthSystem = GetComponent<HealthSystem>();

        //Will also grab children, such as moveAction and testAction.
        ActionArray = GetComponents<BaseAction>();
        actionPoints = actionPointsMAX;
    }
    private void Start()
    {
        //Set the Actor's position in the LevelGrid.
        gridVector = LevelGrid.instance.GetGridPosition(transform.position);
        LevelGrid.instance.AddActorAtPosition(gridVector, this);
        
        TurnSystem.instance.OnRoundUpdate += TurnSystem_OnRoundUpdate;
        healthSystem.OnDead += HealthSystem_OnDead;

        //Announce that actors have been spawned.
        OnAnyActorSpawn?.Invoke(this, EventArgs.Empty);

    }

    private void Update()
    {
        //If our current vector isn't the vector we're supposed to be at...
        Vector2Int newVector = LevelGrid.instance.GetGridPosition(this.transform.position);
        if (newVector != gridVector)
        {
            //Update the unit's position in the LevelGrid.
            Vector2Int oldVector = gridVector;
            gridVector = newVector;
            //Announce that we've moved to a new position.
            LevelGrid.instance.UpdateActorPosition(this, oldVector, gridVector);
            Debug.Log(name + " moved to " + gridVector);
        }
    }
    public void OnMouseDown()
    {
        if (isEnemy) { return; }
        //If this actor is already selected, do nothing.
        if (ActorActionSystem.Instance.GetSelectedActor() == this) return;

        //Otherwise, select the actor.
        canMove = false;
        ActorActionSystem.Instance.SelectActor(this);
        Debug.Log("Selected " + name);
        StartCoroutine(JustSelected());
    }
    public Vector2Int GetGridPosition() => LevelGrid.instance.GetGridPosition(transform.position);

    IEnumerator JustSelected()
    {
        //Prevents movement for 1/3 of a second after an actor is selected.
        yield return new WaitForSeconds(0.3f);
        canMove = true;
    }
    public BaseAction[] GetActionArray()
    {
        return ActionArray;
    }

    public bool CanSpendActionPoints(BaseAction baseAction)
    {
        return actionPoints >= baseAction.GetActionPointCost();
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    public bool TrySpendActionPoints(BaseAction baseAction)
    {
        if (CanSpendActionPoints(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        } else
        {
            return false;
        }
    }

    private void TurnSystem_OnRoundUpdate(object sender, EventArgs e)
    {
        //If it's an enemy and the enemy's turn, or if it's a player and the player's turn, refresh action points.
        if ((isEnemy && !TurnSystem.instance.IsPlayerTurn()) || !isEnemy && TurnSystem.instance.IsPlayerTurn()){
            actionPoints = actionPointsMAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
        
    }

    public bool IsEnemy() { return isEnemy; }

    public void TakeDamage(int damage)
    {
        healthSystem.TakeDamage(damage);
        OnHit?.Invoke(this, EventArgs.Empty);
        Debug.Log(name + " took " + damage + " damage!");
    }

    private void HealthSystem_OnDead(object sender, EventArgs eventArgs)
    {
        LevelGrid.instance.ClearActorFromPosition(this, gridVector);
        OnAnyActorDestroyed?.Invoke(this, EventArgs.Empty);
       Destroy(this.gameObject);

    }

    public float GetHealthPercentage()
    {
        return healthSystem.GetHealthPercentage();
    }

    //USING GENERICS: this function accepts ANY input, so long as it extends from BaseAction. So, this function can be used with ANY action type.
    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in ActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }

        return null;
    }
}

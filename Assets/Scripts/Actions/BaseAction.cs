using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    //Protected is like private, except classes that extend this class can still use them.
    protected Actor actor;
    protected bool isActive;
    protected Action onActionComplete;
    [SerializeField] protected string actionName;
    [SerializeField] protected Sprite actionIcon;
    [SerializeField] protected int actionPointCost;
    [SerializeField] protected GridVisualManager.GridVisualEffect effectType;

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    //Virtual functions/methods can be overriden.
    protected virtual void Awake()
    {
        actor = GetComponent<Actor>();
    }

    public virtual string GetActionName()
    {
        return actionName;
    }

    public virtual Sprite GetActionIcon()
    {
        return actionIcon;
    }

    public virtual int GetActionPointCost()
    {
        return actionPointCost;
    }

    public virtual void TakeAction(Vector2Int gridPosition, Action onActionComplete)
    {
        if (!actor.TrySpendActionPoints(this)) return;

        onActionComplete(); 
    }

    public abstract List<Vector2Int> GetValidGridVectors();

    public abstract bool ValidateAction(Vector2Int gridPosition);

    protected virtual void ActionStart(Action onActionComplete)
    {
        Debug.Log(actionName + " started!");
        isActive = true;
        this.onActionComplete = onActionComplete;

        //Announce that the action has started.
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void ActionComplete()
    {
        Debug.Log(actionName + " completed!");
        isActive = false;
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        onActionComplete();
    }

    public Actor GetActor() { return actor; }

    public GridVisualManager.GridVisualEffect GetVisualEffect() { return effectType;  }

    public EnemyAIActionData GetBestEnemyAIAction()
    {
        List<EnemyAIActionData> enemyActionList = new List<EnemyAIActionData> ();

        List<Vector2Int> validActionGridPositions = GetValidGridVectors();

        foreach (Vector2Int gridVector in validActionGridPositions) 
        {
            EnemyAIActionData enemyAIActionData = GetEnemyAIActionData(gridVector);
            enemyActionList.Add(enemyAIActionData);
        }

        if(enemyActionList.Count > 0)
        {
            enemyActionList.Sort((EnemyAIActionData a, EnemyAIActionData b) => b.actionValue - a.actionValue);

            return enemyActionList[0];
        } else
        {
            Debug.Log("Enemy has no valid actions.");
            //No AI actions are possible (none have valid targets)
            return null;
        }      
    }

    public abstract EnemyAIActionData GetEnemyAIActionData(Vector2Int gridVector);
}
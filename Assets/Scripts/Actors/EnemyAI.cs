using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn, TakingTurn, Busy
    }

    private State state;

    private float timer = 2f;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }
    private void Start()
    {
        TurnSystem.instance.OnRoundUpdate += TurnSystem_OnRoundUpdate;
    }
    void Update()
    {
        if (TurnSystem.instance.IsPlayerTurn()) return;

        switch(state)
        {
            case State.WaitingForEnemyTurn: break;
            case State.TakingTurn:
                GridVisualManager.Instance.HideAllGridVectors();
                //Enemy waits for timer to complete, then takes actions.
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    //If enemies can successfully take actions...
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        Debug.Log("Enemy is taking a test action.");
                        //Set the state to busy.
                        state = State.Busy;
                    } else
                    {
                        //No more enemies can take actions.
                        Debug.Log("All enemies have acted.");
                        state = State.WaitingForEnemyTurn;
                        TurnSystem.instance.NextTurn();
                    }
                }

                break;

            case State.Busy: break;
        }
    }

    private void SetStateTakingTurn()
    {
        Debug.Log("Continuing Enemy Turn...");
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnRoundUpdate(object sender, EventArgs e)
    {
        if (!TurnSystem.instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f; 
        }
    }
    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        //For each enemy, try to take an action.
        foreach (Actor enemyActor in ActorManager.instance.GetEnemyList())
        {
            if (TryTakeEnemyAIAction(enemyActor, onEnemyAIActionComplete)) return true;
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Actor enemyActor, Action onEnemyAIActionComplete)
    {
        BaseAction bestAction = null;
        EnemyAIActionData bestActionData = null;

        //Loop through all actions available to the enemy.
        foreach (BaseAction baseAction in enemyActor.GetActionArray())
        {
            if (!enemyActor.CanSpendActionPoints(baseAction))
            {
                //Enemy does not have enough AP to perform this action.
                continue;
            }

            //If this is the first action we're checking...
            if (bestActionData == null)
            {
                //...Save its info.
                bestAction = baseAction;
                bestActionData = baseAction.GetBestEnemyAIAction();
            }
            else
            {
                //Save the current data as a separate variable.
                EnemyAIActionData testEnemyActionData = baseAction.GetBestEnemyAIAction();
                //Compare the current data to the saved info.
                if (testEnemyActionData != null && testEnemyActionData.actionValue > bestActionData.actionValue)
                {
                    //If this action's value is > the saved action's, replace it.
                    bestAction = baseAction;
                    bestActionData = testEnemyActionData;
                }
            }
        }

        if (bestAction != null)
        {
            bestAction.TakeAction(bestActionData.gridVector, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            //No valid actions available.
            return false;
        }
    }
}

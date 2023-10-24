using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackAction : BaseAction
{
    [SerializeField] private int atkRange = 7;

    //State Machine for Attacks.
    private enum State {Windup, Attack, FollowThrough }
    private State state;
    private float stateTimer;
    private bool canAttack;
    private Actor targetActor;
    private GridVisualManager.GridVisualEffect rangefinderEffect = GridVisualManager.GridVisualEffect.Red_Solid;

    public override bool ValidateAction(Vector2Int gridVector)
    {
        //This action targets the user; it has a range of zero.
        if (GetValidGridVectors().Contains(gridVector)) return true;
        else return false;
    }

    private void Update()
    {
        if (!isActive) return;
        stateTimer -= Time.deltaTime;

        //Leaving this in for later.
        switch (state)
        {
            case State.Windup:
                break;
            case State.Attack:
                if (canAttack)
                {
                    AttackTarget();
                    canAttack = false;
                }
                break;
            case State.FollowThrough:
                break;
        }

        if (stateTimer <= 0) NextState();
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Windup:
                state = State.Attack;
                float attackTime = 0.5f;
                stateTimer = attackTime;
                break;
            case State.Attack:
                state = State.FollowThrough;
                float followThroughTime = 2f;
                stateTimer = followThroughTime;
                break;
            case State.FollowThrough:
                ActionComplete();

                ///Note: Multiattack actions should lock the player into the attack action.
                ///However, the Action should not complete until ALL attacks have been made.
                break;
        }

        Debug.Log(state);
    }

    public override void TakeAction(Vector2Int gridPosition, Action onActionComplete)
    {
        //Abort the action if there are not enough action points.
        if (!actor.TrySpendActionPoints(this)) return;

        //Set the initial state and timer.
        state = State.Windup;
        float windupTimer = 1f; 
        stateTimer = windupTimer;

        //Get a reference to the target.
        targetActor = LevelGrid.instance.GetActor(gridPosition);
        canAttack = true;

        //Start the action.
        ActionStart(onActionComplete);
    }

    public override List<Vector2Int> GetValidGridVectors()
    {
        Vector2Int actorPosition = actor.GetGridPosition();
        return GetValidGridVectors(actorPosition);
    }
    public List<Vector2Int> GetValidGridVectors(Vector2Int position)
    {
        List<Vector2Int> validVectors = new List<Vector2Int>();

        for (int x = -atkRange; x <= atkRange; x++)
        {
            for (int y = -atkRange; y <= atkRange; y++)
            {
                Vector2Int offsetVector = new Vector2Int(x, y);
                Vector2Int testVector = position + offsetVector;
                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                //If the target tile isn't valid, skip it.
                if (!LevelGrid.instance.CheckTile(testVector)) continue;

                //If the target tile is out of a circular range, skip it.
                if (testDistance > atkRange) continue;

                //If the target tile is NOT already occupied, skip it.
                if (!LevelGrid.instance.HasActorOrObject(testVector)) continue;

                //If the target tile does not have an enemy, skip it.
                if (!LevelGrid.instance.GetActor(testVector)) continue;

                //Get a reference to the Actor in the tile.
                Actor targetActor = LevelGrid.instance.GetActor(testVector);

                //If the target is on the same team as the attacker, skip it (they're allies).
                if (targetActor.IsEnemy() == actor.IsEnemy()) continue;

                //If the tile is valid, add it to the list of valid vectors.
                validVectors.Add(testVector);
            }
        }
        return validVectors;
    }

    public List<Vector2Int> GetAllVectorsInRange(Vector2Int position)
    {
        List<Vector2Int> validVectors = new List<Vector2Int>();

        for (int x = -atkRange; x <= atkRange; x++)
        {
            for (int y = -atkRange; y <= atkRange; y++)
            {
                Vector2Int offsetVector = new Vector2Int(x, y);
                Vector2Int testVector = position + offsetVector;
                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                //If the target tile isn't valid, skip it.
                if (!LevelGrid.instance.CheckTile(testVector)) continue;

                //If the target tile is out of a circular range, skip it.
                if (testDistance > atkRange) continue;

                //If the target tile is our current tile, skip it.
                if (actor.GetGridPosition() == testVector) continue;

                //If the tile is valid, add it to the list of valid vectors.
                validVectors.Add(testVector);
            }
        }
        return validVectors;
    }

    private void AttackTarget()
    {
        //Tell the target to take damage from our attack.
        //NOTE: Attacks will be scriptable objects we can drag-and-drop in.
        //Maybe a unit has a List of attacks, and it uses a loop to keep the player in the attack action until all attacks have been made?
        targetActor.TakeDamage(4);
    }

    public Actor GetTarget()
    {
        return targetActor;
    }

    public int GetRange()
    {
        return atkRange;
    }

    public GridVisualManager.GridVisualEffect GetRangeFinderEffect()
    {
        return rangefinderEffect;
    }

    public override EnemyAIActionData GetEnemyAIActionData(Vector2Int gridVector)
    {
        Actor targetActor = LevelGrid.instance.GetActor(gridVector);

        return new EnemyAIActionData
        {
            gridVector = gridVector,
            //Enemies are more likely to target characters with less HP.
            actionValue = 100 + Mathf.RoundToInt((1 - targetActor.GetHealthPercentage()) * 100f),
        };
    }

    public int GetTargetCountFromPosition(Vector2Int position)
    {
        //Returns the number of targets within range from a given grid vector.
        return GetValidGridVectors(position).Count;
    }
}

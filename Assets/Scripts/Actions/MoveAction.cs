using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveAction : BaseAction
{
    private Vector3 targetPosition;
    [SerializeField]
    private int moveDistance;

    protected override void Awake()
    {
        //base.Awake() calls the BaseAction's Awake() function.
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update()
    {
        //If the move action is not active, update will stop right here.
        if (!isActive) return;

        float stoppingDistance = 0.1f;
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveSpeed = 6f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        } else if (Vector3.Distance(transform.position, targetPosition) != 0)
        {
            transform.position = targetPosition;
            ActionComplete();
        }
    }

    public override bool ValidateAction(Vector2Int gridVector)
    {
        //The Move action is valid if the input Vector2Int is unoccupied and within [moveDistance] tiles of the actor.
        List<Vector2Int> validPositions = GetValidGridVectors();
        return validPositions.Contains(gridVector);
    }

    public override void TakeAction(Vector2Int gridVector, Action onActionComplete)
    {
        if (!actor.TrySpendActionPoints(this)) return;

        //Gets the center of the targeted tile, then tells the unit to move there.
        Vector3 destination = LevelGrid.instance.GetWorldPosition(gridVector);
        targetPosition = destination;

        //Start the action.
        ActionStart(onActionComplete);
    }
    public override List<Vector2Int> GetValidGridVectors()
    {
        List<Vector2Int> validVectors = new List<Vector2Int>();

        Vector2Int actorPosition = actor.GetGridPosition();

        for (int x = -moveDistance; x <= moveDistance; x++)
        {
            for (int y = -moveDistance; y <= moveDistance; y++)
            {
                Vector2Int offsetVector = new Vector2Int(x, y);
                Vector2Int testVector = actorPosition + offsetVector;
                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                //If the target tile isn't valid, skip it.
                if (!LevelGrid.instance.CheckTile(testVector)) continue;

                //If the target tile is out of a circular range, skip it.
                if (testDistance > moveDistance) continue;

                //If the target tile is already occupied, skip it.
                if (LevelGrid.instance.HasActorOrObject(testVector)) continue;

                //If the target tile is our current tile, skip it.
                if (actorPosition == testVector) continue;

                //If the tile is valid, add it to the list of valid vectors.
                validVectors.Add(testVector);
            }
        }
        return validVectors;
    }

    public int GetMoveDistance()
    {
        return moveDistance;
    }
    public override EnemyAIActionData GetEnemyAIActionData(Vector2Int gridVector)
    {
        int targetCountAtPosition = actor.GetAction<AttackAction>().GetTargetCountFromPosition(gridVector);

        return new EnemyAIActionData
        {
            gridVector = gridVector,

            //Enemy will prioritize moving to tiles with the most enemies within range.
            actionValue = targetCountAtPosition * 10,
        };
    }
}

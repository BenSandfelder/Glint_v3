using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAction : BaseAction
{
    private float totalSpinAmount;

    private void Update()
    {
        if (!isActive) return;
        float spinAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, 0, spinAmount);
        totalSpinAmount += spinAmount;

        if (totalSpinAmount >= 360f)
        {
            transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            ActionComplete();
        }
    }

    public override bool ValidateAction(Vector2Int gridVector)
    {
        //This action targets the user; it has a range of zero.
        if (GetValidGridVectors().Contains(gridVector)) return true;
        else return false;
    }

    public override List<Vector2Int> GetValidGridVectors()
    {
        //Spin is validated if the actor's own tile is clicked.
        List<Vector2Int> validVectors = new List<Vector2Int>();
        
        validVectors.Add(actor.GetGridPosition());

        return validVectors;
    }
    public override void TakeAction(Vector2Int gridVector, Action onActionComplete)
    {
        if (!actor.TrySpendActionPoints(this)) return;

        //Reset the spin amount to 0.
        totalSpinAmount = 0;

        //Start the action.
        ActionStart(onActionComplete);
    }

    public override EnemyAIActionData GetEnemyAIActionData(Vector2Int gridVector)
    {
        return new EnemyAIActionData
        {
            gridVector = gridVector,
            actionValue = 0,
        };
    }
}

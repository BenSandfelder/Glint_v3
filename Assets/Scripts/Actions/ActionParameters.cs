using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionParameters
{
    public enum TargetType { Ally, Enemy, Object, Point, Self };
    protected TargetType targetType;
    protected Vector2Int targetPoint;
    protected int range;
    protected Action onActionComplete;

    //Public class ActionParameters are required by all actions, but can be extended to include additional details.
    public ActionParameters(TargetType targetType, Vector2Int targetPoint, int range,  Action onActionComplete)
    {
        this.targetType = targetType;
        this.targetPoint = targetPoint;
        this.range = range;
        this.onActionComplete = onActionComplete;
    }

    public TargetType GetTargetType()
    {
        return targetType;
    }

    public Vector2Int GetTargetPoint()
    {
        return targetPoint;
    }

    public int GetRange()
    {
        return range;
    }

    public Action GetOnActionComplete()
    {
        return onActionComplete;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem instance;
    public event EventHandler OnRoundUpdate;
    private bool isPlayerTurn = true;

    private int roundNumber = 1;



    private void Awake()
    {
        //Singleton Pattern
        if (instance != null)
        {
            Debug.LogError("Multiple Turn Systems present in scene!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void NextTurn()
    {
        //Only update the round number after the enemies act.
        if (!isPlayerTurn) roundNumber++;

        isPlayerTurn = !isPlayerTurn;
        OnRoundUpdate?.Invoke(this, EventArgs.Empty);
    }

    public int GetCurrentRound()
    {
        return roundNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

}

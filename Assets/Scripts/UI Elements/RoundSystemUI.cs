using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class RoundSystemUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roundCounter;
    [SerializeField]
    private Button endRoundButton;

    [SerializeField]
    private GameObject enemyTurnBanner;
    // Start is called before the first frame update
    void Start()
    {
        TurnSystem.instance.OnRoundUpdate += TurnSystem_OnRoundUpdate;
        endRoundButton.onClick.AddListener(() => TurnSystem.instance.NextTurn());
        UpdateRoundCounter(1);
        UpdateEndRoundButton();
    }

    // Update is called once per frame
    private void UpdateRoundCounter(int round)
    {
        roundCounter.text = "Round " + round;
    }

    private void TurnSystem_OnRoundUpdate(object sender, EventArgs e)
    {
        UpdateRoundCounter(TurnSystem.instance.GetCurrentRound());
        UpdateEnemyTurnBanner();
        UpdateEndRoundButton();
    }

    private void UpdateEnemyTurnBanner()
    {
        enemyTurnBanner.SetActive(!TurnSystem.instance.IsPlayerTurn());
    }

    private void UpdateEndRoundButton()
    {
        endRoundButton.gameObject.SetActive(TurnSystem.instance.IsPlayerTurn());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using TMPro;
using System;
using UnityEngine.UI;

public class Actor_WorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Actor actor;
    [SerializeField] private Image healthbarImage;
    [SerializeField] private HealthSystem healthSystem;

    // Start is called before the first frame update

    private void Start()
    {
        Actor.OnAnyActionPointsChanged += Actor_OnAnyActionPointsChanged;
        healthSystem.OnHealthChanged += HealthSystem_OnDamaged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }
    private void UpdateActionPointsText()
    {
        actionPointText.text = actor.GetActionPoints().ToString();
    }

    private void Actor_OnAnyActionPointsChanged(object sender, EventArgs eventArgs)
    {
        UpdateActionPointsText();
    }

    private void UpdateHealthBar()
    {
        healthbarImage.fillAmount = healthSystem.GetHealthPercentage();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs eventArgs)
    {
        UpdateHealthBar();
    }
}

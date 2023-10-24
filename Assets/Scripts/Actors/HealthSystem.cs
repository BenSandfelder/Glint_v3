using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int healthMax = 10;
    private int health;
    public event EventHandler OnDead;
    public event EventHandler OnHealthChanged;

    private void Awake()
    {
        health = healthMax;
    }
    public void TakeDamage(int damage)
    {
        //Reduce Health by input damage.
        health -= damage;

        //Health cannot go below 0.
        if (health < 0)
        {
            health = 0;
        }

        //Announce that our Health changed.
        OnHealthChanged?.Invoke(this, EventArgs.Empty);

        //If Health is reduced to 0, die.
        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPercentage()
    {
        //Return the unit's current health as a percent value.
        return (float)health / healthMax;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCorpseSpawner : MonoBehaviour
{
    [SerializeField] private Transform corpsePrefab;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        //Listen for when the attached Actor dies.
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs eventArgs)
    {
        //Spawn the corpse prefab.
        Instantiate(corpsePrefab, transform.position, Quaternion.identity);
        healthSystem.OnDead -= HealthSystem_OnDead;
    }
}

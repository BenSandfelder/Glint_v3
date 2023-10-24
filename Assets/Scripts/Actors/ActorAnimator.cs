
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        /// If there are action-dependent animations, put references to them here:
        /// if (TryGetComponent<AttackAction>(out AttackAction attackAction)) {...}
        /// 

    }

    private void Start()
    {
        if (TryGetComponent<Actor>(out Actor actor))
        {
            actor.OnHit += Actor_OnHit;
        }
    }
    private void Actor_OnHit(object sender, EventArgs args)
    {
        animator.SetTrigger("OnHit");
        //Later, add an if statement to check if it's a crit.
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorSelectedVisual : MonoBehaviour
{
    [SerializeField]
    private Actor actor;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        //Grab the icon's sprite renderer component.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //+= subscribes our ActorActionSystem_OnActorSelected event to the instance's event.
        ActorActionSystem.Instance.OnActorSelected += ActorActionSystem_OnActorSelected;
        UpdateVisual();
    }

    //Parameters should match those sent by the triggering event.
    private void ActorActionSystem_OnActorSelected(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (ActorActionSystem.Instance.GetSelectedActor() == actor)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnDestroy()
    {
        //-= unsubscribes our ActorActionSystem_OnActorSelected event to the instance's event when this gameObject is destroyed.
        ActorActionSystem.Instance.OnActorSelected -= ActorActionSystem_OnActorSelected;
    }


}

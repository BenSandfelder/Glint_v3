using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private CinemachineVirtualCamera actionCamera;
    private bool isFollowingActor = false;
    private Actor actorToFollow;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple Camera Managers present in scene!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        actionCamera = actionCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        HideActionCamera();
    }

    private void Update()
    {
        if (isFollowingActor)
        {
            Vector3 targetPos = new Vector3(actorToFollow.transform.position.x, actorToFollow.transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 3f);
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case AttackAction attackAction:
                HideActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case AttackAction attackAction:
                Actor attacker = attackAction.GetActor();
                Actor target = attackAction.GetTarget();

                Vector3 aboveTarget = new Vector3(target.transform.position.x, target.transform.position.y, -7);
                actionCameraGameObject.transform.position = aboveTarget;

                ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }

    public void EnableActorFollowCamera(Actor actor)
    {
        isFollowingActor = true;
        actorToFollow = actor;
        ShowActionCamera();
    }

    public void DisableActorFollowCamera()
    {
        isFollowingActor = false;
        actorToFollow = null;
        HideActionCamera();
    }
}

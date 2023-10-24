using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionBusyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI actionDescription;

    private void Start()
    {
        ActorActionSystem.Instance.OnBusyChanged += ActorActionSystem_OnBusyChanged;
        Hide();
    }

    private void ActorActionSystem_OnBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
            Show();
        else Hide();
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}

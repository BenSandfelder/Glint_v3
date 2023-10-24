using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
//Advanced Custom Events pass in their source and data, not just the event signal.
public class AdvancedCustomEvent : UnityEvent<Component, object> { }

public class CustomEventListener : MonoBehaviour
{
    public CustomEvent gameEvent;
    public AdvancedCustomEvent response;

    //When this gameObject is enabled, register it as a listener.
    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    //When this gameObject is disabled, unregister it.
    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    //When a signal is received, trigger the event we're listening for.
    public void OnEventRaised(Component sender, object data)
    {
        response.Invoke(sender, data);
    }
}
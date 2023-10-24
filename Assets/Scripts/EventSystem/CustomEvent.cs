using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Custom Events can be created in the editor.
[CreateAssetMenu(menuName = "GameEvent")]
public class CustomEvent : ScriptableObject
{
    //A database of all gameObjects listening for this event.
    public List<CustomEventListener> listeners = new List<CustomEventListener>();

    //Adds a new listener to the listeners registry.
    public void RegisterListener(CustomEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    //Removes a gameObject from the listener registry.
    public void UnregisterListener(CustomEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

    //Broadcasts a signal for all listeners.
    public void Raise(Component sender, object data)
    {
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnEventRaised(sender, data);
        }
    }
}

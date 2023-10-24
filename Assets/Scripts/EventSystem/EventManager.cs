using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    //Collection of all events.
    private Dictionary<string, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    //An instance of this EventManager.
    public static EventManager instance
    {
        get
        {
            //If there isn't an event manager...
            if (!eventManager)
            {
                //Search the scene for one.
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                //If there still isn't an event manager, report an error.
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    //Initialize the event manager.
                    eventManager.Init();
                }
            }

            //Return the event manager.
            return eventManager;
        }
    }

    //Initializes the dictionary of events.
    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    //Add a new event to the eventDictionary.
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    //Remove an event from the eventDictionary
    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    //Trigger an event recorded in the dictionary.
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

}

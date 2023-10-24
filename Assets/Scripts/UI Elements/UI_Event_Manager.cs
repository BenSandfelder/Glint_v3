using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UI_Event_Manager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> UIeventDictionary;

    private static UI_Event_Manager UIeventManager;

    //An instance of this EventManager.
    public static UI_Event_Manager instance
    {
        get
        {
            //If there isn't an event manager...
            if (!UIeventManager)
            {
                //Search the scene for one.
                UIeventManager = FindObjectOfType(typeof(UI_Event_Manager)) as UI_Event_Manager;

                //If there still isn't an event manager, report an error.
                if (!UIeventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    //Initialize the event manager.
                    UIeventManager.Init();
                }
            }

            //Return the event manager.
            return UIeventManager;
        }
    }

    //Initializes the dictionary of events.
    void Init()
    {
        if (UIeventDictionary == null)
        {
            UIeventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    //Add a new event to the eventDictionary.
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.UIeventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.UIeventDictionary.Add(eventName, thisEvent);
        }
    }

    //Remove an event from the eventDictionary
    public static void StopListening(string eventName, UnityAction listener)
    {
        if (UIeventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.UIeventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    //Trigger an event recorded in the dictionary.
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.UIeventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}

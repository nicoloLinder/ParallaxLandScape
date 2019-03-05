using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public enum EventName
{
    TILT_PHONE,
    DRAGGING_FINGER,
    SCROLLING
}

public class EventManager : MonoBehaviour
{

    #region Variables

    #region PublicVariables

    #endregion

    #region PrivateVariables

    private Dictionary<EventName, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    #endregion

    #endregion

    #region Properties

    #endregion

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogWarning("There was no active EventManager script on a GameObject in your scene, a new GameObject with and EventManager was created");

                    GameObject eventManagerGameObject = new GameObject("EventManager");
                    eventManager = eventManagerGameObject.AddComponent<EventManager>();

                    eventManager.Init();
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    #region MonoBehabiourMethods

    #endregion

    #region Methods

    #region PublicMethods

    /// <summary>
    /// Starts the listening to an event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    /// <param name="listener">Listener.</param>
    public static void StartListening(EventName eventName, UnityAction listener)
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

    /// <summary>
    /// Stops the listening to an event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    /// <param name="listener">Listener.</param>
    public static void StopListening(EventName eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Removes an event completely.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    public static void StopListening(EventName eventName)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            instance.eventDictionary.Remove(eventName);
        }
    }

    /// <summary>
    /// Trigger an event.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    public static void TriggerEvent(EventName eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    /// <summary>
    /// Trigger an event from the scene.
    /// </summary>
    /// <param name="eventName">Event name.</param>
    public void TriggerEventFromScene(EventName eventName)
    {
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    #endregion

    #region PrivateMethods

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EventName, UnityEvent>();
        }
    }


    #endregion

    #endregion
}
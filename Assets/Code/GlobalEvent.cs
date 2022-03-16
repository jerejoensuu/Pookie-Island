using System;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class GlobalEvent : MonoBehaviour {

    public static Action GamePaused;
    public static Action GameUnpaused;
    
    public static void PauseGame() {
        GamePaused?.Invoke();
        Time.timeScale = 0;
    }
    
    public static void UnpauseGame() {
        GameUnpaused?.Invoke();
        Time.timeScale = 1;
    }
    
    
    private static HashSet<GameObject> listeners = new HashSet<GameObject>();
    private static HashSet<GameObject> listenersToBeRemoved = new HashSet<GameObject>();
    private static bool iterating;

    public static void SendEvent(string eventName) {
        iterating = true;
        foreach (GameObject listener in listeners) {
            CustomEvent.Trigger(listener, eventName);
        }
        foreach (GameObject toBeRemoved in listenersToBeRemoved) {
            listeners.Remove(toBeRemoved);
        }
        listenersToBeRemoved.Clear();
        iterating = false;
    }

    public static void Subscribe(GameObject listener) {
        listeners.Add(listener);
    }
    
    public static void UnSubscribe(GameObject listener) {
        if (iterating) {
            listenersToBeRemoved.Add(listener);
        }
        else {
            listeners.Remove(listener);
        }
    }
}

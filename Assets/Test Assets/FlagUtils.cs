using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class FlagUtils {
    
    private static HashSet<GameObject> listeners = new HashSet<GameObject>();
    private static HashSet<GameObject> listenersToBeRemoved = new HashSet<GameObject>();
    private static bool iterating;

    public static bool IsFlagOn(String flagName) {
        return false;
    }

    public static void SetFlag(String flagName) {
        iterating = true;
        foreach (GameObject listener in listeners) {
            CustomEvent.Trigger(listener, "OnFlagChanged", flagName);
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

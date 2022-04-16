using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class FlagUtils {
    
    private static HashSet<GameObject> listeners = new HashSet<GameObject>();
    private static HashSet<GameObject> listenersToBeRemoved = new HashSet<GameObject>();
    private static bool iterating;

    public static Action<string> OnFlagSet;

    public static bool IsFlagOn(string flagName) {
        return SaveUtils.currentSaveGame.Flags.Contains(flagName);
    }

    public static void SetFlag(string flagName) {
        iterating = true;
        if (!SaveUtils.currentSaveGame.Flags.Contains(flagName)) {
            SaveUtils.currentSaveGame.Flags.Add(flagName);
            foreach (GameObject listener in listeners) {
                CustomEvent.Trigger(listener, "OnFlagChanged", flagName);
                OnFlagSet?.Invoke(flagName);
            }
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

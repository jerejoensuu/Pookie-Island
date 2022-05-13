using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndChecker : MonoBehaviour {
    
    
    void OnTriggerEnter(Collider col) {
        int totalCrystalAmount = 1;
        if (SaveUtils.currentSaveGame.Crystal == totalCrystalAmount) {
            GameObject.Find("Root").GetComponent<SceneRoot>().parent.GetComponent<SceneLoader>().GameComplete();
        } else {
            Debug.Log("Not enough crystals");
        }
    }

}

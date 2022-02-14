using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    public GameObject prefabToSpawn;

    public void SpawnNow() {
        Instantiate(prefabToSpawn, transform);
    }

    public void SpawnOverTime(int seconds) {
        //do over time spawning shit
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTimer : MonoBehaviour {

    public float Limit;
    float timer;

    void Update() {
        timer += Time.deltaTime;
        if (timer >= Limit) Destroy(gameObject);
    }
}

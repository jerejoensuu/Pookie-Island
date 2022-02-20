using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour {

    public GameObject testPookie;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (transform.childCount < 5) {
            GameObject pookie = Instantiate(testPookie);
            pookie.transform.SetParent(transform);
            pookie.transform.position = new Vector3(transform.position.x + Random.Range(-1f, 1f),
                                                    transform.position.y + Random.Range(-1f, 1f),
                                                    transform.position.z + Random.Range(-1f, 1f));
        }
    }
}

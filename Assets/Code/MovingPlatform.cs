using UnityEngine;
using Sirenix.OdinInspector;

public class MovingPlatform : MonoBehaviour {

    [Required] public Collider trigger;
    GameObject player;
    GameObject originalParent;

    void Start() {
        originalParent = GameObject.Find("Default Prefabs");  //TODO: Set in editor to be the default location for character
    }

    void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Player")) {
            col.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.CompareTag("Player")) {
            col.transform.parent = originalParent.transform;
        }
    }
}
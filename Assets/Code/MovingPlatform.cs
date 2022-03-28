using UnityEngine;
using Sirenix.OdinInspector;

public class MovingPlatform : MonoBehaviour {

    [Required] public Collider trigger;
    GameObject player;
    GameObject originalParent;

    void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Player")) {
            originalParent = col.transform.parent.gameObject;
            col.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.CompareTag("Player")) {
            col.transform.parent = originalParent.transform;
        }
    }
}
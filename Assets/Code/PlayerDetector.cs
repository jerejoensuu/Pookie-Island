using UnityEngine;

public class PlayerDetector : MonoBehaviour {
    
    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        other.gameObject.GetComponent<PlayerController>().CrystalGot();
    }

}

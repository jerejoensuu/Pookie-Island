using UnityEngine;

public class DestroyOnCollision : MonoBehaviour {
    


    void Start() {
        
    }

    void Update() {
        
    }

    void OnCollisionEnter(Collision col) {
        if (col.collider.gameObject.layer == 7) return;
        Destroy(gameObject);
    }
}

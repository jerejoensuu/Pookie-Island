using UnityEngine; 
using Sirenix.OdinInspector; 
 
public class MovingPlatform : MonoBehaviour { 
 
    [Required] public Collider Trigger; 
    [Required] public Rigidbody Rigidbody; 
 
    void Start() { 
        if (Rigidbody) Rigidbody.isKinematic = true;
    } 
 
    void OnTriggerEnter(Collider col) { 
        if (col.CompareTag("Player")) { 
            col.transform.parent = transform; 
        } 
    } 
 
    void OnTriggerExit(Collider col) { 
        if (col.CompareTag("Player")) { 
            col.gameObject.GetComponent<PlayerController>().ResetParent(); 
            col.gameObject.GetComponent<PlayerMovement>().momentum = GetComponent<Rigidbody>().velocity; 
        } 
    } 
}
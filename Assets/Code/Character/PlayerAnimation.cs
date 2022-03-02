using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    
    [SerializeField] PlayerController player;

    internal Animator animator;


    void Start() {
        animator = player.model.GetComponent<Animator>();
    }
}

using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    
    [SerializeField] PlayerController player;

    internal Animator animator;
    public Animator camAnimator;

    void Start() {
        animator = player.model.GetComponent<Animator>();
    }

    void Update() {
        if (player.movement.movementBlocked && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            player.movement.movementBlocked = false;
            camAnimator.SetTrigger("MovementEnabled");
        }
    }
}

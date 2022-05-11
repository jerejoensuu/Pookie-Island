using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {
    
    [SerializeField] PlayerController player;

    internal Animator animator;
    public Animator camAnimator;
    public GameObject collectedCrystal;
    GameObject c;

    void Start() {
        animator = player.model.GetComponent<Animator>();
    }

    void Update() {
        if (player.movement.movementBlocked && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            player.movement.movementBlocked = false;
            camAnimator.SetTrigger("MovementEnabled");
        }
    }

    public void CrystalAnimation() {
        c = Instantiate(collectedCrystal, transform.position, Quaternion.identity);
        StartCoroutine(Timer());

        IEnumerator Timer() {
            yield return new WaitForSeconds(2.5f);
            Destroy(c);
        }
    }

}
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    internal CharacterController controller;
    GameObject camTarget;

    public Vector3 movement;
    private float accel = 0;

    internal bool moving, jumping, jumpPressed, autoTurningPlayer;
    public bool knockedBack = false;
    int jumps = 2;

    // GroundCheck:
    internal RaycastHit rayHit;
    internal bool grounded;
    internal Vector3 center, size;


    void Start() {
        controller = GetComponent<CharacterController>();
        camTarget = player.vcamera.camTarget;
        SetupJumpVariables();
    }

    void SetupJumpVariables() {
        float timeToApex = player.maxJumpTime / 2;
        player.gravity = (-2 * player.maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        player.jumpSpeed = (2 * player.maxJumpHeight) / timeToApex;
    }

    public void HandleMovement() {
        if (!controller.isGrounded && jumps > 1) jumps = 1;

        if (!knockedBack) controller.Move(movement * Time.deltaTime);

        ApplyGravity();

        if (!knockedBack) {
            HandleJump();
            HandleWalk();
        } else {
            player.knockbackHandler.HandleKnockback();
        }
        
        if (knockedBack == true) knockedBack = !controller.isGrounded;
        camTarget.transform.position = player.transform.position + (Vector3.up * player.cameraHeight); // move camera to player:
    }

    void ApplyGravity() {
        bool falling = movement.y <= 0;
        float gravity = !knockedBack ? player.gravity : player.gravity * 0.25f;
        if (controller.isGrounded & falling) { //TODO Remove "falling" if it causes problems
            movement.y = player.groudedGravity;
        } else if (falling) {
            movement.y += gravity * player.fallMultiplier * Time.deltaTime;
        } else {
            movement.y += gravity * Time.deltaTime;
        }
    }

    void HandleJump() {
        if (jumpPressed && (controller.isGrounded || jumps > 0)) {
            player.anim.animator.SetTrigger("jump");
            jumpPressed = false;
            movement.y = jumps == 2 ? player.jumpSpeed : player.jumpSpeed * 0.75f;
            jumps--;
        } else if (controller.isGrounded && !jumpPressed) {
            player.anim.animator.SetTrigger("grounded");
            jumps = 2;
        }

        
    }
    
    void HandleWalk() {
        // dampen movement:
        if (moving) {
            if (grounded) {
                accel += player.accelSpeed;
            } else {
                accel += player.accelSpeed * .3f;
            }
            if (accel > 1) accel = 1;
            if (accel > player.inputReader.rawInput.magnitude) accel = player.inputReader.rawInput.magnitude;
            RotatePlayer();
        } else {
            accel -= player.decelSpeed;
            if (accel <= 0) accel = 0;
        }

        movement.x = player.inputReader.directionInput.x * accel;
        movement.z = player.inputReader.directionInput.y * accel;
        
        // Slow movement when vacuuming:
        if (player.vacuum.pull) {
            movement.x *= player.vacuumSpeedMod;
            movement.z *= player.vacuumSpeedMod;
        }

        // Movement in relation to the camera:
        movement = Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * movement;
    }

    public Vector3 GetTrueDirection() {
        return Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * new Vector3(player.inputReader.rawInput.x, 0, player.inputReader.rawInput.y);
    }

    void RotatePlayer() {
        if (player.vcamera.aiming) return;
        player.model.transform.forward = Vector3.Slerp(player.model.transform.forward, GetTrueDirection(), 20 * Time.deltaTime);
    }

    public IEnumerator PointPlayerAt(Vector3 direction) {
        if (autoTurningPlayer) yield break;
        autoTurningPlayer = true;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        while(Quaternion.Angle(targetRotation, player.model.transform.rotation) > 1f) {
            player.model.transform.rotation = Quaternion.Slerp(player.model.transform.rotation, targetRotation, 15 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        autoTurningPlayer = false;
    }
}

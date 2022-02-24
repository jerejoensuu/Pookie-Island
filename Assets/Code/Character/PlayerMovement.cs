using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    internal CharacterController controller;
    GameObject camTarget;

    Vector3 movement;
    private float accel = 0;

    internal bool moving, jumping, jumpPressed;
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

    internal void HandleMovement() {
        if (!controller.isGrounded && jumps > 1) jumps = 1;
        controller.Move(movement * Time.deltaTime);
        ApplyGravity();
        HandleJump();
        HandleWalk();
        camTarget.transform.position = player.transform.position + (Vector3.up * player.cameraHeight); // move camera to player:
    }

    void ApplyGravity() {
        bool falling = movement.y <= 0;
        if (controller.isGrounded) {
            movement.y = player.groudedGravity;
        } else if (falling) {
            movement.y += player.gravity * player.fallMultiplier * Time.deltaTime;
        } else {
            movement.y += player.gravity * Time.deltaTime;
        }
    }

    void HandleJump() {
        if (jumpPressed && (controller.isGrounded || jumps > 0)) {
            jumpPressed = false;
            movement.y = jumps == 2 ? player.jumpSpeed : player.jumpSpeed * 0.75f;
            jumps--;
        } else if (controller.isGrounded && !jumpPressed) {
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
            if (accel > player.input.rawInput.magnitude) accel = player.input.rawInput.magnitude;
            RotatePlayer();
        } else {
            accel -= player.decelSpeed;
            if (accel <= 0) accel = 0;
        }
        movement.x = player.input.directionInput.x * accel;
        movement.z = player.input.directionInput.y * accel;
        
        // Slow movement when vacuuming:
        if (player.vacuum.pull) {
            movement.x *= player.vacuumSpeedMod;
            movement.z *= player.vacuumSpeedMod;
        }

        // Movement in relation to the camera:
        movement = Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * movement;
    }

    public Vector3 GetTrueDirection() {
        return Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * new Vector3(player.input.rawInput.x, 0, player.input.rawInput.y);
    }

    void RotatePlayer() {
        if (player.vcamera.aiming) return;
        player.model.transform.forward = Vector3.Slerp(player.model.transform.forward, GetTrueDirection(), 20 * Time.deltaTime);
    }
}

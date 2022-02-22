using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    CharacterController controller;
    GameObject camTarget;
    Vector3 movementDirection;
    private float accel = 0;

    internal bool moving, jumping;

    internal RaycastHit rayHit;
    internal bool grounded;
    internal Vector3 center, size;


    void Start() {
        controller = GetComponent<CharacterController>();
        camTarget = player.vcamera.camTarget;
    }

    void Update() {
        
    }

    internal void ApplyMovement() {
        // dampen movement:
        if (moving) {
            if (grounded) {
                accel += player.accelSpeed;
            } else {
                accel += player.accelSpeed * .7f;
            }
            if (accel >= 1) accel = 1;
            RotatePlayer();
        } else {
            accel -= player.decelSpeed;
            if (accel <= 0) accel = 0;
        }
        // change player velocity based on input and camera rotation:
        movementDirection.x = player.input.directionInput.x * accel;
        movementDirection.z = player.input.directionInput.y * accel;
        if (grounded && movementDirection.y < 0) {
            movementDirection.y = 0;
        } else {
            movementDirection.y -= player.gravity * Time.deltaTime;
        }
        if (player.vacuum.pull) {
            movementDirection.x *= player.vacuumSpeedMod;
            movementDirection.z *= player.vacuumSpeedMod;
        }
        controller.Move(Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * movementDirection * Time.deltaTime);
        camTarget.transform.position = player.transform.position + (Vector3.up * player.cameraHeight); // move camera to player:
    }

    public Vector3 GetTrueDirection() {
        return Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * new Vector3(player.input.directionInput.x, 0, player.input.directionInput.y);
    }

    void RotatePlayer() {
        if (player.vcamera.aimingMode) return;
        player.model.transform.forward = Vector3.Slerp(player.model.transform.forward, GetTrueDirection(), 20 * Time.deltaTime);
    }

    public void Jump(InputAction.CallbackContext context) {
        if (!grounded || jumping) return;
        jumping = context.performed;
        player.vacuum.pull = !context.performed;
        player.anim.animator.SetTrigger("jump");
        movementDirection.y = player.jumpSpeed;
    }

    internal void GroundCheck() {
        center = new Vector3(player.model.transform.position.x, controller.bounds.min.y + 0.1f, player.model.transform.position.z);
        size = new Vector3(controller.bounds.extents.x * 1.95f, 0, controller.bounds.extents.z * 1.95f) * 0.9f;
        grounded = Physics.BoxCast(center, size/2, Vector3.down, out rayHit, player.model.transform.rotation, 0.2f);
        if (grounded && controller.velocity.y >= 0) {
            jumping = false;
            player.anim.animator.SetTrigger("grounded");
        }
    }

    void OnDrawGizmos() {
        if (grounded) {
            Gizmos.color = Color.green;
        } else {
            Gizmos.color = Color.red;
        }

        //Draw a Ray forward from GameObject toward the hit
        Gizmos.DrawRay(center, Vector3.down * rayHit.distance);
        //Draw a cube that extends to where the hit exists
        Gizmos.DrawWireCube(center + Vector3.down * (rayHit.distance / 2), new Vector3(size.x, rayHit.distance, size.z));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    
    GameObject player;
    Rigidbody rb;
    Collider col;
    GameObject camTarget;
    Inputs inputs;

    public float speed = 5;
    public float mouseSensitivity = 2;
    public float accelSpeed = 0.0025f;
    public float decelSpeed = 0.005f;

    Vector2 direction;
    private float accel = 0;
    private bool moving;
    private bool jumping;

    private Vector2 cameraRotation;
    public float cameraHeight;
    private bool rotatingCamera;

    RaycastHit rayHit;
    bool grounded;
    Vector3 center;
    Vector3 size;

    void Start() {
        player = transform.Find("Model").gameObject;
        rb = player.GetComponent<Rigidbody>();
        col = player.GetComponent<Collider>();
        camTarget = transform.Find("Camera Target").gameObject;

        inputs = new Inputs();
        inputs.Enable();
        inputs.Player.Move.performed += ReadMovement;
        inputs.Player.Move.canceled += ReadMovement;
        inputs.Player.Jump.performed += Jump;
        inputs.Player.Jump.canceled += Jump;
    }

    void Update() {
        rotatingCamera = RotateCamera();
        ApplyMovement();
        GroundCheck();
    }

    void ApplyMovement() {
        // dampen movement:
        if (moving) {
            accel += accelSpeed;
            if (accel >= 1) accel = 1;
            RotatePlayer();
        } else {
            accel -= decelSpeed;
            if (accel <= 0) accel = 0;
        }
        // change player velocity based on input and camera rotation:
        Vector3 vel = new Vector3(direction.x, 0, direction.y) * accel;
        rb.velocity = Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * vel;
        // move camera to player:
        camTarget.transform.position = player.transform.position + (Vector3.up * cameraHeight);
    }

    void RotatePlayer() {
        // rotate player based on movement direction
    }

    void ReadMovement(InputAction.CallbackContext context) {
        moving = context.performed;
        if (context.performed) direction = context.ReadValue<Vector2>() * speed;
    }

    void Jump(InputAction.CallbackContext context) {
        if (!grounded || jumping) return;
        jumping = context.performed;
        // jumping mechanic
    }

    bool RotateCamera() {
        cameraRotation.x += mouseSensitivity * Input.GetAxis("Mouse X");

        cameraRotation.y -= mouseSensitivity * Input.GetAxis("Mouse Y");
        cameraRotation.y = Mathf.Clamp (cameraRotation.y, -70, 80); // limits vertical rotation

        camTarget.transform.eulerAngles = new Vector3(cameraRotation.y, cameraRotation.x, 0.0f);

        return Input.GetAxis("Mouse X") != 0 ? true : false;
    }

    void GroundCheck() {
        center = new Vector3(player.transform.position.x, col.bounds.min.y, player.transform.position.z);
        size = new Vector3(player.transform.localScale.x, 0, player.transform.localScale.z) * 0.9f;
        grounded = Physics.BoxCast(center, size, Vector3.down, out rayHit, player.transform.rotation, 0.2f);
        if (grounded) {
            jumping = false;
            Debug.Log("Hit : " + rayHit.collider.name);
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
        Gizmos.DrawWireCube(center + Vector3.down * rayHit.distance, size);
    }

    void OnDisable() {
        inputs.Disable();
        inputs.Player.Move.performed -= ReadMovement;
        inputs.Player.Move.canceled -= ReadMovement;
        inputs.Player.Jump.performed -= Jump;
        inputs.Player.Jump.canceled -= Jump;
    }
}

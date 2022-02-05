using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    
    GameObject player;
    Rigidbody rb;
    GameObject camTarget;
    Inputs inputs;
    Vector2 direction;
    public float speed = 5;
    public float mouseSensitivity = 2;
    public float accelSpeed = 0.0025f;
    public float decelSpeed = 0.005f;
    private float accel = 0;
    private bool moving;
    private Vector2 cameraRotation;
    public float cameraHeight;

    void Start() {
        player = transform.Find("Model").gameObject;
        rb = player.GetComponent<Rigidbody>();
        camTarget = transform.Find("Camera Target").gameObject;

        inputs = new Inputs();
        inputs.Enable();
        inputs.Player.Move.performed += ReadMovement;
        inputs.Player.Move.canceled += ReadMovement;
        inputs.Player.Jump.performed += Jump;
        inputs.Player.Jump.canceled += Jump;
    }

    void Update() {
        ApplyMovement();
        RotateCamera();
    }

    void ApplyMovement() {
        if (moving) {
            accel += accelSpeed;
            if (accel >= 1) accel = 1;
            RotatePlayer();
        } else {
            accel -= decelSpeed;
            if (accel <= 0) accel = 0;
        }
        Vector3 vel = new Vector3(direction.x, 0, direction.y) * accel;
        rb.velocity = Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * vel;
        camTarget.transform.position = player.transform.position + (Vector3.up * cameraHeight);
    }

    void RotatePlayer() {
        // Vector3 current = new Vector3(0, transform.localEulerAngles.y, 0);
        // Vector3 target = new Vector3(0, transform.localEulerAngles.y + rb.velocity.y, 0);
        // transform.localEulerAngles = Vector3.RotateTowards(transform.localEulerAngles, transform.localEulerAngles + rb.velocity, 1, 1);

        // float angle = 180 + Vector3.SignedAngle(Vector3.back, rb.velocity, Vector3.up);
        // player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, angle, player.transform.eulerAngles.z);
    }

    void ReadMovement(InputAction.CallbackContext context) {
        moving = context.performed;
        if (context.performed) direction = context.ReadValue<Vector2>() * speed;
    }

    void Jump(InputAction.CallbackContext context) {

    }

    void RotateCamera() {
        cameraRotation.x += mouseSensitivity * Input.GetAxis("Mouse X");

        cameraRotation.y -= mouseSensitivity * Input.GetAxis("Mouse Y");
        cameraRotation.y = Mathf.Clamp (cameraRotation.y, -70, 80); // limits vertical rotation

        camTarget.transform.eulerAngles = new Vector3(cameraRotation.y, cameraRotation.x, 0.0f);
    }

    void OnDisable() {

    }
}

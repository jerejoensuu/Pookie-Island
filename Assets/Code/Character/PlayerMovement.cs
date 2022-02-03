using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    
    Rigidbody rb;
    Inputs inputs;
    Vector2 direction;
    public float speed = 5;
    public float accelSpeed = 0.0025f;
    public float decelSpeed = 0.005f;
    private float accel = 0;
    private bool moving;

    void Start() {
        rb = GetComponent<Rigidbody>();

        inputs = new Inputs();
        inputs.Enable();
        inputs.Player.Move.performed += ReadMovement;
        inputs.Player.Move.canceled += ReadMovement;
        inputs.Player.Jump.performed += Jump;
        inputs.Player.Jump.canceled += Jump;
    }

    void Update() {
        ApplyMovement();
    }

    void ApplyMovement() {
        if (moving) {
            Debug.Log("Accelerating");
            accel += accelSpeed;
            if (accel >= 1) accel = 1;
        } else {
            Debug.Log("Decelerating");
            accel -= decelSpeed;
            if (accel <= 0) accel = 0;
        }
        rb.velocity = new Vector3(direction.x, 0, direction.y) * accel;
    }

    void ReadMovement(InputAction.CallbackContext context) {
        moving = context.performed;
        if (context.performed) direction = context.ReadValue<Vector2>() * speed;
    }

    void Jump(InputAction.CallbackContext context) {

    }

    void OnDisable() {

    }
}

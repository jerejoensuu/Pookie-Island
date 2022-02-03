using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    
    Rigidbody rb;
    Inputs inputs;
    Vector2 movement;
    public float speed = 3;

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
        rb.velocity = new Vector3(movement.x, 0, movement.y);
    }

    void ReadMovement(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>() * speed;
    }

    void Jump(InputAction.CallbackContext context) {

    }

    void OnDisable() {

    }
}

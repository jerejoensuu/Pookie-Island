using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    [Header("Helper scripts")]
    [SerializeField] internal PlayerInput input;
    [SerializeField] internal PlayerMovement movement;
    [SerializeField] internal PlayerCamera vcamera;
    [SerializeField] internal PlayerAnimation anim;
    [SerializeField] internal VacuumController vacuum;

    [Header("Movement")]
    public GameObject model;
    internal float gravity;
    internal float groudedGravity = -0.5f;
    internal float jumpSpeed;
    [SerializeField] internal float maxJumpHeight;
    [SerializeField] internal float maxJumpTime;
    [SerializeField] internal float fallMultiplier = 2;
    public float speed = 15;
    internal float vacuumSpeedMod = 0.5f;
    
    public float accelSpeed = 0.0025f;
    public float decelSpeed = 0.005f;

    [Header("Camera")]
    public float stickSensitivity = 2;
    public float mouseSensitivity = 2;
    public float cameraSpeedModifier = 0.75f;
    public float autoRotateSpeed;
    internal float cameraHeight;
    
    void Awake() {
        model = transform.Find("Model").gameObject;
    }

    void Update() {
        vcamera.RotateCamera();
        vcamera.AutoRotateCamera();
        movement.HandleMovement();
        vcamera.Aim();
    }
    
}

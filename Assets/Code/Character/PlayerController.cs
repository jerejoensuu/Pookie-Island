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
    [Space(10)]

    public GameObject model;
    

    public float gravity = 10;
    public float speed = 15;
    internal float vacuumSpeedMod = 0.5f;
    public float jumpSpeed = 2;
    public float accelSpeed = 0.0025f;
    public float decelSpeed = 0.005f;

    
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
        movement.ApplyMovement();
        movement.GroundCheck();
        vcamera.Aim();
    }
    
}

using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Helper scripts")]
    public PlayerInput input;
    public PlayerMovement movement;
    public PlayerCamera vcamera;
    public PlayerAnimation anim;
    public VacuumController vacuum;

    [Header("Movement")]
    public GameObject model;
    internal float gravity;
    internal float groudedGravity = -0.5f;
    internal float jumpSpeed;
    public float maxJumpHeight;
    public float maxJumpTime;
    public float fallMultiplier = 2;
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

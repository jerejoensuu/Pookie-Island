using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {

    [FormerlySerializedAs("input")] [Header("Helper scripts")]
    public InputReader inputReader;
    public PlayerMovement movement;
    public PlayerCamera vcamera;
    public PlayerAnimation anim;
    public VacuumController vacuum;
    public PlayerHealth playerHealth;
    public KnockbackHandler knockbackHandler; 

    [Header("Movement")]
    public GameObject model;
    internal float gravity;
    internal float groudedGravity = -8.5f;
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
    public float cameraHeight;

    Vector3 spawnPoint;
    internal Vector3 softSpawnPoint;
    float softRespawnCounter = 0;
    bool unsafeSpawn = false;

    void Start() {
        spawnPoint = softSpawnPoint = transform.position;
    }

    void Update() {
        vcamera.RotateCamera();
        vcamera.AutoRotateCamera();
        movement.HandleMovement();
        vcamera.Aim();
        CheckSoftRespawn();
    }

    void CheckSoftRespawn() {
        softRespawnCounter += Time.deltaTime;
        if (softRespawnCounter > 1.5f && movement.grounded && !unsafeSpawn) {
            softRespawnCounter = 0;
            softSpawnPoint = transform.position;
        } 
    }

    public void SoftRespawn() {
        movement.controller.enabled = false;
        transform.position = softSpawnPoint;
        movement.controller.enabled = true;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == 10) {
            unsafeSpawn = true;
        }
    }

    void OntriggerExit(Collider col) {
        if (col.gameObject.layer == 10) {
            unsafeSpawn = false;
        }
    }

    // void OnDrawGizmos() {
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawSphere(softSpawnPoint, 0.3f);
    // }
    
}

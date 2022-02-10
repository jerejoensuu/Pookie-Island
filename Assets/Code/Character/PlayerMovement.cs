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
    public float stickSensitivity = 2;
    public float mouseSensitivity = 2;
    public float accelSpeed = 0.0025f;
    public float decelSpeed = 0.005f;

    Vector2 directionInput;
    Vector2 trueDirection;
    private float accel = 0;
    private bool turningCamera, moving, jumping;
    private Vector2 cameraRotation;
    public float cameraHeight;

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
        inputs.Player.LookStick.performed += ReadCameraInputStick;
        inputs.Player.LookStick.canceled += ReadCameraInputStick;
        inputs.Player.LookMouse.performed += ReadCameraInputMouse;
        inputs.Player.LookMouse.canceled += ReadCameraInputMouse;
        inputs.Player.CenterCamera.performed += CenterCamera;
        inputs.Player.Move.performed += ReadMovement;
        inputs.Player.Move.canceled += ReadMovement;
        inputs.Player.Jump.performed += Jump;
        inputs.Player.Jump.canceled += Jump;
    }

    void Update() {
        RotateCamera();
        AutoRotateCamera();
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
        Vector3 vel = new Vector3(directionInput.x, 0, directionInput.y) * accel;
        rb.velocity = Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * vel;
        // move camera to player:
        camTarget.transform.position = player.transform.position + (Vector3.up * cameraHeight);
    }

    Vector3 GetTrueDirection() {
        return Quaternion.AngleAxis(camTarget.transform.eulerAngles.y, Vector3.up) * new Vector3(directionInput.x, 0, directionInput.y);
    }

    void RotatePlayer() {
        player.transform.forward = Vector3.Slerp(player.transform.forward, GetTrueDirection(), 20 * Time.deltaTime);
    }

    void ReadMovement(InputAction.CallbackContext context) {
        moving = context.performed;
        if (context.performed) directionInput = context.ReadValue<Vector2>() * speed;
    }

    void Jump(InputAction.CallbackContext context) {
        if (!grounded || jumping) return;
        Debug.Log("adf");
        jumping = context.performed;
        rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
    }

    void ReadCameraInputStick(InputAction.CallbackContext context) {
        turningCamera = context.performed;
        cameraRotation.x = stickSensitivity * context.ReadValue<Vector2>().x;
    }

    void ReadCameraInputMouse(InputAction.CallbackContext context) {
        turningCamera = context.performed;
        cameraRotation.x = mouseSensitivity * context.ReadValue<Vector2>().x;
    }

    void RotateCamera() {
        if (!turningCamera) return;
        
        camTarget.transform.eulerAngles += new Vector3(0, cameraRotation.x, 0) * Time.deltaTime;
    }

    void AutoRotateCamera() {
        if (turningCamera || !moving) return;
        // float angle = Vector3.SignedAngle()
        camTarget.transform.forward = Vector3.Slerp(camTarget.transform.forward, GetTrueDirection(), 0.2f * Time.deltaTime);
    }

    void CenterCamera(InputAction.CallbackContext context) {
        StartCoroutine(PointCameraAt(GetTrueDirection()));
    }

    IEnumerator PointCameraAt(Vector3 direction) {
        while(Mathf.Abs(Vector3.SignedAngle(direction, camTarget.transform.forward, Vector3.up)) > 3) {
            if (turningCamera) break;
            camTarget.transform.forward = Vector3.Slerp(camTarget.transform.forward, direction, 25 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    void GroundCheck() {
        center = new Vector3(player.transform.position.x, col.bounds.min.y + 0.1f, player.transform.position.z);
        size = new Vector3(player.transform.localScale.x, 0, player.transform.localScale.z) * 0.9f;
        grounded = Physics.BoxCast(center, size, Vector3.down, out rayHit, player.transform.rotation, 0.2f);
        Debug.Log(grounded);
        if (grounded) {
            jumping = false;
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
        inputs.Player.LookStick.performed -= ReadCameraInputStick;
        inputs.Player.LookStick.canceled -= ReadCameraInputStick;
        inputs.Player.LookMouse.performed -= ReadCameraInputMouse;
        inputs.Player.LookMouse.canceled -= ReadCameraInputMouse;
        inputs.Player.CenterCamera.performed -= CenterCamera;
        inputs.Player.Move.performed -= ReadMovement;
        inputs.Player.Move.canceled -= ReadMovement;
        inputs.Player.Jump.performed -= Jump;
        inputs.Player.Jump.canceled -= Jump;
    }
}

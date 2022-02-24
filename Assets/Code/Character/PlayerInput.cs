using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {

    [SerializeField] PlayerController player;
    Inputs inputs;
    internal Vector2 directionInput;
    internal Vector2 rawInput;
    bool manualAiming;


    void Start() {
        inputs = new Inputs();
        inputs.Enable();
        inputs.Player.Move.performed += ReadMovement;
        inputs.Player.Move.canceled += ReadMovement;
        inputs.Player.Jump.performed += ReadJump;
        inputs.Player.Jump.canceled += ReadJump;

        inputs.Player.Pull.performed += ReadPullInput;
        inputs.Player.Pull.canceled += ReadPullInput;
        inputs.Player.Aim.performed += ReadAimInput;
        inputs.Player.Aim.canceled += ReadAimInput;

        inputs.Player.LookStick.performed += ReadCameraInputStick;
        inputs.Player.LookStick.canceled += ReadCameraInputStick;
        inputs.Player.LookMouse.performed += ReadCameraInputMouse;
        inputs.Player.LookMouse.canceled += ReadCameraInputMouse;
        inputs.Player.CenterCamera.performed += CenterCamera;
    }

    void Update() {
        
    }

    void ReadMovement(InputAction.CallbackContext context) {
        if (context.ReadValue<Vector2>().magnitude < 0.15f) {
            directionInput = Vector2.zero;
            player.movement.moving = false;
            player.anim.animator.SetBool("walking", false);
        } else {
            directionInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1) * player.speed;
            player.movement.moving = true;
            player.anim.animator.SetBool("walking", true);
        }
        if (context.performed) rawInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1);
    }

    void ReadJump(InputAction.CallbackContext context) {
        player.movement.jumpPressed = context.performed;
    }

    void ReadPullInput(InputAction.CallbackContext context) {
        if (!player.movement.controller.isGrounded) return;

        player.vacuum.pull = context.performed;
        if (!manualAiming) player.vcamera.aiming = context.performed;
        if (context.performed && !manualAiming) StartCoroutine(player.vcamera.PointCameraAt(player.model.transform.forward));
    }

    void ReadAimInput(InputAction.CallbackContext context) {
        if (!player.vacuum.pull) player.vcamera.aiming = context.performed;
        manualAiming = context.performed;
    }

    void ReadCameraInputStick(InputAction.CallbackContext context) {
        if (context.ReadValue<Vector2>().magnitude < 0.2f) {
            player.vcamera.turningCamera = false;
            player.vcamera.cameraRotation.x = 0;
        } else {
            player.vcamera.turningCamera = true;
            player.vcamera.cameraRotation.x = player.stickSensitivity * context.ReadValue<Vector2>().x;
        }
    }

    void ReadCameraInputMouse(InputAction.CallbackContext context) {
        player.vcamera.turningCamera = context.performed;
        player.vcamera.cameraRotation.x = player.mouseSensitivity * context.ReadValue<Vector2>().x;
    }

    void CenterCamera(InputAction.CallbackContext context) {
        player.vcamera.CenterCamera();
    }

    void OnDisable() {
        inputs.Player.Move.performed -= ReadMovement;
        inputs.Player.Move.canceled -= ReadMovement;
        inputs.Player.Jump.performed -= ReadJump;
        inputs.Player.Jump.canceled -= ReadJump;

        inputs.Player.Pull.performed -= ReadPullInput;
        inputs.Player.Pull.canceled -= ReadPullInput;
        inputs.Player.Aim.performed -= ReadAimInput;
        inputs.Player.Aim.canceled -= ReadAimInput;

        inputs.Player.LookStick.performed -= ReadCameraInputStick;
        inputs.Player.LookStick.canceled -= ReadCameraInputStick;
        inputs.Player.LookMouse.performed -= ReadCameraInputMouse;
        inputs.Player.LookMouse.canceled -= ReadCameraInputMouse;
        inputs.Player.CenterCamera.performed -= CenterCamera;
    }
}

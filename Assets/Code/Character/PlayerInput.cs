using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {

    [SerializeField] PlayerController player;
    Inputs inputs;
    internal Vector2 directionInput;
    

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
        player.movement.moving = context.performed;
        player.anim.animator.SetBool("walking", context.performed);
        if (context.performed) directionInput = context.ReadValue<Vector2>() * player.speed;
    }

    void ReadJump(InputAction.CallbackContext context) {
        player.movement.jumpPressed = context.performed;
    }

    void ReadPullInput(InputAction.CallbackContext context) {
        if (!player.movement.grounded) return;
        player.vacuum.pull = player.vcamera.aimingMode = context.performed;
        if (player.vcamera.manualAiming) player.vcamera.aimingMode = true;
    }

    void ReadAimInput(InputAction.CallbackContext context) {
        if (player.vacuum.pull) {
            player.vcamera.manualAiming = context.performed;
        } else {
            player.vcamera.aimingMode = player.vcamera.manualAiming = context.performed;
        }
    }

    void ReadCameraInputStick(InputAction.CallbackContext context) {
        player.vcamera.turningCamera = context.performed;
        player.vcamera.cameraRotation.x = player.stickSensitivity * context.ReadValue<Vector2>().x;
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

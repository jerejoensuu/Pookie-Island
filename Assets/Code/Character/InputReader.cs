using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour {

    [SerializeField] PlayerController player;
    Inputs inputs;
    internal Vector2 directionInput;
    internal Vector2 rawInput;
    public bool manualAiming;


    private void Awake() {
        inputs = new Inputs();
        GlobalEvent.GamePaused += GamePaused;
        GlobalEvent.GameUnpaused += GameUnpaused;
    }

    private void OnDestroy() {
        GlobalEvent.GamePaused -= GamePaused;
        GlobalEvent.GameUnpaused -= GameUnpaused;
    }

    private void GameUnpaused() {
        enabled = true;
    }

    private void GamePaused() {
        enabled = false;
    }

    void OnEnable() {
        inputs.Enable();
        inputs.Player.Move.performed += ReadMovement;
        inputs.Player.Move.canceled += ReadMovement;
        inputs.Player.Jump.performed += ReadJump;
        inputs.Player.Jump.canceled += ReadJump;
        inputs.Player.Roll.performed += ReadRoll;
        inputs.Player.Run.performed += ReadRun;
        inputs.Player.Run.canceled += ReadRun;

        inputs.Player.Pull.performed += ReadPullInput;
        inputs.Player.Pull.canceled += ReadPullInput;
        inputs.Player.Use.performed += ReadUseInput;
        inputs.Player.Use.canceled += ReadUseInput;
        inputs.Player.Eject.performed += Eject;
        inputs.Player.Aim.performed += ReadAimInput;
        inputs.Player.Aim.canceled += ReadAimInput;

        inputs.Player.LookStick.performed += ReadCameraInputStick;
        inputs.Player.LookStick.canceled += ReadCameraInputStick;
        inputs.Player.LookMouse.performed += ReadCameraInputMouse;
        inputs.Player.LookMouse.canceled += ReadCameraInputMouse;
        inputs.Player.CenterCamera.performed += CenterCamera;
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

    void ReadRun(InputAction.CallbackContext context) {
        player.movement.run = context.performed;
    }

    void ReadJump(InputAction.CallbackContext context) {
        player.movement.jumpPressed = context.performed;
        player.vcamera.aiming = player.vacuum.pull = false;
    }

    void ReadRoll(InputAction.CallbackContext context) {
        if (!player.movement.controller.isGrounded
            || player.vacuum.elements.use
            || player.vacuum.pull
            || player.vacuum.tank.carriedObject != null) return;
        if (player.movement.rollTimer > player.movement.rollLength / 2) StartCoroutine(player.movement.Roll());
    }

    void ReadPullInput(InputAction.CallbackContext context) {
        if (!player.movement.controller.isGrounded
            || player.vacuum.elements.use
            || player.vacuum.tank.carriedObject != null
            || player.playerHealth.onCooldown) return;

        player.vacuum.pull = context.performed;
        if (!manualAiming) player.vcamera.aiming = context.performed;
        if (context.performed && !manualAiming) StartCoroutine(player.vcamera.PointCameraAt(player.model.transform.forward));
    }

    void ReadUseInput(InputAction.CallbackContext context) {
        if (player.vacuum.pull
            || player.vacuum.tank.carriedObject != null
            || player.playerHealth.onCooldown) return;

        player.vacuum.elements.use = context.performed;
    }

    void Eject(InputAction.CallbackContext context) {
        if (player.playerHealth.onCooldown) return;
        player.vacuum.tank.Eject();
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
        inputs.Disable();

        inputs.Player.Move.performed -= ReadMovement;
        inputs.Player.Move.canceled -= ReadMovement;
        inputs.Player.Jump.performed -= ReadJump;
        inputs.Player.Jump.canceled -= ReadJump;
        inputs.Player.Roll.performed -= ReadRoll;

        inputs.Player.Pull.performed -= ReadPullInput;
        inputs.Player.Pull.canceled -= ReadPullInput;
        inputs.Player.Use.performed -= ReadUseInput;
        inputs.Player.Use.canceled -= ReadUseInput;
        inputs.Player.Eject.performed -= Eject;
        inputs.Player.Aim.performed -= ReadAimInput;
        inputs.Player.Aim.canceled -= ReadAimInput;

        inputs.Player.LookStick.performed -= ReadCameraInputStick;
        inputs.Player.LookStick.canceled -= ReadCameraInputStick;
        inputs.Player.LookMouse.performed -= ReadCameraInputMouse;
        inputs.Player.LookMouse.canceled -= ReadCameraInputMouse;
        inputs.Player.CenterCamera.performed -= CenterCamera;
        
        directionInput = Vector2.zero;
        player.movement.moving = false;
        player.anim.animator.SetBool("walking", false);
        player.movement.jumpPressed = false;
        player.vacuum.pull = false;
        player.vcamera.turningCamera = false;
        player.vcamera.cameraRotation.x = 0;
        player.vacuum.elements.use = false;
    }
}

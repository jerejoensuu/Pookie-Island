using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    [SerializeField] internal GameObject camTarget;
    internal bool aimingMode, manualAiming;
    Vector2 trueDirection;
    internal bool turningCamera;
    internal Vector2 cameraRotation;

    void Start() {

    }
    void Update() {
        
    }

    internal void Aim() {
        if (!aimingMode) return;
        Vector3 angles;
        angles = new Vector3(0, Mathf.LerpAngle(player.model.transform.eulerAngles.y, camTarget.transform.eulerAngles.y, 25 * Time.deltaTime), 0);
        player.model.transform.eulerAngles = angles;
        
    }

    internal void RotateCamera() {
        if (!turningCamera) return;
        float modifier = player.vacuum.pull ? player.vacuumSpeedMod : 1;
        camTarget.transform.eulerAngles += new Vector3(0, cameraRotation.x, 0) * modifier * Time.deltaTime;
    }

    internal void AutoRotateCamera() {
        if (turningCamera || !player.movement.moving || aimingMode) return;
        float rotateFactor = Vector3.Dot(camTarget.transform.right, Vector3.Normalize(player.movement.GetTrueDirection()));
        camTarget.transform.eulerAngles += new Vector3(0, rotateFactor * player.autoRotateSpeed * Mathf.Abs(player.input.directionInput.x / player.speed), 0) * Time.deltaTime;
    }

    public void CenterCamera() {
        StartCoroutine(PointCameraAt(player.movement.GetTrueDirection()));
    }

    IEnumerator PointCameraAt(Vector3 direction) {
        while(Mathf.Abs(Vector3.SignedAngle(direction, camTarget.transform.forward, Vector3.up)) > 3) {
            if (turningCamera) break;
            camTarget.transform.forward = Vector3.Slerp(camTarget.transform.forward, direction, 25 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}

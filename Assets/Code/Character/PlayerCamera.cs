using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    [SerializeField] internal GameObject camTarget;
    internal bool aiming;
    Vector2 trueDirection;
    internal bool turningCamera, autoTurningCamera;
    internal Vector2 cameraRotation;

    void Start() {

    }
    void Update() {
        
    }

    internal void Aim() {
        if (!aiming || autoTurningCamera) return;
        Vector3 angles = new Vector3(0, Mathf.LerpAngle(player.model.transform.eulerAngles.y, camTarget.transform.eulerAngles.y, 25 * Time.deltaTime), 0);
        player.model.transform.eulerAngles = angles;
        
    }

    internal void RotateCamera() {
        if (!turningCamera || autoTurningCamera) return;
        float modifier = player.vacuum.pull ? player.vacuumSpeedMod : 1;

        camTarget.transform.eulerAngles += new Vector3(0, cameraRotation.x, 0) * modifier * Time.deltaTime;
    }

    internal void AutoRotateCamera() {
        if (turningCamera || autoTurningCamera || !player.movement.moving || aiming) return;
        float rotateFactor = Vector3.Dot(camTarget.transform.right, Vector3.Normalize(player.movement.GetTrueDirection()));
        camTarget.transform.eulerAngles += new Vector3(0, rotateFactor * player.autoRotateSpeed * Mathf.Abs(player.input.directionInput.x / player.speed), 0) * Time.deltaTime;
    }

    public void CenterCamera() {
        if (autoTurningCamera) return;
        StopCoroutine("PointCameraAt");
        StartCoroutine(PointCameraAt(player.model.transform.forward));
    }

    public IEnumerator PointCameraAt(Vector3 direction) {
        autoTurningCamera = true;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        while(Quaternion.Angle(targetRotation, camTarget.transform.rotation) > 0.5f) {
            camTarget.transform.rotation = Quaternion.RotateTowards(camTarget.transform.rotation, targetRotation, 100 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        autoTurningCamera = false;
    }
}

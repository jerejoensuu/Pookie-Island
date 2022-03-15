using System.Collections;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    
    public GameObject camTarget;
    public GameObject cam;
    CinemachineVirtualCamera cmCam;
    CinemachineComponentBase componentBase;

    internal bool aiming;
    Vector2 trueDirection;
    internal bool turningCamera, autoTurningCamera;
    internal Vector2 cameraRotation;

    RaycastHit hit;
    float cameraDistance = 8;
    public bool cameraSafe = true;

    void Update() {
        if (!cameraSafe) UpdateSafePosition();
        //Debug.Log(cameraSafe);
    }

    void Start() {
        cmCam = cam.GetComponent<CinemachineVirtualCamera>();
        componentBase = cmCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
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
        camTarget.transform.eulerAngles += new Vector3(0, rotateFactor * player.autoRotateSpeed * Mathf.Abs(player.inputReader.directionInput.x / player.speed), 0) * Time.deltaTime;
    }

    public void CenterCamera() {
        if (autoTurningCamera || turningCamera) return;
        StopCoroutine("PointCameraAt");
        StartCoroutine(PointCameraAt(player.model.transform.forward));
    }

    public void UpdateSafePosition() {
        Vector3 camPos = cam.transform.position;
        Vector3 camTargetPos = camTarget.transform.position;

        if (Physics.Raycast(camTargetPos, camPos - camTargetPos, out hit, 8, ~(1 << 5))) {
            cameraDistance = Vector3.Distance(hit.point, camTargetPos);
            (componentBase as Cinemachine3rdPersonFollow).CameraDistance =
                Mathf.Lerp((componentBase as Cinemachine3rdPersonFollow).CameraDistance,
                0,
                0.99f * Time.deltaTime);
        } else {
            (componentBase as Cinemachine3rdPersonFollow).CameraDistance =
                Mathf.Lerp((componentBase as Cinemachine3rdPersonFollow).CameraDistance,
                8,
                0.99f * Time.deltaTime);
        }
    }

    void LerpCameraDistance() {
        (componentBase as Cinemachine3rdPersonFollow).CameraDistance =
            Mathf.Lerp((componentBase as Cinemachine3rdPersonFollow).CameraDistance,
            cameraDistance,
            0.8f * Time.deltaTime);
    }

    public IEnumerator PointCameraAt(Vector3 direction) {
        if (autoTurningCamera) yield break;
        autoTurningCamera = true;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        while(Quaternion.Angle(targetRotation, camTarget.transform.rotation) > 1f) {
            camTarget.transform.rotation = Quaternion.Slerp(camTarget.transform.rotation, targetRotation, 15 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        autoTurningCamera = false;
    }

    void OnDrawGizmos() {
        Debug.DrawLine(camTarget.transform.position, cam.transform.position, Color.red);
    }
}

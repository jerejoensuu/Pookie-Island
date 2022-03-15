using UnityEngine;

public class CameraCollision : MonoBehaviour {

    public PlayerCamera playerCamera;

    void OnCollisionEnter(Collision col) {
        playerCamera.cameraSafe = false;
    }

    void OnCollisionExit(Collision col) {
        playerCamera.cameraSafe = true;
    }
}

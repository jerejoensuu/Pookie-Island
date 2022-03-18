using System;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public Transform cameraTransform;

    private void Update() {
        Vector3 lookAt = cameraTransform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(new Vector3(-lookAt.x, 0, -lookAt.z), Vector3.up);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine;

public class TestCharacterController : MonoBehaviour {
    private Rigidbody body;

    public float rotationSensitivity = 2;
    public float gravity = 10;
    public float forwardSensitivity = 5;
    
    // Start is called before the first frame update
    void Start() {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void FixedUpdate() {
        body.AddForce(transform.forward * Input.GetAxis("Vertical") * forwardSensitivity);
        Vector3 planetNormal = transform.localPosition.normalized * -1;
        body.AddForce(planetNormal * gravity, ForceMode.Acceleration);
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, planetNormal), transform.localPosition);
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * rotationSensitivity, Space.Self);
    }
}

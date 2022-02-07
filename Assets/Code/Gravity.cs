using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour {
    
    private Rigidbody body;
    public float extraGravity;
    public float extraDrag;
    public float extraAngularDrag;
    public float mass = 1;
    public RigidbodyInterpolation interpolation;
    public CollisionDetectionMode collisionDetection;
    
    private float gravity;

    private void Awake() {
        body = gameObject.GetComponent<Rigidbody>();
        PlanetData planetData = Planet.GetPlanetData();
        body.drag = planetData.drag + extraDrag;
        body.angularDrag = planetData.angularDrag + extraAngularDrag;
        gravity = planetData.gravity + extraGravity;
        body.useGravity = false;
        body.mass = mass;
        body.interpolation = interpolation;
        body.collisionDetectionMode = collisionDetection;
    }

    void FixedUpdate() {
        Vector3 planetNormal = transform.localPosition.normalized * -1;
        body.AddForce(planetNormal * gravity, ForceMode.Acceleration);
    }
}

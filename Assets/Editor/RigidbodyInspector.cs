using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rigidbody))]
public class RigidbodyInspector : Editor {
    public override void OnInspectorGUI() {
        Rigidbody gameObject = (Rigidbody)target;
        if (gameObject.TryGetComponent(out Gravity gravity)) {
            EditorGUILayout.HelpBox("USE GRAVITY COMPONENT TO CONFIGURE RIGIDBODY!", MessageType.Info);
        } else {
            EditorGUILayout.HelpBox("IF YOU WANT OBJECT TO USE SPHERICAL GRAVITY, ADD THE GRAVITY SCRIPT COMPONENT", MessageType.Info);
            base.OnInspectorGUI();
        }
    }
}

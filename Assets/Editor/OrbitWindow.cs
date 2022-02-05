using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

class OrbitWindow : EditorWindow {
    
    public bool orbitTool = false;
    public bool autoAlign = false;
    public Transform pivot;

    public static string windowName = "Orbit";

    [MenuItem("Window/Orbit")] 
    public static void ShowWindow () {
        GetWindow<OrbitWindow>(windowName);
    }
    
    void OnGUI () {
        GUILayout.BeginVertical("box");
        orbitTool = GUILayout.Toggle(orbitTool, "Orbit Tool", "Button");
        if (orbitTool) {
            GUILayout.Label("Tool Settings");
            autoAlign = GUILayout.Toggle(autoAlign, "Maintain Rotation", "Button");
        }
        GUILayout.EndVertical();
        
        GUILayout.Label("Pivot");
        pivot = (Transform)EditorGUILayout.ObjectField(pivot, typeof(Transform), true);
        
        Transform transform = Selection.activeTransform;
        GUI.enabled = transform != null;
        
        if (GUILayout.Button("Align with parent")) {
            Undo.RecordObject(transform, "Align with parent");
            Vector3 planetNormal = pivot == null ? transform.localPosition.normalized * -1 : (transform.position - pivot.position).normalized * -1;
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, planetNormal), pivot == null ? transform.localPosition : transform.position - pivot.position);
        }

        GUI.enabled = true;
    }
}

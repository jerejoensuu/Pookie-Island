using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Transform))]
public class Orbit : Editor {

    private bool continueEditing = false;
    
    public Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point) {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }
    
    public void OnSceneGUI() {
        var transform = target as Transform;
        if (transform == null) return;
        if (EditorWindow.HasOpenInstances<OrbitWindow>()) {
            OrbitWindow window = EditorWindow.GetWindow<OrbitWindow>(OrbitWindow.windowName,false);
            Event currentEvent = Event.current;
            Transform pivot = window.pivot != null ? window.pivot : transform.parent;
            if (window.orbitTool && pivot != null) {
                Vector3 YVector = (transform.position - pivot.position).normalized;
                if (YVector.magnitude == 0) return;
                Quaternion viewRotation = SceneView.lastActiveSceneView.rotation;
                Vector3 XVector = Vector3.Cross(YVector, viewRotation * Vector3.left).normalized;
                Vector3 ZVector = Vector3.Cross(XVector, YVector).normalized;
                //ROTATION
                Quaternion rotation = Handles.Disc(transform.localRotation, transform.position, YVector,
                    HandleUtility.GetHandleSize(transform.position) * 0.7f, false, 0);
                if (rotation != transform.localRotation) {
                    if (continueEditing == false) Undo.RecordObject(transform, "Rotate");
                    else Undo.IncrementCurrentGroup();
                    continueEditing = true;
                    transform.localRotation = rotation;
                    return;
                }
                //HEIGHT
                Vector3 height = Handles.Slider(transform.position, transform.position - pivot.position);
                if (height != transform.position) {
                    if (continueEditing == false) Undo.RecordObject(transform, "Change Height");
                    else Undo.IncrementCurrentGroup();
                    continueEditing = true;
                    transform.position = height;
                    return;
                }
                //ORBIT
                if (Handles.Slider2D(transform.position, YVector, XVector, ZVector,
                        HandleUtility.GetHandleSize(transform.position) * 0.5f, Handles.CircleHandleCap, Vector2.zero) != transform.position) {
                    if (continueEditing == false) Undo.RecordObject(transform, "Orbit");
                    else Undo.IncrementCurrentGroup();
                    continueEditing = true;
                    Camera camera = SceneView.lastActiveSceneView.camera;
                    Vector2 mousePos = new Vector2(currentEvent.mousePosition.x,
                        camera.pixelHeight - currentEvent.mousePosition.y);
                    Vector3 point1 =
                        camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.nearClipPlane));
                    Vector3 point2 =
                        camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.farClipPlane));

                    Vector3 nearest = FindNearestPointOnLine(point1, point2, pivot.position);
                    float shortestMagnitude = (nearest - pivot.position).magnitude;
                    float desiredMagnitude = (transform.position - pivot.position).magnitude;

                    if (desiredMagnitude < shortestMagnitude) return;

                    Vector3 desiredPosition = nearest +
                                              -Mathf.Sqrt(Mathf.Pow(desiredMagnitude, 2) -
                                                          Mathf.Pow(shortestMagnitude, 2)) *
                                              (point2 - point1).normalized;
                    Quaternion relativeRotation =
                        Quaternion.FromToRotation((transform.position - pivot.position).normalized, (desiredPosition - pivot.position).normalized);
                    transform.position = desiredPosition;

                    if (window.autoAlign) {
                        transform.localRotation = relativeRotation * transform.localRotation;
                    }
                    return;
                }
                continueEditing = false;
            }
        }
    }
}

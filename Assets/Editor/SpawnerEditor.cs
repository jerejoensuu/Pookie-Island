using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : OdinEditor {
    
    public void OnSceneGUI() {
        Spawner spawner = target as Spawner;
        Handles.zTest = CompareFunction.Less;
        Handles.color = new Color(1f, 0f, 0f, .2f);
        Handles.DrawSolidDisc(spawner.transform.position, Vector3.up, spawner.radius);
    }
}
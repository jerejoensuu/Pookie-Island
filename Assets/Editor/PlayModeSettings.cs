using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

class PlayModeSettings : EditorWindow {
    
    [MenuItem("Window/Play Mode Settings")]
    public static void ShowWindow() {
        GetWindow(typeof(PlayModeSettings));
    }
    
    void OnGUI() {
        if (GUILayout.Button("Enter Play Mode (Main Scene)")) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
            EditorSceneManager.playModeStartScene = sceneAsset;
            EditorApplication.EnterPlaymode();
        } else if (GUILayout.Button("Enter Play Mode (Current Scene)")) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.playModeStartScene = null;
            EditorApplication.EnterPlaymode();
        }
    }
}
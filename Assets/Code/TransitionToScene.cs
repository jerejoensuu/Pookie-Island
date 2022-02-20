using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TransitionToScene : MonoBehaviour
{
    public SceneRoot root;
    
    [AssetsOnly, ValidateInput("@TransitionTo.AssetGUID.Length != 0")]
    public AssetReference TransitionTo;

#if UNITY_EDITOR
    private void Reset() {
        Button btn = GetComponent<Button>();
        GetSceneRoot();
        if(btn != null) {
            UnityEventTools.AddVoidPersistentListener(btn.onClick, DoTransition);
        }
    }

    private void GetSceneRoot() {
        foreach (var rootGameObject in gameObject.scene.GetRootGameObjects()) {
            root = rootGameObject.GetComponent<SceneRoot>();
            if (root != null) break;
        }
        if (root == null) Debug.LogError("Missing SceneRoot!");
    }
#endif

    public void DoTransition() {
        if (TransitionTo != null && root != null) root.LoadScene(TransitionTo);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;

public class introtransition : MonoBehaviour
{
        public SceneRoot root;
    public SceneLoader.GameState transitionToState = SceneLoader.GameState.MENU;
    
    [AssetsOnly, ValidateInput("@TransitionTo.AssetGUID.Length != 0")]
    public AssetReference TransitionTo;

    void OnEnable()
    {
        foreach (var rootGameObject in gameObject.scene.GetRootGameObjects()) {
            root = rootGameObject.GetComponent<SceneRoot>();
            if (root != null) break;
        }
        if (root == null) Debug.LogError("Missing SceneRoot!");
        root.LoadScene(TransitionTo, transitionToState);
    }
}

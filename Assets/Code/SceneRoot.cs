using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneRoot : MonoBehaviour {

    [HideInInspector]
    public SceneLoader parent;

    public void LoadScene(AssetReference toLoad, SceneLoader.GameState state) {
#if UNITY_EDITOR
        if (parent == null) {
            Debug.Log("Can't load scene from this context!");
            return;
        }
#endif
        parent.LoadScene(toLoad, state);
    }
}

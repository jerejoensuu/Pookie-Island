using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneRoot : MonoBehaviour {

    [HideInInspector]
    public SceneLoader parent;

    public void LoadScene(AssetReference toLoad) {
        parent.LoadScene(toLoad);
    }
}

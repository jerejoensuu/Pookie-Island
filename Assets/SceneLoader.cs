using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    [AssetsOnly]
    public AssetReference startingScene;

    [SceneObjectsOnly]
    public GameObject loadingScene;
    
    private SceneInstance currentScene;
    private Scene defaultScene;
    
    void Start() {
        defaultScene = SceneManager.GetActiveScene();
        LoadScene(startingScene);
    }

    public void LoadScene(AssetReference toLoad) {
        UnloadScene();
        loadingScene.SetActive(true);
        Addressables.LoadSceneAsync(toLoad, LoadSceneMode.Additive, false).Completed += operationHandle => {
            currentScene = operationHandle.Result;
            operationHandle.Result.ActivateAsync().completed += _ => {
                loadingScene.SetActive(false);
                foreach (var gameObject in operationHandle.Result.Scene.GetRootGameObjects()) {
                    SceneRoot component = gameObject.GetComponent<SceneRoot>();
                    if (component != null) component.parent = this;
                }
                
                SceneManager.SetActiveScene(currentScene.Scene);
                DynamicGI.UpdateEnvironment();
            };
        };
    }

    public void UnloadScene() {
        if (default(SceneInstance).Equals(currentScene)) return;
        SceneManager.SetActiveScene(defaultScene);
        Addressables.UnloadSceneAsync(currentScene);
    }
}

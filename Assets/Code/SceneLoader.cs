using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    [AssetsOnly]
    public AssetReference startingScene;

    [SceneObjectsOnly]
    public GameObject loadingScene;
    
    private SceneInstance currentScene;
    private AssetReference currentSceneReference;
    private Scene defaultScene;
    
    public enum GameState {MENU, INGAME}

    private GameState gameState = GameState.MENU;

    public AssetReference getCurrentSceneReference() {
        return currentSceneReference;
    }
    
    public bool isInMenu() {
        return gameState == GameState.MENU;
    }

    public bool isInGame() {
        return gameState == GameState.INGAME;
    }
    
    void Start() {
        defaultScene = SceneManager.GetActiveScene();
        LoadScene(startingScene);
    }

    public void LoadCurrentSave() {
        LoadScene(new AssetReference(SaveUtils.currentSaveGame.currentGameScene), GameState.INGAME);
    }

    public void LoadScene(AssetReference toLoad, GameState state = GameState.MENU) {
        UnloadScene();
        loadingScene.SetActive(true);
        Addressables.LoadSceneAsync(toLoad, LoadSceneMode.Additive, false).Completed += operationHandle => {
            currentScene = operationHandle.Result;
            currentSceneReference = toLoad;
            operationHandle.Result.ActivateAsync().completed += _ => {
                loadingScene.SetActive(false);
                gameState = state;
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

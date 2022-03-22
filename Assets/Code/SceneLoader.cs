using System;
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
    private AssetReference currentSceneReference;
    private AsyncOperationHandle currentSceneHandle;
    private Scene defaultScene;

    private static SceneLoader instance;

    private void Awake() {
        instance = this;
    }

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

    public static void StaticLoadCurrentSave() {
#if UNITY_EDITOR
        if (instance != null)
#endif
        instance.LoadCurrentSave();
    }

    public void LoadCurrentSave() {
        string sceneToLoad = SaveUtils.currentSaveGame.currentGameScene;
        if (String.IsNullOrEmpty(sceneToLoad)) sceneToLoad = currentSceneReference.AssetGUID;
        LoadScene(new AssetReference(sceneToLoad), GameState.INGAME);
    }

    public static void StaticLoadScene(AssetReference toLoad, GameState state = GameState.MENU) {
#if UNITY_EDITOR
        if (instance != null)
#endif
        instance.LoadScene(toLoad, state);
    }

    public void LoadScene(AssetReference toLoad, GameState state = GameState.MENU) {
        loadingScene.SetActive(true);
        if (!default(SceneInstance).Equals(currentScene)) {
            SceneManager.SetActiveScene(defaultScene);
            Addressables.UnloadSceneAsync(currentScene).Completed += _ => {
                if (currentSceneHandle.IsValid()) Addressables.Release(currentSceneHandle);
                currentScene = default;
                currentSceneReference = null;
                Resources.UnloadUnusedAssets().completed += _ => LoadSceneInternal(toLoad, state);
            };
        } else {
            LoadSceneInternal(toLoad, state);
        }
    }

    private void LoadSceneInternal(AssetReference toLoad, GameState state = GameState.MENU) {
        Addressables.LoadSceneAsync(toLoad, LoadSceneMode.Additive, false).Completed += operationHandle => {
            currentSceneHandle = operationHandle;
            currentScene = operationHandle.Result;
            currentSceneReference = toLoad;
            operationHandle.Result.ActivateAsync().completed += _ => {
                loadingScene.SetActive(false);
                gameState = state;
                if (state == GameState.INGAME)
                    SaveUtils.currentSaveGame.currentGameScene = toLoad.AssetGUID;
                foreach (var gameObject in operationHandle.Result.Scene.GetRootGameObjects()) {
                    SceneRoot component = gameObject.GetComponent<SceneRoot>();
                    if (component != null) component.parent = this;
                }
                
                SceneManager.SetActiveScene(currentScene.Scene);
                DynamicGI.UpdateEnvironment();
            };
        };
    }
}

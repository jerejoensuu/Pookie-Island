using System;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ViewManager : MonoBehaviour {

    [AssetsOnly, Required]
    public ViewRoot loadingView;
    private ViewRoot _currentView;
    
    [AssetsOnly, ValidateInput("@startingView.AssetGUID.Length != 0")]
    public AssetReference startingView;

    void Start() {
        loadingView = Instantiate(loadingView, transform.parent);
        loadingView.gameObject.SetActive(false);
        loadingView.name = loadingView.name.Remove(loadingView.name.LastIndexOf('('));
        TransitionTo(startingView);
    }

    public void TransitionTo(AssetReference toTransition) {
        if(_currentView != null) {
            Destroy(_currentView.gameObject);
        }

        loadingView.gameObject.SetActive(true);
        toTransition.LoadAssetAsync<GameObject>().Completed += OnCompleted;
    }

    private void OnCompleted(AsyncOperationHandle<GameObject> handle) {
        loadingView.gameObject.SetActive(false);
        if(handle.Status == AsyncOperationStatus.Succeeded) {
            GameObject instance = Instantiate(handle.Result, transform.parent);
            _currentView = instance.GetComponent<ViewRoot>();
            _currentView.viewManager = this;
            _currentView.name = handle.Result.name;
        }
    }
}

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public class TransitionToView : MonoBehaviour {

    public ViewRoot root;
    
    [AssetsOnly, ValidateInput("@TransitionTo.AssetGUID.Length != 0")]
    public AssetReference TransitionTo;

    #if UNITY_EDITOR
    private void Reset() {
        Button btn = GetComponent<Button>();
        RecursiveGetViewRoot(gameObject);
        if(btn != null) {
            UnityEventTools.AddVoidPersistentListener(btn.onClick, DoTransition);
        }
    }

    private void RecursiveGetViewRoot(GameObject current) {
        root = current.GetComponent<ViewRoot>();
        Transform parentTransform = current.transform.parent;
        if(root == null) {
            if(parentTransform == null) {
                Debug.LogError("Missing ViewRoot!");
            } else {
                RecursiveGetViewRoot(parentTransform.gameObject);
            }
        }
    }
    #endif

    public void DoTransition() {
        if (TransitionTo != null && root != null) root.viewManager.TransitionTo(TransitionTo);
    }
}

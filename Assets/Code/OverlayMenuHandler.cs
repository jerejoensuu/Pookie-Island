using UnityEngine;
using UnityEngine.InputSystem;

public class OverlayMenuHandler : MonoBehaviour {

    private Inputs inputs;
    public SceneLoader loader;
    public GameObject overlay;

    private void Start() {
        inputs = new Inputs();
        inputs.Enable();
        inputs.UI.Cancel.performed += CancelOnperformed;
    }

    private void OnDestroy() {
        inputs.UI.Cancel.performed -= CancelOnperformed;
        inputs.Dispose();
    }

    public void CloseOverlayMenu() {
        overlay.SetActive(false);
        GlobalEvent.UnpauseGame();
    }

    public void OpenOverlayMenu() {
        overlay.SetActive(true);
        GlobalEvent.PauseGame();
    }

    private void CancelOnperformed(InputAction.CallbackContext obj) { 
        if (!overlay.activeSelf && loader.isInGame()) OpenOverlayMenu();
    }
}

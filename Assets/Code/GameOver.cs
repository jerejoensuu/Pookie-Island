using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

    void OnEnable() {
        GameObject.Find("Menu Handler").GetComponent<OverlayMenuHandler>().inputs.Disable();
    }

    public void Continue() {
        Cursor.lockState = CursorLockMode.Locked;

        if (SaveUtils.health == 0) {
            // player.vacuum.tank.ResetTank();
            SaveUtils.health = 4;
            SceneLoader.StaticLoadCurrentSave();
        }

        Cursor.lockState = CursorLockMode.Confined;
        gameObject.SetActive(false);
        GlobalEvent.UnpauseGame();
        GameObject.Find("Menu Handler").GetComponent<OverlayMenuHandler>().inputs.Enable();
    }

    public void Exit() {
        Application.Quit();
    }

}

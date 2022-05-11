using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {

    public void Continue() {
        Cursor.lockState = CursorLockMode.Locked;

        if (SaveUtils.health == 0) {
            // player.vacuum.tank.ResetTank();
            SaveUtils.health = 4;
            SceneLoader.StaticLoadCurrentSave();
        }

        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }

    public void Exit() {

    }

}

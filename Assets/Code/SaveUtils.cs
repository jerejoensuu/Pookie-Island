using System;
using System.IO;
using Bolt;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class SaveUtils {

    public static SaveData currentSaveGame = ScriptableObject.CreateInstance<SaveData>().Initialize();

    public static void IncrementCrystal() {
        currentSaveGame.Crystal++;

        int totalCrystalAmount = 10;
        if (currentSaveGame.Crystal == totalCrystalAmount) {
            Debug.Log("asdf");
        }
    }

    public static int health {
        get => currentSaveGame.Health;
        set => currentSaveGame.Health = value;
    }

    private static string GetSaveFolder() {
        string path = Path.Combine(Application.persistentDataPath, "Savegames");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }

    public static void Save(String name, AssetReference currentSceneReference) {
        currentSaveGame.currentGameScene = currentSceneReference.AssetGUID;
        File.WriteAllText(Path.Combine(GetSaveFolder(), name) + ".sav", JsonUtility.ToJson(currentSaveGame));
    }

    public static void PopulateList(Button buttonPrefab, Transform parent, GameObject listener) {
        for (int childIndex = parent.childCount - 1; childIndex >= 0; childIndex--) {
            Object.Destroy(parent.GetChild(childIndex).gameObject);
        }
        
        string path = GetSaveFolder();
        foreach (var saveName in Directory.GetFiles(path)) {
            if (!Path.GetExtension(saveName).Equals(".sav")) continue;
            Button item = Object.Instantiate(buttonPrefab, parent);
            item.name = Path.GetFileNameWithoutExtension(saveName);
            item.onClick.AddListener(() => CustomEvent.Trigger(listener, "SaveFileSelected", item.name));
        }
    }

    public static void Load(String name) {
        JsonUtility.FromJsonOverwrite(File.ReadAllText(Path.Combine(GetSaveFolder(), name) + ".sav"), currentSaveGame);
    }
}


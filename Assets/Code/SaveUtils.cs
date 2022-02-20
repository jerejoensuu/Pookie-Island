using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveUtils {

    public static SaveData currentSaveGame = new SaveData().Initialize();

    public static void Save(String name) {
        string path = Path.Combine(Application.persistentDataPath, "Savegames");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "Savegames" , name), JsonUtility.ToJson(currentSaveGame));
    }

    public static void Load(String name) {
        currentSaveGame = JsonUtility.FromJson<SaveData>(
            File.ReadAllText(Path.Combine(Application.persistentDataPath, "Savegames", name)));
    }
}

[Serializable]
public struct SaveData {
    
    public List<String> Flags;

    public SaveData Initialize() {
        Flags = new List<string>();
        return this;
    }

}


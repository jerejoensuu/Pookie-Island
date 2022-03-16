using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "SaveData", order = 1)]
public class SaveData : ScriptableObject {

    public int Crystal;
    public int Health;
    public int gauge;
    public DamageElement.DamageType type;

    public List<String> Flags;

    public string currentGameScene;

    public SaveData Initialize() {
        Health = 4;
        Flags = new List<string>();
        return this;
    }
}

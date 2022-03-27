using UnityEngine;

public class Spawnable : MonoBehaviour {

    public GlobalSpawnHandler.SpawnableType type;

    private void OnEnable() {
        GlobalSpawnHandler.IncrementCount(type);
    }

    private void OnDisable() {
        GlobalSpawnHandler.DecrementCount(type);
    }
}

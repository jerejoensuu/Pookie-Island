using System;
using UnityEngine;

public class Shard : MonoBehaviour {

    private int uid;
    public static Action<int> onShardPicked;

    private void Awake() {
        uid = (int)(transform.position.x * 1327 + transform.position.z * 4673);
    }

    private void Start() {
        bool contains = SaveUtils.currentSaveGame.PickedShards.Contains(uid);
        gameObject.SetActive(!contains);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        SaveUtils.currentSaveGame.PickedShards.Add(uid);
        onShardPicked?.Invoke(++SaveUtils.currentSaveGame.Shards);
        Destroy(gameObject);
    }
}
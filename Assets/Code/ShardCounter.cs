using System;
using TMPro;
using UnityEngine;

public class ShardCounter : MonoBehaviour {
    
    private TextMeshProUGUI text;
    
    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        text.text = SaveUtils.currentSaveGame.Shards.ToString();
        Shard.onShardPicked += ONShardPicked;
    }

    private void OnDestroy() {
        Shard.onShardPicked -= ONShardPicked;
    }

    private void ONShardPicked(int count) {
        text.text = count.ToString();
    }
}
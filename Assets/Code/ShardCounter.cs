using TMPro;
using UnityEngine;

public class ShardCounter : MonoBehaviour {
    

    // dumb temp code
    public GameObject CrystalCounter;
    private TextMeshProUGUI text;
    [SerializeField] AudioClip shardSFX;
    //[SerializeField] AudioClip shardExchangeSFX;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        text.text = SaveUtils.currentSaveGame.Shards.ToString();
        Shard.onShardPicked += ONShardPicked;
    }

    private void OnDestroy() {
        Shard.onShardPicked -= ONShardPicked;
    }

    private void ONShardPicked(int count) {
        GetComponent<AudioSource>().PlayOneShot(shardSFX);
        if (count % 30 == 0) {
            SaveUtils.currentSaveGame.Exchanges = SaveUtils.currentSaveGame.Exchanges + 1;
            SaveUtils.IncrementCrystal();
            CrystalCounter.GetComponent<TextMeshProUGUI>().text = SaveUtils.currentSaveGame.Crystal.ToString();
        }
        if (SaveUtils.currentSaveGame.Exchanges > 0) {
            //GetComponent<AudioSource>().PlayOneShot(shardExchangeSFX);
            count = count - (30 * SaveUtils.currentSaveGame.Exchanges);
        }
        text.text = count.ToString();
    }
}
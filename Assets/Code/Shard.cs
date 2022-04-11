using System;
using UnityEngine;

public class Shard : MonoBehaviour {
    [SerializeField] float speed = 5f;
    [SerializeField] float height = 0.5f;
    Vector3 pos;
    private int uid;
    public static Action<int> onShardPicked;

    private void Awake() {
        uid = (int)(transform.position.x * 1327 + transform.position.z * 4673);
    }


    private void Start() {
        pos = transform.position;
        bool contains = SaveUtils.currentSaveGame.PickedShards.Contains(uid);
        gameObject.SetActive(!contains);
    }
    void Update()
    {
        float newY = Mathf.Sin(Time.time * speed) * height + pos.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        SaveUtils.currentSaveGame.PickedShards.Add(uid);
        onShardPicked?.Invoke(++SaveUtils.currentSaveGame.Shards);
        Destroy(gameObject);
    }
}


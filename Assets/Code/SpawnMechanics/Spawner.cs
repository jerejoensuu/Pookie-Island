using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {

    public GlobalSpawnHandler.SpawnableType type;
    
    [AssetsOnly, Required]
    public Spawnable prefab;
    public float radius;

    private void OnEnable() {
        GlobalSpawnHandler.RegisterSpawner(type, this);
    }

    private void OnDisable() {
        GlobalSpawnHandler.UnregisterSpawner(type, this);
    }

    public void DoSpawn() {
        Vector2 rndPoint =  Random.insideUnitCircle * radius;
        Vector3 spawnPoint = transform.position;
        spawnPoint.x += rndPoint.x;
        spawnPoint.z += rndPoint.y;

        RaycastHit hit;
        while (!Physics.Raycast(spawnPoint, Vector3.down, out hit)) ;
        Instantiate(prefab.gameObject, hit.point, Quaternion.identity, transform);
    }
}
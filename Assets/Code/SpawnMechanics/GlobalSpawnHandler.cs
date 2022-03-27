using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GlobalSpawnHandler : SerializedMonoBehaviour {

    private static GlobalSpawnHandler instance;

    public static void RegisterSpawner(SpawnableType type, Spawner toAdd) {
        instance.types[type].spawners.Add(toAdd);
    }
    
    public static void UnregisterSpawner(SpawnableType type, Spawner toRemove) {
        instance.types[type].spawners.Remove(toRemove);
    }

    public static void IncrementCount(SpawnableType type) {
        SpawnableInstance toChange = instance.types[type];
        if (++toChange.count < toChange.threshold) {
            instance.StartSpawnRoutine(toChange);
        }
    }

    public static void DecrementCount(SpawnableType type) {
        SpawnableInstance toChange = instance.types[type];
        if (--toChange.count < toChange.threshold) {
            instance.StartSpawnRoutine(toChange);
        }
    }

    public static void ChangeThreshold(SpawnableType type, int byThisAmount) {
        SpawnableInstance toChange = instance.types[type];
        toChange.threshold += byThisAmount;
        if (toChange.threshold - toChange.count <= 0) return;
        instance.StartSpawnRoutine(toChange);
    }

    private void StartSpawnRoutine(SpawnableInstance toSpawn) {
        if (this == null || toSpawn.spawners.Count == 0) return;
        StartCoroutine(SpawnRoutine(toSpawn));
    }
    
    private static Random rng = new Random(666);

    private IEnumerator SpawnRoutine(SpawnableInstance toSpawn) {
        yield return new WaitForSeconds(rng.NextFloat(minimumRespawnTimeSeconds, maximumRespawnTimeSeconds));
        if (toSpawn.count < toSpawn.threshold) toSpawn.spawners.ElementAt(rng.NextInt(toSpawn.spawners.Count - 1)).DoSpawn();
    }

    public enum SpawnableType {FIRE_POOKIE, ICE_POOKIE, WATER_POOKIE, BULLET_POOKIE}

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine)]
    public Dictionary<SpawnableType, SpawnableInstance> types = new Dictionary<SpawnableType, SpawnableInstance>();

    public float minimumRespawnTimeSeconds = 2;
    public float maximumRespawnTimeSeconds = 10;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        foreach (var value in types.Values) {
            if (value.threshold - value.count <= 0) continue;
            instance.StartSpawnRoutine(value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class SpawnableInstance {
    
    public int threshold;
    
    [NonSerialized] public int count;

    [NonSerialized]
    public HashSet<Spawner> spawners;
    
    [OnDeserialized]
    private void OnDeserialized() {
        spawners = new HashSet<Spawner>();
    }
}
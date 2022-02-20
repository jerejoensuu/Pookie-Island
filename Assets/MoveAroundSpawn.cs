using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAroundSpawn : MonoBehaviour {

    public float range = 10f;
    public GameObject spawnPoint;
    private NavMeshAgent agent;
    
    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (agent.pathPending || agent.remainingDistance > 0.1f)
            return;
        Vector3 rand = Random.insideUnitCircle;
        rand = new Vector3(rand.x, 0, rand.y);
        agent.destination = spawnPoint.transform.position + (Vector3)(range * rand);
    }
}

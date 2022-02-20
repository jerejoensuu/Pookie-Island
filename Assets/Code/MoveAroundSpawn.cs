using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAroundSpawn : MonoBehaviour {

    public float range = 10f;
    public GameObject spawnPoint;
    private NavMeshAgent agent;
    private GameObject player;
    
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void MoveToRandomPlace() {
        Vector3 rand = Random.insideUnitCircle;
        rand = new Vector3(rand.x, 0, rand.y);
        agent.destination = spawnPoint.transform.position + (Vector3)(range * rand);
    }

    public bool IsPlayerTooClose(float distance) {
        if (player == null) return false;
        return (transform.position - player.transform.position).sqrMagnitude < distance * distance;
    }

    public void FleeFromPlayer() {
        if (player == null) return;
        agent.destination = transform.position - player.transform.position + transform.position;
    }
}

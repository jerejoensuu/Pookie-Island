using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MoveAroundSpawn : MonoBehaviour {

    public float range = 10f;
    private Vector3 spawnPoint;
    private NavMeshAgent agent;
    private GameObject player;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        spawnPoint = transform.position;
    }

    private float counter;
    private Quaternion fromDirection = Quaternion.identity;
    private Quaternion toDirection = Quaternion.identity;

    public void FacePlayer(bool restart) {
        if (player == null) return;
        if (restart) {
            counter = 0;
            fromDirection = transform.rotation;
            Vector3 lookAt = player.transform.position - transform.position;
            toDirection = Quaternion.LookRotation(new Vector3(lookAt.x, 0, lookAt.z), Vector3.up);
        }

        counter += Time.deltaTime;

        transform.rotation = Quaternion.Lerp(fromDirection, toDirection, counter);
    }

    public void MoveToRandomPlace() {
        Vector3 rand = Random.insideUnitCircle;
        rand = new Vector3(rand.x, 0, rand.y);
        agent.destination = spawnPoint + (Vector3)(range * rand);
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

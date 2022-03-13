using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackHandler : MonoBehaviour {
    
    [SerializeField] PlayerController player;
    [SerializeField] float knockbackForce = 1;
    Vector3 knockbackHor = Vector3.zero;
    float knockbackY = 0;

    public void SetKnockback(ControllerColliderHit hit) {
        if (player.movement.knockedBack == true) return;
        player.movement.knockedBack = true;
        player.movement.movement = Vector3.zero;
        player.vacuum.elements.use = false;
        player.vacuum.pull = false;

        Vector3 damageSource = hit.collider.ClosestPoint(player.model.transform.position);
        damageSource = new Vector3(damageSource.x, 0, damageSource.z);
        Vector3 playerPos = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        knockbackHor = (playerPos - damageSource).normalized;
        if (knockbackHor.magnitude == 0) knockbackHor = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;
        knockbackHor *= knockbackForce;
        player.movement.movement.y = player.jumpSpeed * 0.4f;
    }

    public void HandleKnockback() {
        player.movement.controller.Move(new Vector3(knockbackHor.x, player.movement.movement.y, knockbackHor.z) * Time.deltaTime);
    }


}
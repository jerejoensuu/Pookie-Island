using System;
using Bolt;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] PlayerController player;
    public HealthBar healthBar;
    public SkinnedMeshRenderer playerMesh;
    public int cooldown = 300;
    public bool onCooldown = false;

    private void Start() {
        healthBar.SetLives(SaveUtils.currentSaveGame.Health);
    }

    public void GainHealth(int gainAmount = 1) {
        SaveUtils.health = Mathf.Min(healthBar.MaximumHearts, SaveUtils.health + gainAmount);
        healthBar.SetLives(SaveUtils.health);
    }

    public void TakeDamage(int damageAmount = 1) {
        SaveUtils.health -= damageAmount;
        if (SaveUtils.health <= 0) {
            StartCoroutine(KillPlayer());
        }
        healthBar.SetLives(SaveUtils.health);
    }

    public IEnumerator KillPlayer() {
        player.anim.animator.SetTrigger("death");
        yield return new WaitForSeconds(3);
        GameObject.Find("Root").GetComponent<SceneRoot>().parent.GetComponent<SceneLoader>().GameOver();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (onCooldown) return;

        if (hit.gameObject.TryGetComponent(out PlayerHealthAffect affectedBy)) {
            Debug.Log("take damage");
            if (affectedBy.effectType == PlayerHealthAffect.EffectType.DAMAGE) {
                TakeDamage(affectedBy.effectAmount);
            } else {
                GainHealth(affectedBy.effectAmount);
            }
            StartCoroutine(DamageCooldown());
            player.knockbackHandler.SetKnockback(hit);
            player.knockbackHandler.HandleKnockback();

            affectedBy.OnPlayerHit();
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == 9) {
            if (SaveUtils.health <= 0) {
                StartCoroutine(KillPlayer());
            } else {
                TakeDamage(1);
                player.SoftRespawn();
            }
        }
    }

    IEnumerator DamageCooldown() {
        int c = cooldown;
        onCooldown = true;
        while (c > 0) {
            c--;
            if (c % 20 == 0) playerMesh.enabled = !playerMesh.enabled;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        playerMesh.enabled = true;
        onCooldown = false;
    }
}

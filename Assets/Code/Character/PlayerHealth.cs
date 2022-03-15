using System;
using Bolt;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] PlayerController player;
    public HealthBar healthBar;
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
            SaveUtils.health = healthBar.MaximumHearts;
            // player.anim.animator.SetTrigger("death");
            SceneLoader.StaticLoadCurrentSave();
        }
        healthBar.SetLives(SaveUtils.health);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (onCooldown) return;
        if (hit.gameObject.TryGetComponent(out PlayerHealthAffect affectedBy)) {
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

    IEnumerator DamageCooldown() {
        int c = cooldown;
        onCooldown = true;
        while (c > 0) {
            c--;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        onCooldown = false;
    }
}

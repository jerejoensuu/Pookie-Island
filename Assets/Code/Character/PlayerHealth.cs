using System;
using Bolt;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public HealthBar healthBar;

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
            SceneLoader.StaticLoadCurrentSave();
        }
        healthBar.SetLives(SaveUtils.health);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.TryGetComponent(out PlayerHealthAffect affectedBy)) {
            if (affectedBy.effectType == PlayerHealthAffect.EffectType.DAMAGE) {
                TakeDamage(affectedBy.effectAmount);
            } else {
                GainHealth(affectedBy.effectAmount);
            }
            //TODO: enter invulnerability
            //TODO: pass hit to Jere
            
            affectedBy.OnPlayerHit();
        }
    }
}

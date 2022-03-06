using System;
using UnityEngine;

public class PlayerHealthAffect : MonoBehaviour {
    public bool destroyOnPlayerCollision = true;
    public enum EffectType {HEAL, DAMAGE}
    public EffectType effectType = EffectType.DAMAGE;
    public int effectAmount = 1;

    public void OnPlayerHit() {
        if (destroyOnPlayerCollision) {
            Destroy(gameObject);
        }
    }
}

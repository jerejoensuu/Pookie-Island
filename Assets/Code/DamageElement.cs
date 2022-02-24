using UnityEngine;

public class DamageElement : MonoBehaviour {
    public enum DamageType {FIRE, WATER, ICE, BULLET}
    public DamageType damageType;

    public bool IsSameDamageType(DamageType toCheck) {
        return damageType == toCheck;
    }

    public static bool HasDamageType(Collider toCheck, DamageType type) {
        DamageElement other = toCheck.gameObject.GetComponent<DamageElement>();
        if (other == null) return false;
        return other.IsSameDamageType(type);
    }
}
using UnityEngine;

public class DamageElement : MonoBehaviour {
    public enum DamageType {FIRE, WATER, ICE, BULLET}
    public DamageType damageType;

    public bool IsSameDamageType(DamageType toCheck) {
        return damageType == toCheck;
    }

    public static bool isFire(DamageType type) {
        return type == DamageType.FIRE;
    }
    
    public static bool isIce(DamageType type) {
        return type == DamageType.ICE;
    }
    
    public static bool HasDamageType(Collider toCheck, DamageType type) {
        DamageElement other = toCheck.gameObject.GetComponent<DamageElement>();
        if (other == null) return false;
        return other.IsSameDamageType(type);
    }
}
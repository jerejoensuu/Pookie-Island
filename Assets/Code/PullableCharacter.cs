using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PullableCharacter : MonoBehaviour {
    public Pauser characterParent;
    public StateMachine behaviour;
    public DamageElement.DamageType type;

    public void enterPulledMode() {
        CustomEvent.Trigger(characterParent.gameObject, "StartPull");
    }
    
    public void exitPulledMode() {
        CustomEvent.Trigger(characterParent.gameObject,"StopPull");
    }
    
#if UNITY_EDITOR
    [Sirenix.OdinInspector.OnInspectorInit]
    void Bind() {
        characterParent = GetComponentInParent<Pauser>();
        behaviour = GetComponentInParent<StateMachine>();
        if (gameObject.tag.Contains("Fire")) {
            type = DamageElement.DamageType.FIRE;
        } else if (gameObject.tag.Contains("Ice")) {
            type = DamageElement.DamageType.ICE;
        } else if (gameObject.tag.Contains("Water")) {
            type = DamageElement.DamageType.WATER;
        } else if (gameObject.tag.Contains("Bullet")) {
            type = DamageElement.DamageType.BULLET;
        }
    }
#endif
}

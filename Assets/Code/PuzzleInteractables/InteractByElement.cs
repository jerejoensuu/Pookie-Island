using System;
using Bolt;
using UnityEngine;

public class InteractByElement : Interactable {

    public bool requiresReset;
    private bool completed;
    public DamageElement.DamageType type = DamageElement.DamageType.WATER;
    private EventHook eventHook;

    private void Awake() {
        if (type == DamageElement.DamageType.FIRE) EventBus.Register<CustomEventArgs>(eventHook = new EventHook("Custom", gameObject), DestroyFire);
        else if (type == DamageElement.DamageType.ICE) EventBus.Register<CustomEventArgs>(eventHook = new EventHook("Custom", gameObject), DestroyIce);
    }

    private void OnDestroy() {
        if (type == DamageElement.DamageType.FIRE) EventBus.Unregister(eventHook, (Action<CustomEventArgs>) DestroyFire);
        else if (type == DamageElement.DamageType.ICE) EventBus.Unregister(eventHook, (Action<CustomEventArgs>) DestroyIce);
    }

    public void ResetState() {
        completed = false;
        OnReset?.Invoke(this);
    }

    private void Triggered() {
        if (requiresReset && completed) return;
        completed = true;
        OnInteraction?.Invoke(this);
    }

    private void DestroyFire(CustomEventArgs args) {
        if (args.name == "DestroyFire") Triggered();
    }
    
    private void DestroyIce(CustomEventArgs args) {
        if (args.name == "DestroyIce") Triggered();
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.TryGetComponent(out DamageElement component) && component.damageType == type) {
            Triggered();
        }
    }
}

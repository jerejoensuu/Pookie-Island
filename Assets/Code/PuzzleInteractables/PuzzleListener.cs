using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PuzzleListener : MonoBehaviour {
    
#if UNITY_EDITOR
    private void Reset() {
        if (animator == null) TryGetComponent(out animator);
        if (interactable == null) TryGetComponent(out interactable);
    }
#endif

    public Interactable interactable;

    public enum State { ON, OFF }

    public enum Action { ANIMATOR_TRIGGER, SET_FLAG, ACTIVATE_CHILD, DEACTIVATE_CHILD, SELF_DESTROY, SOFT_RESET_ON_TIMER }

    public Action action = Action.ANIMATOR_TRIGGER;
    
    public State state = State.ON;

    [ShowIf("@action==Action.SET_FLAG || action==Action.ANIMATOR_TRIGGER")]
    public string value;
    [ShowIf("@action==Action.ANIMATOR_TRIGGER")]
    public Animator animator;
    [ShowIf("@action==Action.ACTIVATE_CHILD || action==Action.DEACTIVATE_CHILD")]
    public GameObject child;
    [ShowIf("@action==Action.SOFT_RESET_ON_TIMER")]
    public float timer;
    
    
    private void Awake() {
        if (state == State.ON) interactable.OnInteraction += Execute;
        else interactable.OnReset += Execute;
    }

    private void OnDestroy() {
        if (state == State.ON) interactable.OnInteraction -= Execute;
        else interactable.OnReset -= Execute;
    }

    private IEnumerator OnTimerDone() {
        yield return new WaitForSeconds(timer);
        interactable.OnReset?.Invoke(interactable);
    }

    private void Execute(Interactable ignored) {
        switch (action) {
            case Action.SET_FLAG:
                FlagUtils.SetFlag(value);
                break;
            case Action.ANIMATOR_TRIGGER:
                animator.SetTrigger(value);
                break;
            case Action.ACTIVATE_CHILD:
                child.SetActive(true);
                break;
            case Action.SELF_DESTROY:
                Destroy(gameObject);
                break;
            case Action.DEACTIVATE_CHILD:
                child.SetActive(false);
                break;
            case Action.SOFT_RESET_ON_TIMER:
                StartCoroutine(OnTimerDone());
                break;
        }
    }
}

using System;
using System.Collections.Generic;

public class PuzzleController : Interactable {

    public bool isSequential;

    public List<Interactable> interactionsRequiredToComplete;
    public List<Interactable> interactionsThatResetPartialCompletion;
    public List<Interactable> interactionsThatHardResetCompletion;

    internal HashSet<Interactable> set = new HashSet<Interactable>();
    internal bool completed;

    private void Awake() {
        foreach (Interactable interactable in interactionsRequiredToComplete) {
            if (isSequential) interactable.OnInteraction += OnSequentialSet;
            else interactable.OnInteraction += OnSet;
        }

        foreach (Interactable interactable in interactionsThatResetPartialCompletion) {
            interactable.OnInteraction += OnUnset;
        }

        foreach (Interactable interactable in interactionsThatHardResetCompletion) {
            interactable.OnInteraction += OnHardReset;
        }
    }

    private void OnDestroy() {
        foreach (Interactable interactable in interactionsRequiredToComplete) {
            if (isSequential) interactable.OnInteraction -= OnSequentialSet;
            else interactable.OnInteraction -= OnSet;
        }

        foreach (Interactable interactable in interactionsThatResetPartialCompletion) {
            interactable.OnInteraction -= OnUnset;
        }

        foreach (Interactable interactable in interactionsThatHardResetCompletion) {
            interactable.OnInteraction -= OnHardReset;
        }
    }

    internal void OnHardReset(Interactable _) {
        if (completed) {
            foreach (Interactable interactable in interactionsRequiredToComplete) {
                if (isSequential) interactable.OnInteraction += OnSequentialSet;
                else interactable.OnInteraction += OnSet;
            }

            foreach (Interactable interactable in interactionsThatResetPartialCompletion) {
                interactable.OnInteraction += OnUnset;
            }
        }

        completed = false;
        set.Clear();
        OnReset?.Invoke(this);
    }
    
    internal void OnSequentialSet(Interactable interactable) {
        if (completed) return;
        if (interactionsRequiredToComplete[set.Count].Equals(interactable)) {
            set.Add(interactable);
            CheckCompletion();
        } else {
            ResetState();
        }
    }

    internal void ResetState() {
        foreach (var toReset in interactionsRequiredToComplete) {
            if (toReset is InteractByElement {requiresReset: true} resettable) resettable.ResetState();
        }
        set.Clear();
        OnReset?.Invoke(this);
    }

    internal void CheckCompletion() {
        if (set.Count == interactionsRequiredToComplete.Count) {
            foreach (Interactable toRemove in interactionsRequiredToComplete) {
                if (isSequential) toRemove.OnInteraction -= OnSequentialSet;
                else toRemove.OnInteraction -= OnSet;
            }

            foreach (Interactable toRemove in interactionsThatResetPartialCompletion) {
                toRemove.OnInteraction -= OnUnset;
            }

            completed = true;
            OnInteraction?.Invoke(this);
        }
    }

    internal void OnSet(Interactable interactable) {
        if (completed) return;
        set.Add(interactable);
        CheckCompletion();
    }

    internal void OnUnset(Interactable interactable) {
        if (completed) return;
        ResetState();
    }
}

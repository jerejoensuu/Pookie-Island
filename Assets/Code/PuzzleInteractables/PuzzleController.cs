using System.Collections.Generic;

public class PuzzleController : Interactable {

    public List<Interactable> interactionsRequiredToComplete;
    public List<Interactable> interactionsThatResetPartialCompletion;
    public List<Interactable> interactionsThatHardResetCompletion;

    private HashSet<Interactable> set = new HashSet<Interactable>();
    private bool completed = false;

    private void Awake() {
        foreach (Interactable interactable in interactionsRequiredToComplete) {
            interactable.OnInteraction += OnSet;
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
            interactable.OnInteraction -= OnSet;
        }
        foreach (Interactable interactable in interactionsThatResetPartialCompletion) {
            interactable.OnInteraction -= OnUnset;
        }
        foreach (Interactable interactable in interactionsThatHardResetCompletion) {
            interactable.OnInteraction -= OnHardReset;
        }
    }

    private void OnHardReset(Interactable _) {
        if (completed) {
            foreach (Interactable interactable in interactionsRequiredToComplete) {
                interactable.OnInteraction += OnSet;
            }
            foreach (Interactable interactable in interactionsThatResetPartialCompletion) {
                interactable.OnInteraction += OnUnset;
            }
        }
        completed = false;
        set.Clear();
        OnReset?.Invoke(this);
    }

    private void OnSet(Interactable interactable) {
        if (completed) return;
        set.Add(interactable);
        if (set.Count == interactionsRequiredToComplete.Count) {
            foreach (Interactable toRemove in interactionsRequiredToComplete) {
                toRemove.OnInteraction -= OnSet;
            }
            foreach (Interactable toRemove in interactionsThatResetPartialCompletion) {
                toRemove.OnInteraction -= OnUnset;
            }

            completed = true;
            OnInteraction?.Invoke(this);
        }
    }
    
    private void OnUnset(Interactable interactable) {
        if (completed) return;
        set.Clear();
        OnReset?.Invoke(this);
    }
}

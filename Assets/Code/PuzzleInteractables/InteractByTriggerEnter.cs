using UnityEngine;

public class InteractByTriggerEnter : Interactable {
    public string tagToTrigger;
    public int amountToTrigger = 1;
    private int currentAmount = 0;

    private void OnTriggerEnter(Collider other) {
        if (other.tag.Equals(tagToTrigger)) {
            if (++currentAmount == amountToTrigger) {
                OnInteraction?.Invoke(this);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals(tagToTrigger)) {
            currentAmount--;
        }
    }
}

using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    public delegate void Interact(Interactable self);

    public Interact OnInteraction;
    public Interact OnReset;

}
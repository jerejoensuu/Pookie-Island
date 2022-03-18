using System;
using System.Linq;
using Bolt;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour {

    public Text dialogueText;
    public GameObject dialoguePanel;
    public InputReader playerControls;
    public GameObject hoveringText;
    
    private Inputs inputs;
    private Dialogue target;
    private PieceOfDialogue currentDialogue;
    private int currentDialoguePosition;
    private State state = State.NO_DIALOGUE;

    private enum State { IN_DIALOGUE, NO_DIALOGUE, MAYBE_DIALOGUE }

    private void Start() {
        inputs = new Inputs();
        inputs.Enable();
        dialoguePanel.SetActive(false);
        hoveringText.SetActive(false);
    }

    private void OnDisable() {
        inputs.Player.Jump.performed -= ContinueDialogue;
        inputs.Player.Interact.performed -= Interaction;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (hoveringText != null) hoveringText.SetActive(false);
    }

    private void ContinueDialogue(InputAction.CallbackContext obj) {
        if (currentDialogue.dialogueBoxes.Count == ++currentDialoguePosition) {
            dialoguePanel.SetActive(false);
            state = State.MAYBE_DIALOGUE;
            DisplayHoveringText(target.transform);
            inputs.Player.Jump.performed -= ContinueDialogue;
            inputs.Player.Interact.performed += Interaction;
            dialogueText.text = "";
            if (!String.IsNullOrEmpty(currentDialogue.FlagToSet)) FlagUtils.SetFlag(currentDialogue.FlagToSet);
            Pauser.ResumeAll();
            playerControls.enabled = true;
            CustomEvent.Trigger(target.behaviour.gameObject, "DialogueEnded");
        } else {
            dialogueText.text = currentDialogue.dialogueBoxes[currentDialoguePosition];
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (state == State.IN_DIALOGUE) return;
        if (other.gameObject.TryGetComponent(out Dialogue toCheck)) {
            state = State.MAYBE_DIALOGUE;
            inputs.Player.Interact.performed += Interaction;
            target = toCheck;
            DisplayHoveringText(target.transform);
        }
    }

    private void DisplayHoveringText(Transform parent) {
        hoveringText.SetActive(true);
        hoveringText.transform.SetParent(parent);
        hoveringText.transform.localPosition = new Vector3(0, 2, 0);
    }

    private void HideHoveringText() {
        hoveringText.SetActive(false);
        hoveringText.transform.SetParent(transform);
    }

    private void Interaction(InputAction.CallbackContext obj) {
        PieceOfDialogue validDialogue = null;
        foreach (PieceOfDialogue pieceOfDialogue in target.dialogues) {
            foreach (string flag in pieceOfDialogue.disabledByFlags) {
                if (FlagUtils.IsFlagOn(flag)) goto outer;
            }
            
            foreach (string flag in pieceOfDialogue.requiredFlags) {
                if (!FlagUtils.IsFlagOn(flag)) goto outer;
            }

            if (validDialogue == null || validDialogue.priority < pieceOfDialogue.priority) validDialogue = pieceOfDialogue;
            outer:;
        }

        if (validDialogue != null) {
            state = State.IN_DIALOGUE;
            dialoguePanel.SetActive(true);
            inputs.Player.Interact.performed -= Interaction;
            inputs.Player.Jump.performed += ContinueDialogue;
            HideHoveringText();
            currentDialogue = validDialogue;
            dialogueText.text = currentDialogue.dialogueBoxes.First();
            currentDialoguePosition = 0;
            target.rootPauser.PauseAllExceptMe();
            playerControls.enabled = false;
            CustomEvent.Trigger(target.behaviour.gameObject, "DialogueStarted");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (state == State.IN_DIALOGUE) return;
        if (other.gameObject.TryGetComponent(out Dialogue toCheck)) {
            if (toCheck.Equals(target)) {
                state = State.NO_DIALOGUE;
                HideHoveringText();
                inputs.Player.Interact.performed -= Interaction;
                target = null;
            }
        }
    }
}

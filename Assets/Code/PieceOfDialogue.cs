using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PieceOfDialogue", menuName = "PieceOfDialogue", order = 1)]
public class PieceOfDialogue : ScriptableObject {
    public int priority;
    public List<string> requiredFlags;
    public List<string> disabledByFlags;
    public List<string> dialogueBoxes;
    public string FlagToSet;
}

using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Dialogue : MonoBehaviour {

    public Pauser rootPauser;
    public StateMachine behaviour;
    public List<PieceOfDialogue> dialogues;
    
#if UNITY_EDITOR
    [Sirenix.OdinInspector.OnInspectorInit]
    void Bind() {
        rootPauser = GetComponentInParent<Pauser>();
        behaviour = GetComponentInParent<StateMachine>();
    }
#endif


    public static Dialogue getDialogue(GameObject toCheck) {
        return toCheck.GetComponent<Dialogue>();
    }

    public static bool isSameDialogue(GameObject toCheck, Dialogue against) {
        Dialogue other = toCheck.GetComponent<Dialogue>();
        return other != null && other.Equals(against);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    
    [SerializeField] PlayerController player;

    internal Animator animator;


    void Start() {
        animator = player.model.GetComponent<Animator>();
    }

    void Update() {
        
    }
}

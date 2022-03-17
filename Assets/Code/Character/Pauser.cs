using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Pauser : MonoBehaviour {

    public List<Behaviour> componentsToPause;

    private delegate void ExecuteOnAllExcept();

#if UNITY_EDITOR
    [Sirenix.OdinInspector.OnInspectorInit]
    void Bind() {
        componentsToPause = new List<Behaviour>();
        componentsToPause.AddRange(GetComponentsInChildren<Animator>());
        componentsToPause.AddRange(GetComponentsInChildren<StateMachine>());
        componentsToPause.AddRange(GetComponentsInChildren<UnityEngine.AI.NavMeshAgent>());
    }
#endif

    private static ExecuteOnAllExcept stopAll;
    private static ExecuteOnAllExcept resumeAll;

    public void PauseAllExceptMe() {
        stopAll -= DoStop;
        stopAll?.Invoke();
        stopAll += DoStop;
    }
    
    public static void ResumeAll() {
        resumeAll?.Invoke();
    }

    private void Start() {
        stopAll += DoStop;
        resumeAll += DoResume;
    }

    private void OnDestroy() {
        stopAll -= DoStop;
        resumeAll -= DoResume;
    }

    public void DoStop() {
        foreach (var behaviour in componentsToPause) {
            behaviour.enabled = false;
        }
    }

    public void DoResume() {
        foreach (var behaviour in componentsToPause) {
            behaviour.enabled = true;
        }
    }
}

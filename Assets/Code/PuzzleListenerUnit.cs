using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;

[UnitCategory("Events")]
[UnitTitle("Puzzle Listener")]
public class PuzzleListenerUnit : EventUnit<bool> {

    public ValueInput interactable;
    private GraphReference graphReference;

    private struct EventInstance {
        public Interactable interactable;
        public Interactable.Interact onInteraction;
        public Interactable.Interact onReset;
    }

    [DoNotSerialize] private Dictionary<GraphReference, EventInstance> instances = new Dictionary<GraphReference, EventInstance>();
    
    [DoNotSerialize] public ControlInput setListener { get; private set; }
    
    [DoNotSerialize] public ControlOutput onReset { get; private set; }
    
    [DoNotSerialize] public ControlOutput onInteraction { get; private set; }
    protected override bool register => false;

    protected override void Definition() {
        isControlRoot = true;
        setListener = ControlInput(nameof(setListener), SetListener);
        onInteraction = ControlOutput(nameof(onInteraction));
        onReset = ControlOutput(nameof(onReset));
        interactable = ValueInput<Interactable>(nameof(interactable));
        
        //Succession(setListener, trigger);
        //Requirement(interactable, setListener);
    }

    public override void StopListening(GraphStack stack) {
        GraphReference instance = stack.AsReference();
        if (!instances.ContainsKey(instance)) return;
        EventInstance listeningTo = instances[instance];
        listeningTo.interactable.OnInteraction -= listeningTo.onInteraction;
        listeningTo.interactable.OnReset -= listeningTo.onReset;
        instances.Remove(instance);
    }

    private ControlOutput SetListener(Flow arg) {
        GraphReference graphReference = arg.stack.AsReference();
        Interactable changeTo = arg.GetValue<Interactable>(interactable);
        EventInstance listeningTo;
        if (instances.ContainsKey(graphReference)) {
            listeningTo = instances[graphReference];
            if (listeningTo.interactable == changeTo) return trigger;
            listeningTo.interactable.OnInteraction -= listeningTo.onInteraction;
            listeningTo.interactable.OnReset -= listeningTo.onReset;
            listeningTo.interactable = changeTo;
        } else {
            listeningTo = new EventInstance {interactable = changeTo};
            instances.Add(graphReference, listeningTo);
        }
        listeningTo.onInteraction = _ => Run(onInteraction, graphReference);
        listeningTo.onReset = _ => Run(onReset, graphReference);
        listeningTo.interactable.OnInteraction += listeningTo.onInteraction;
        listeningTo.interactable.OnReset += listeningTo.onReset;
        return trigger;
    }

    private void Run(ControlOutput toRun, GraphReference graphReference) {
        Flow flow = Flow.New(graphReference);
        if (flow.enableDebug) {
            IUnitDebugData elementDebugData = flow.stack.GetElementDebugData<IUnitDebugData>(this);
            elementDebugData.lastInvokeFrame = EditorTimeBinding.frame;
            elementDebugData.lastInvokeTime = EditorTimeBinding.time;
        }
        flow.Run(toRun);
    }
}
using Bolt;
using Ludiq;

[UnitCategory("Events")]
[UnitTitle("Puzzle Listener")]
public class PuzzleListenerUnit : Unit {

    public ValueInput interactable;
    private GraphReference graphReference;

    [DoNotSerialize] private Interactable listeningTo;
    
    [DoNotSerialize] public ControlInput setListener { get; private set; }
    
    [DoNotSerialize] public ControlOutput onReset { get; private set; }
    
    [DoNotSerialize] public ControlOutput onInteraction { get; private set; }
    
    [DoNotSerialize, PortLabel("Exit")] public ControlOutput trigger { get; private set; }

    protected override void Definition() {
        isControlRoot = true;
        trigger = ControlOutput(nameof(trigger));
        setListener = ControlInput(nameof(setListener), SetListener);
        onInteraction = ControlOutput(nameof(onInteraction));
        onReset = ControlOutput(nameof(onReset));
        interactable = ValueInput<Interactable>(nameof(interactable));
        
        Succession(setListener, trigger);
        /*Succession(setListener, onInteraction);
        Succession(setListener, onReset);*/
        Requirement(interactable, setListener);
    }

    private ControlOutput SetListener(Flow arg) {
        graphReference = arg.stack.AsReference();
        Interactable changeTo = arg.GetValue<Interactable>(interactable);
        if (listeningTo != null) {
            if (listeningTo == changeTo) return trigger;
            listeningTo.OnInteraction -= ListenerUp;
            listeningTo.OnReset -= ListenerDown;
        }
        listeningTo = changeTo;
        if (listeningTo != null) {
            listeningTo.OnInteraction += ListenerUp;
            listeningTo.OnReset += ListenerDown;
        }
        return trigger;
    }
    
    private void ListenerUp(Interactable data) {
        Run(onInteraction);
    }

    private void ListenerDown(Interactable data) {
        Run(onReset);
    }

    public override void Uninstantiate(GraphReference instance) {
        base.Uninstantiate(instance);
        if (listeningTo != null) {
            listeningTo.OnInteraction -= ListenerUp;
            listeningTo.OnReset -= ListenerDown;
        }
    }

    private void Run(ControlOutput toRun) {
        Flow flow = Flow.New(graphReference);
        if (flow.enableDebug) {
            IUnitDebugData elementDebugData = flow.stack.GetElementDebugData<IUnitDebugData>(this);
            elementDebugData.lastInvokeFrame = EditorTimeBinding.frame;
            elementDebugData.lastInvokeTime = EditorTimeBinding.time;
        }
        flow.Run(toRun);
    }
}
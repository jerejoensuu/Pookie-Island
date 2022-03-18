using System.Collections.Generic;
using Bolt;
using Ludiq;

public interface IRunner {
    void Run(DelegateHandler context, ControlOutput port);
    void Run(DelegateHandler context);
}

public abstract class DelegateEventUnit<T> : EventUnit<bool>, IRunner where T : DelegateHandler {
    
    protected override bool register => false;
    protected abstract T InstantiateHandler();
    [DoNotSerialize] private Dictionary<GraphReference, T> instances = new Dictionary<GraphReference, T>();

    public override void StartListening(GraphStack stack) {
        GraphReference graphReference = stack.AsReference();
        if (instances.TryGetValue(graphReference, out T instance)) instance.UnassignListeners();
        T newInstance = instances[graphReference] = InstantiateHandler();
        newInstance.graph = graphReference;
        newInstance.unit = this;
        newInstance.AssignListeners();
    }

    public override void StopListening(GraphStack stack) {
        GraphReference graphReference = stack.AsReference();
        if (instances.TryGetValue(graphReference, out T instance)) {
            instance.UnassignListeners();
            instances.Remove(graphReference);
        }
    }
    
    public void Run(DelegateHandler context, ControlOutput port) {
        Flow flow = Flow.New(context.graph);
        if (flow.enableDebug) {
            IUnitDebugData elementDebugData = flow.stack.GetElementDebugData<IUnitDebugData>(this);
            elementDebugData.lastInvokeFrame = EditorTimeBinding.frame;
            elementDebugData.lastInvokeTime = EditorTimeBinding.time;
        }
        flow.Run(port);
    }

    public void Run(DelegateHandler context) {
        Run(context, trigger);
    }
}
using System;
using Bolt;
using Ludiq;

public class TestEvent : EventUnit<TestEventData> {
    
    [DoNotSerialize]
    public ValueOutput result { get; private set; }

    public static Action<TestEventData> listeners;
    GraphReference graphReference;

    public static void TriggerAll(string value) {
        listeners?.Invoke(new TestEventData{whatever = value});
    }

    protected override bool register => false;
    
    protected override void Definition() {
        base.Definition();
        result = ValueOutput<TestEventData>(nameof(result));
    }
    
    protected override void AssignArguments(Flow flow, TestEventData data) {
        flow.SetValue(result, data);
    }

    public override void StartListening(GraphStack stack) {
        graphReference = stack.AsReference();
        Data elementData = stack.GetElementData<Data>(this);
        if (elementData.isListening) return;
        listeners += Listener;
        elementData.isListening = true;
    }

    public override void StopListening(GraphStack stack) {
        Data elementData = stack.GetElementData<Data>(this);
        if (!elementData.isListening) return;
        listeners -= Listener;
        elementData.isListening = false;
    }

    private void Listener(TestEventData data) {
        Trigger(graphReference, data);
    }
}

public struct TestEventData {

    public string whatever;

}
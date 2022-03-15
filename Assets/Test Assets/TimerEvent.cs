using System.Collections;
using Bolt;
using Ludiq;
using UnityEngine;

[TypeIcon(typeof(Time))]
[UnitCategory("Events/Time")]
[UnitTitle("Repeat")]
public class TimerEvent : EventUnit<int> {
    
    GraphReference graphReference;
    private bool run;
    protected override bool register => false;

    [SerializeAs("seconds")]
    private float _seconds;
    
    [DoNotSerialize]
    [Inspectable]
    [UnitHeaderInspectable("Seconds")]
    public float seconds {
        get => _seconds;
        set => _seconds = Mathf.Max(0, value);
    }

    public override void StartListening(GraphStack stack) {
        graphReference = stack.AsReference();
        Data elementData = stack.GetElementData<Data>(this);
        if (elementData.isListening) return;
        run = true;
        graphReference.component.StartCoroutine(Tick());
        elementData.isListening = true;
    }

    private IEnumerator Tick() {
        while (run) {
            yield return new WaitForSeconds(seconds);
            Trigger(graphReference, 0);
        }
    }

    public override void StopListening(GraphStack stack) {
        Data elementData = stack.GetElementData<Data>(this);
        if (!elementData.isListening) return;
        run = false;
        graphReference.component.StopCoroutine(Tick());
        elementData.isListening = false;
    }
}
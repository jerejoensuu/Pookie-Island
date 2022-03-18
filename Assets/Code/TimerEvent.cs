using System.Collections;
using Bolt;
using Ludiq;
using UnityEngine;

public class TimerHandler : DelegateHandler {
    
    private bool run;
    public TimerEvent eventUnit;
    public TimerHandler(TimerEvent eventUnit) {
        this.eventUnit = eventUnit;
    }

    public override void AssignListeners() {
        run = true;
        graph.component.StartCoroutine(Tick());
    }
    
    private IEnumerator Tick() {
        while (run) {
            yield return new WaitForSeconds(eventUnit.seconds);
            Run();
        }
    }

    public override void UnassignListeners() {
        run = false;
        graph.component.StopCoroutine(Tick());
    }
}

[TypeIcon(typeof(Time))]
[UnitCategory("Events/Time")]
[UnitTitle("Repeat")]
public class TimerEvent : DelegateEventUnit<TimerHandler> {

    protected override TimerHandler InstantiateHandler() => new TimerHandler(this);

    [SerializeAs("seconds")]
    private float _seconds;
    
    [DoNotSerialize]
    [Inspectable]
    [UnitHeaderInspectable("Seconds")]
    public float seconds {
        get => _seconds;
        set => _seconds = Mathf.Max(0, value);
    }
}
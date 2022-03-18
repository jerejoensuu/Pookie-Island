using Bolt;
using UnityEngine.InputSystem;

public class OnUICancelListener : DelegateHandler {
    
    private Inputs inputListener;

    public override void AssignListeners() {
        inputListener = new Inputs();
        inputListener.Enable();
        inputListener.UI.Cancel.performed += CancelOnperformed;
    }

    private void CancelOnperformed(InputAction.CallbackContext obj) {
        Run();
    }

    public override void UnassignListeners() {
        inputListener.Disable();
        inputListener.UI.Cancel.performed -= CancelOnperformed;
    }
}

[UnitTitle("On Input Cancel")]
[UnitCategory("Events")]
public class OnUICancelUnit : DelegateEventUnit<OnUICancelListener> {

    protected override OnUICancelListener InstantiateHandler() => new OnUICancelListener();
}

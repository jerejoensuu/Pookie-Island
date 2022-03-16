using Bolt;
using Ludiq;
using UnityEngine.InputSystem;

[UnitTitle("On Input Cancel")]
[UnitCategory("Events")]
public class OnUICancelUnit : EventUnit<bool> {

    GraphReference graphReference;
    private Inputs inputListener;

    protected override bool register => false;

    public override void StartListening(GraphStack stack) {
        graphReference = stack.AsReference();
        Data elementData = stack.GetElementData<Data>(this);
        if (elementData.isListening) return;
        inputListener = new Inputs();
        inputListener.Enable();
        inputListener.UI.Cancel.performed += CancelOnperformed;
        elementData.isListening = true;
    }

    private void CancelOnperformed(InputAction.CallbackContext obj) {
        Trigger(graphReference, true);
    }

    public override void StopListening(GraphStack stack) {
        Data elementData = stack.GetElementData<Data>(this);
        if (!elementData.isListening) return;
        inputListener.UI.Cancel.performed -= CancelOnperformed;
        inputListener.Dispose();
        inputListener = null;
        elementData.isListening = false;
    }
}

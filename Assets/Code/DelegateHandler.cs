using Bolt;
using Ludiq;

public abstract class DelegateHandler {

    public GraphReference graph;
    public IRunner unit;

    protected void Run() {
        unit.Run(this);
    }
    
    protected void Run(ControlOutput port) {
        unit.Run(this, port);
    }

    public abstract void AssignListeners();
    public abstract void UnassignListeners();

}
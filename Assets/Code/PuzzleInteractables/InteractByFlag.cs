public class InteractByFlag : Interactable {

    public string flagToCheck;
    private void OnEnable() {
        if (FlagUtils.IsFlagOn(flagToCheck)) OnFlagSet(flagToCheck);
        else FlagUtils.OnFlagSet += OnFlagSet;
    }

    private void OnFlagSet(string obj) {
        if (!obj.Equals(flagToCheck)) return;
        OnInteraction?.Invoke(this);
        enabled = false;
    }
    
    private void OnDisable() {
        FlagUtils.OnFlagSet -= OnFlagSet;
    }
}

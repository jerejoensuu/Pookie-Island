using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SmartButton : MonoBehaviour {
    
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)] public Text textField;

    [OnInspectorInit]
    void Bind() {
        textField = transform.GetComponentInChildren<Text>();
    }
}

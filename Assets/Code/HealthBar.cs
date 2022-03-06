using System;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public int HeartWidth;
    public int MaximumHearts = 4;
    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetLives(int amount) {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp(amount, 0, MaximumHearts) * HeartWidth);
    }
}

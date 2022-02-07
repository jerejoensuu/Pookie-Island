using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "PlanetData", order = 1)]
public class PlanetData : ScriptableObject {
    public float gravity = 20;
    public float drag = 4;
    public float angularDrag = 10;
}

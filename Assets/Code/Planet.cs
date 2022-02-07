using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Planet : MonoBehaviour {
    
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public PlanetData planetData;
    private static PlanetData currentPlanetData;

    public static PlanetData GetPlanetData() {
        if (currentPlanetData == null) currentPlanetData = ScriptableObject.CreateInstance<PlanetData>();
        return currentPlanetData;
    }

    private void Awake() {
        currentPlanetData = planetData;
    }
}

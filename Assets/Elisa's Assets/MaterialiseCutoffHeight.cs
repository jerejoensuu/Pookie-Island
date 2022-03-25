using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialiseCutoffHeight : MonoBehaviour
{

    public GameObject parent;
    private Material m;
    public float maxvalue = 2f;
    public float minvalue = -2f;
    public float value = 3.5f;
    float changePerSecond;
    public float duration = 1.01f;

    // Start is called before the first frame update
    void Start()
    {
        m = GetComponent<Renderer>().material;
        m.SetFloat("_CutoffHeight", - 2f);
        changePerSecond = (minvalue - maxvalue) / duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (parent.activeSelf == true)
        {
            value = Mathf.Clamp(value + changePerSecond * Time.deltaTime, minvalue, maxvalue);
            m.SetFloat("_CutoffHeight", - value);
        }

    }
}

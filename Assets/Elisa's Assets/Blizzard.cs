using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blizzard : MonoBehaviour
{
    public float verticalCeiling = 100;
    public ParticleSystem[] blizzardEmmitters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var particleSystem in blizzardEmmitters)
        {
            Ray ray = new Ray(particleSystem.transform.position + Vector3.up * verticalCeiling, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, verticalCeiling))
            {
                particleSystem.enableEmission = false;
            }
            else
            {
                particleSystem.enableEmission = true;
            }
        }
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var particleSystem in blizzardEmmitters)
        {
            Gizmos.DrawRay(particleSystem.transform.position + Vector3.up * verticalCeiling, Vector3.down);
        }
    }
}

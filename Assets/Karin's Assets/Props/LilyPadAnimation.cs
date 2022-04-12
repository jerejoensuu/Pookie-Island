using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPadAnimation : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    int blendShapeCount;
    private SkinnedMeshRenderer renderer;
    Mesh skinnedMesh;
    //private float currentBlendShapeValue = 0;
    private bool isDecreasing = false;
    private float blendOne = 0f;
    private float blendTwo = 0f;
    private bool blendOneFinished = false;

    private void Awake()
    {
        renderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        
    }
    private void Start()
    {
        blendShapeCount = skinnedMesh.blendShapeCount;

    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        //currentBlendShapeValue = renderer.GetBlendShapeWeight(0);

        if (blendShapeCount >2)
            if (blendOne < 100f) {
                renderer.SetBlendShapeWeight(0, blendOne);
                blendOne += movementSpeed;
            } else
            {
                blendOneFinished = true;
            }
        if (blendOneFinished == true && blendTwo < 100f)
        {
            renderer.SetBlendShapeWeight(1, blendTwo);
            blendTwo += movementSpeed;

            //if (!isDecreasing)
            // renderer.SetBlendShapeWeight(0, currentBlendShapeValue + movementSpeed * Time.deltaTime);
            // else
            //  renderer.SetBlendShapeWeight(0, currentBlendShapeValue - movementSpeed * Time.deltaTime);
        }
            }
}

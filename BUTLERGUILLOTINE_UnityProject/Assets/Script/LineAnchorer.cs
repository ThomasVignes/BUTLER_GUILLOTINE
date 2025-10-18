using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAnchorer : MonoBehaviour
{
    public List<GameObject> attachedObjects;
    public LineRenderer lineRenderer;

    void Update()
    {
        for (int i = 0; i < attachedObjects.Count; i++)
        {
            if (i < lineRenderer.positionCount && attachedObjects != null)
            { lineRenderer.SetPosition(i, attachedObjects[i].transform.localPosition); }
        }
    }
}
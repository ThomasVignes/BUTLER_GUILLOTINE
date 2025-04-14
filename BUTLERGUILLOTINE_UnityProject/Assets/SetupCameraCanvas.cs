using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupCameraCanvas : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] float planeDistance;

    private void Start()
    {
        canvas.worldCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();
        canvas.planeDistance = planeDistance;
    }
}

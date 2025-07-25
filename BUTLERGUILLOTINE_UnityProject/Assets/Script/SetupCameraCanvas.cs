using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupCameraCanvas : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] float planeDistance;

    private void Start()
    {
        Camera cam = Camera.main.transform.GetChild(0).GetComponent<Camera>();

        if (cam == null)
             cam = Camera.main;

        canvas.worldCamera = cam;
        canvas.planeDistance = planeDistance;
    }
}

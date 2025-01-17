using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mi_BarMouseParallaxSmooth : MonoBehaviour
{
    public float modifier;
    public float lerpAmount;

    private Vector3 StartPos;



    void Start()
    {
        StartPos = transform.position;
        modifier = -modifier;
    }

    void Update()
    {
        var cam = Camera.main;
        //var ScreenCenter = cam.WorldToViewportPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane));
        var mousePos = cam.ScreenToViewportPoint(Input.mousePosition);
        mousePos.x -= 0.5f;
        mousePos.y -= 0.5f;

        var offset = mousePos;

        transform.position = Vector3.Lerp(transform.position, new Vector3(StartPos.x + (offset.x * modifier), StartPos.y, transform.position.z), lerpAmount);
    }
}

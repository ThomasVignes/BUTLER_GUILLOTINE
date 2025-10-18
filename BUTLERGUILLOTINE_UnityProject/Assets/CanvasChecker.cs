using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasChecker : MonoBehaviour
{
    [SerializeField] float checkDelay = 0.1f;

    Camera UICamera;

    void Start()
    {
        UICamera = Camera.main.gameObject.GetComponentInChildren<Camera>();

        if (UICamera == null)
            return;

        if (checkDelay > 0)
            StartCoroutine(C_Delay());
        else
            CheckAllCanvases();
    }

    void CheckAllCanvases()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        List<Canvas> cameraCanvases = new List<Canvas>();

        foreach (Canvas c in canvases) 
        { 
            if (c.renderMode == RenderMode.ScreenSpaceCamera)
            {
                cameraCanvases.Add(c);
            }
        }

        foreach (Canvas c in cameraCanvases)
        {
            InitCanvas(c);
        }
    }

    IEnumerator C_Delay()
    {
        yield return new WaitForSeconds(checkDelay);

        CheckAllCanvases();
    }

    void InitCanvas(Canvas c)
    {
        if (c.worldCamera != null)
            return;

        c.worldCamera = UICamera;
    }
}

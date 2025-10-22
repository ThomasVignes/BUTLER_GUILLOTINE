using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessOnUI : MonoBehaviour
{
    [SerializeField] bool on;
    float checkDelay = 0.2f;

    Camera UICamera;

    public void Init()
    {
        UICamera = Camera.main.gameObject.GetComponentInChildren<Camera>();

        if (UICamera == null)
            return;

        on = true;

        if (PersistentData.Instance != null)
            on = PersistentData.Instance.PostProcessAffectsUI;

        if (checkDelay > 0)
            StartCoroutine(C_Delay(on));
        else
            ToggleAllCanvases(on);
    }

    public void ToggleAllCanvases(bool on)
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
            InitCanvas(c, on);
        }

        if (PersistentData.Instance != null)
            if (PersistentData.Instance.PostProcessAffectsUI != on)
                PersistentData.Instance.PostProcessAffectsUI = on;
    }

    IEnumerator C_Delay(bool on)
    {
        yield return new WaitForSeconds(checkDelay);

        ToggleAllCanvases(on);
    }

    void InitCanvas(Canvas c, bool on)
    {
        if (c.worldCamera != null)
            return;

        if (on)
            c.worldCamera = UICamera;
        else
            c.worldCamera = null;
    }
}

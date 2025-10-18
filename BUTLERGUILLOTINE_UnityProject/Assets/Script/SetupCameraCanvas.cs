using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupCameraCanvas : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] float planeDistance;

    private void Start()
    {
        StartCoroutine(C_StartDelay());
    }

    IEnumerator C_StartDelay()
    {
        yield return new WaitForSeconds(0.1f);

        var target = Camera.main.gameObject.GetComponentInChildren<UICameraTarget>();

        if (target != null)
        {
            var UICam = target.gameObject.GetComponent<Camera>();

            if (UICam == null)
                UICam = Camera.main;

            canvas.worldCamera = UICam;
            canvas.planeDistance = planeDistance;
        }
    }
}

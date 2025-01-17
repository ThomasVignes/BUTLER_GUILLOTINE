using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSwapper : MonoBehaviour
{
    [Header("LinkedCameras")]
    [SerializeField] CameraZone[] cameras;

    [Header("Objects")]
    [SerializeField] List<GameObject> Specific = new List<GameObject>();
    [SerializeField] List<GameObject> Hide = new List<GameObject>();

    private void Awake()
    {
        foreach (var cam in cameras)
        {
            foreach (var item in Specific)
            {
                cam.ShotSpecificObjects.Add(item);
                RagdollHider hider = item.GetComponent<RagdollHider>();
                if (hider != null)
                {
                    hider.Hide();
                }
                else
                {
                    if (item.activeSelf)
                        item.SetActive(false);
                }
            }

            foreach (var item in Hide)
            {
                cam.ShotSpecificHide.Add(item);
                RagdollHider hider = item.GetComponent<RagdollHider>();
                if (hider != null)
                {
                    hider.Show();
                }
                else
                {
                    if (!item.activeSelf)
                        item.SetActive(true);
                }
            }
        }
    }

    [ContextMenu("EditorShowSpecific")]
    public void ShowSpecific()
    {
        foreach (var item in Specific)
        {
            RagdollHider hider = item.GetComponent<RagdollHider>();
            if (hider != null)
            {
                hider.Show();
            }
            else
            {
                if (!item.activeSelf)
                    item.SetActive(true);
            }
        }

        foreach (var item in Hide)
        {
            RagdollHider hider = item.GetComponent<RagdollHider>();
            if (hider != null)
            {
                hider.Hide();
            }
            else
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }
        }
    }

    [ContextMenu("EditorResetObjects")]
    public void ResetObjects()
    {
        foreach (var item in Specific)
        {
            RagdollHider hider = item.GetComponent<RagdollHider>();
            if (hider != null)
            {
                hider.Hide();
            }
            else
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }
        }

        foreach (var item in Hide)
        {
            RagdollHider hider = item.GetComponent<RagdollHider>();
            if (hider != null)
            {
                hider.Show();
            }
            else
            {
                if (!item.activeSelf)
                    item.SetActive(true);
            }
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CameraDetector>() != null)
        {
            foreach (var item in Specific)
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }

            foreach (var item in Hide) 
            {
                if (!item.activeSelf)
                    item.SetActive(true);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CameraDetector>() != null)
        {
            foreach (var item in Specific)
            {
                if (!item.activeSelf)
                    item.SetActive(true);
            }

            foreach (var item in Hide)
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }
        }
    }
    */
}

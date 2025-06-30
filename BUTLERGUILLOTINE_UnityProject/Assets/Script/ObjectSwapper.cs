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




    public void AddToSpecific(GameObject go)
    {
        Specific.Add(go);

        foreach (var item in cameras)
        {
            item.ShotSpecificObjects.Add(go);
        }
    }

    public void AddToHide(GameObject go)
    {
        Hide.Add(go);

        foreach (var item in cameras)
        {
            item.ShotSpecificHide.Add(go);
        }
    }

    public void RemoveFromSpecific(GameObject go)
    {
        for (int i = 0; i < Specific.Count; i++)
        {
            if (Specific[i] == go)
            {
                Specific.RemoveAt(i);

                foreach (var item in cameras)
                {
                    item.ShotSpecificObjects.Remove(go);
                }
                break;
            }
        }
    }

    public void RemoveFromHide(GameObject go)
    {
        for (int i = 0; i < Hide.Count; i++)
        {
            if (Hide[i] == go)
            {
                Hide.RemoveAt(i);

                foreach (var item in cameras)
                {
                    item.ShotSpecificHide.Remove(go);
                }
                break;
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

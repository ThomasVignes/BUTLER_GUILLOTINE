using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastModeDetection : MonoBehaviour
{
    [SerializeField] GameObject activator;

    private void Start()
    {
        if (PersistentData.Instance != null && PersistentData.Instance.FastMode)
            activator.SetActive(true);
        else
            activator.SetActive(false);
    }
}

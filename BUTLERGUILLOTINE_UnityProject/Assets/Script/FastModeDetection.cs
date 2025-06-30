using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastModeDetection : MonoBehaviour
{
    public bool ConstantlyActive;
    [SerializeField] GameObject activator;

    private void Start()
    {
        if (ConstantlyActive)
        {
            activator.SetActive(true);
            return;
        }

        if (PersistentData.Instance != null && PersistentData.Instance.FastMode)
            activator.SetActive(true);
        else
            activator.SetActive(false);
    }
}

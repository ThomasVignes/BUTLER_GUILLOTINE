using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceLight : MonoBehaviour
{
    public static BounceLight Instance;

    Light directional;

    private void Awake()
    {
        Instance = this;

        directional = GetComponent<Light>();

        if (directional == null) 
            directional = GetComponentInChildren<Light>();
    }

    public void Toggle(bool active)
    {
        directional.enabled = active;
    }
}

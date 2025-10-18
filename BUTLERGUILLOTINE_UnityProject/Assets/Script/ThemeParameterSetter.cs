using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeParameterSetter : MonoBehaviour
{
    [SerializeField] string parameterName;

    ThemeManager themeManager;

    public void SetParam(float value)
    {
        if (themeManager == null)
            themeManager = GetComponent<ThemeManager>();

        if (themeManager == null)
            return;

        themeManager.SetParam(parameterName, value);
    }
}

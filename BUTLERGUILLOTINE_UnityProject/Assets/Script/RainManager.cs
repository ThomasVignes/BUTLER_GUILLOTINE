using DigitalRuby.RainMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManager : MonoBehaviour
{
    public static RainManager Instance;

    float currentRainIntensity;
    RainScript rainScript;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        rainScript = GetComponent<RainScript>();

        currentRainIntensity = 1;
    }

    [ContextMenu("Resume")]
    public void Resume()
    {
        if (!rainScript.Ready)
            return;

        rainScript.RainIntensity = currentRainIntensity;
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        if (!rainScript.Ready)
            return;

        currentRainIntensity = rainScript.RainIntensity;
        rainScript.RainIntensity = 0;
    }
}

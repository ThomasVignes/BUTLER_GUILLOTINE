using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterSceneTheme : MonoBehaviour
{
    EventInstance currentInstance;

    StudioEventEmitter emitter;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        emitter = GetComponent<StudioEventEmitter>();
    }

    public void Play(EventReference reference)
    {
        currentInstance = RuntimeManager.CreateInstance(reference);

        currentInstance.start();
        currentInstance.release();
    }

    public void Stop()
    {
        currentInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
